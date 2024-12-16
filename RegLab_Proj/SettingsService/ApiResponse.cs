namespace RegLab_Test.SettingsService
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
        public int? ErrorCode { get; set; }
        public ApiResponse(T data, string message = null)
        {
            Success = true;
            Data = data;
            Message = message;
        }
        public ApiResponse(string message, int? errorCode = null)
        {
            Success = false;
            Message = message;
            ErrorCode = errorCode;
        }
    }
}
