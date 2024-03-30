using Microsoft.AspNetCore.Mvc;

namespace ManyInOneAPI.Infrastructure.Shared
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Data { get; }
        public Error? Error { get; }

        public Result(bool isSuccess, T? data, Error? error = null)
        {
            IsSuccess = isSuccess;
            Data = data;
            Error = error;
        }

        public static Result<T> Success(T data) => new Result<T>(true, data);
        public static Result<T> Failure(Error error) => new Result<T>(false, default(T), error);

        public static implicit operator Result<T>(T value) => Success(value);
    }
}
