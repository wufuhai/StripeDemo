using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QNE.App.Client.ClientServices;
using StripeDemo.Shared;
using System.Threading.Tasks;

namespace StripeDemo.Client.Pages
{
    public partial class Success
    {
        [Parameter]
        public string SessionId { get; set; }

        [Inject] IHttpService Http { get; set; }

        [Inject] NavigationManager Nav { get; set; }
        [Inject] IJSRuntime Js { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        private async Task ManageBilling()
        {
            var resp = await Http.PostAsync<ManageBillingResultModel>("/api/stripe/ManageBilling", new ManageBillingModel { SessionId = SessionId });
            if (resp != null)
            {
                await Js.InvokeVoidAsync("open", resp.RedirectUrl, "_blank");
            }
        }
    }
}