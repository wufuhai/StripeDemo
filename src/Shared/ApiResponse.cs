namespace QNE.Models.ViewModel
{
    public class ApiResponse<T> : ApiResponseBase
    {
        public T Data { get; set; }
    }
    public abstract class ApiResponseBase
    {
        public bool Success => Code == "0000";
        public string Code { get; set; } = "0000";
        public string Message { get; set; } = "Operation completed successfully.";
    }
}