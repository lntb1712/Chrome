namespace Chrome.DTO
{
    public class ServiceResponse<T>
    {
        public bool Success { get; set; }

        public string Message { get; set; }
        public T Data { get; set; }

        public ServiceResponse(bool success, string message, T data=default!)
        {
            Success = success;
            Message = message;
            Data = data;
        }
    }
}
