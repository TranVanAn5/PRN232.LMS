namespace PRN232.LMS.Repositories.Models
{
    public class ResponseWrapper<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public object Errors { get; set; }
        public PaginationMetadata Pagination { get; set; }

        public ResponseWrapper() { }

        public ResponseWrapper(bool success, string message, T data = default, object errors = null, PaginationMetadata pagination = null)
        {
            Success = success;
            Message = message;
            Data = data;
            Errors = errors;
            Pagination = pagination;
        }

        public static ResponseWrapper<T> SuccessResponse(T data, string message = "Request processed successfully", PaginationMetadata pagination = null)
            => new(true, message, data, null, pagination);

        public static ResponseWrapper<T> FailureResponse(string message, object errors = null)
            => new(false, message, default, errors);

        public static ResponseWrapper<T> NotFoundResponse(string message = "Resource not found")
            => new(false, message, default);
    }
}
