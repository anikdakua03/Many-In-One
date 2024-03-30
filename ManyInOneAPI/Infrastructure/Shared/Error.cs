namespace ManyInOneAPI.Infrastructure.Shared
{
    public record Error
    {
        public static readonly Error None = new (string.Empty, string.Empty, ErrorType.Failure);
        public static readonly Error NullValue = new("Error.NullValue", "Null value was provided", ErrorType.Failure);

        private Error(string code, string description, ErrorType errorType)
        {
            Code = code;
            Description = description;
            Type = errorType;
        }
        public string Code { get; }
        public string Description { get; }
        public ErrorType Type { get; }

        public static Error Failure(string code, string description) => new Error(code, description, ErrorType.Failure);
        public static Error Validation(string code, string description) => new Error(code, description, ErrorType.Validation);
        public static Error NotFound(string code, string description) => new Error(code, description, ErrorType.NotFound);
        public static Error Conflict(string code, string description) => new Error(code, description, ErrorType.Conflict);
    }

    public enum ErrorType 
    {
        Failure = 0, // General fail
        Validation = 1, // Status Code 400 Bad request
        NotFound = 2, // Status Code 404 Not Found
        Conflict = 3 // Status Code 409
    }
}
