namespace Agile_Aggregator.Domain.Models
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Data { get; }
        public string? ErrorCode { get; }
        public string? ErrorDetail { get; }

        private Result(T data)
        {
            IsSuccess = true;
            Data = data;
        }

        private Result(string code, string detail)
        {
            IsSuccess = false;
            ErrorCode = code;
            ErrorDetail = detail;
        }

        public static Result<T> Success(T data)
            => new Result<T>(data);

        public static Result<T> Failure(string code, string detail)
            => new Result<T>(code, detail);
    }
}
