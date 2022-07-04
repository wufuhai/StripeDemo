using AntDesign;
using Microsoft.AspNetCore.Components;
using QNE.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace QNE.App.Client.ClientServices
{
    public interface IHttpService
    {
        ApiHttpClient ApiClient { get; }

        HttpClient AppClient { get; }

        bool Loading { get; }
        Task<T> GetAsync<T>(string uri, object queryParams = null, bool silent = true, bool nowrap = false);
        //Task<int> Count(string uri, QueryOptions options);

        Task<T> PostAsync<T>(string uri, object body, bool silent = false);
        Task<T> DeleteAsync<T>(string uri, object body = null);
        Task<bool> DeleteAsync(string uri, object key);
        Task<T> ExecuteAsync<T>(HttpMethod method, string uri, object body = null, object queryParams = null, bool silent = false, bool nowrap = false);
    }

    public class ApiHttpClient : HttpClient
    {
       
    }

    public class HttpService : IHttpService
    {
        private readonly NavigationManager _nav;
        private readonly MessageService _message;
        private readonly ModalService _modalService;


        public HttpService(ApiHttpClient apiClient, HttpClient appClient, NavigationManager nav, MessageService message, ModalService modalService)
        {
            _nav = nav;
            _message = message;
            _modalService = modalService;
            ApiClient = apiClient;
            AppClient = appClient;
        }

        public ApiHttpClient ApiClient { get; }

        public HttpClient AppClient { get; }

        public bool Loading { get; private set; }

        public async Task<T> GetAsync<T>(string uri, object queryParams, bool silent = true, bool nowrap = false)
        {
            return await ExecuteAsync<T>(HttpMethod.Get, uri, null, queryParams, silent, nowrap);
        }

        public async Task<T> PostAsync<T>(string uri, object body, bool silent = false)
        {
            return await ExecuteAsync<T>(HttpMethod.Post, uri, body, silent: silent);
        }

        public async Task<T> DeleteAsync<T>(string uri, object body = null)
        {
            return await ExecuteAsync<T>(HttpMethod.Delete, uri, body);
        }

        public async Task<bool> DeleteAsync(string uri, object key)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{uri}({key})") ;
            
            var resp = await SendRequest<object>(request);
            return resp.Success;
        }

        public async Task<T> ExecuteAsync<T>(HttpMethod method, string uri, object body = null, object queryParams = null, bool silent = false, bool nowrap = false)
        {
            var request = queryParams == null ? new HttpRequestMessage(method, uri) : new HttpRequestMessage(method, uri + "?" + GetQueryString(queryParams));

            if (body != null)
                request.Content = JsonContent.Create(body);

            var resp = await SendRequest<T>(request, silent: silent, nowrap: nowrap);
            return resp.Data;
        }

        private async Task<HttpResponseContent<T>> SendRequest<T>(HttpRequestMessage request, bool silent = false, bool nowrap = false)
        {
            Loading = true;

            var result = new HttpResponseContent<T>();
            try
            {
                using var response = await ApiClient.SendAsync(request);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return result;
                }

                var isJson = response.Content.Headers.ContentType?.MediaType == "application/json";
                if (!isJson)
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                        ShowMessage(MessageType.Error, "Invalid Api Endpoint: " + request.RequestUri!.LocalPath);
                    else
                        HandleInvalidRequestMessage(response.StatusCode, request.RequestUri!.LocalPath);

                    return result;
                }

                //var isApiData = request.RequestUri?.AbsolutePath.StartsWith("api") ?? false;
                // Handle api response
                if (!nowrap)
                {
                    silent = silent || request.RequestUri?.Query.Contains("_silent") == true;
                    if (response.IsSuccessStatusCode)
                    {
                        var resp = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
                        if (resp == null) return default;

                        if (!silent)
                        {
                            HandleMessage(resp);
                        }
                        result.Data = resp.Data;
                        result.Success = resp.Success;
                        return result;
                    }
                    else
                    {
                        var resp = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
                        if (!silent)
                        {
                            HandleMessage(resp);
                        }
                    }
                }
                // Handle raw data response
                else
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var resp = await response.Content.ReadFromJsonAsync<T>();
                        result.Data = resp ?? default;
                        result.Success = true;
                    }
                    else
                    {
                        ShowMessage(MessageType.Error, $"Unexpected Response: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage(MessageType.Error, ex.Message);
                throw;
            }
            finally
            {
                Loading = false;
            }

            return result;
        }

        private void HandleInvalidRequestMessage(HttpStatusCode statusCode, string uri)
        {
            _message.Warning($"HTTP {statusCode}: " + uri);
        }

        private void HandleMessage(ApiResponseBase respMessage)
        {
            if (string.IsNullOrEmpty(respMessage.Message)) return;

            if (respMessage.Success)
            {
                ShowMessage(MessageType.Success, respMessage.Message);
            }
            else
            {
                if (respMessage.Code == "EXCEPTION")
                {
                    _modalService.ErrorAsync(new ConfirmOptions()
                    {
                        Title = "Server Exception",
                        Content = respMessage.Message
                    });
                }
                else
                    ShowMessage(MessageType.Warning, respMessage.Message);
            }
        }

        private void ShowMessage(MessageType type, string message)
        {
            try
            {
                var config = new MessageConfig()
                {
                    Content = message,
                    Duration = 3,
                    Type = type
                };
                _message.Open(config);
            }
            catch
            {
                // ignored
            }
        }

        private string GetQueryString(object obj)
        {
            if (obj == null)
                return string.Empty;

            var json = JsonSerializer.Serialize(obj);
            var paramValues = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            var properties = from p in paramValues
                             where p.Value != null
                             select p.Key + "=" + p.Value;

            return string.Join("&", properties.ToArray());
        }

        class HttpResponseContent<T>
        {
            public bool Success { get; set; }
            public T Data { get; set; }
        }
    }
}