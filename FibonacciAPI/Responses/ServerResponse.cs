using System.Text.Json.Serialization;

namespace FibonacciAPI.Responses
{
    public class ServerResponse<T>
    {
        public static ServerResponse<T> GetSuccessResponse(T data)
        {
            return new ServerResponse<T>(data);
        }

        public static ServerResponse<T> GetFailResponse(IReadOnlyList<ErrorResponse> messages)
        {
            return new ServerResponse<T>(messages);
        }

        ServerResponse(T data)
        {
            Success = true;
            Data = data;
        }

        ServerResponse(IReadOnlyList<ErrorResponse> messages)
        {
            Success = false;
            Messages = messages;
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T Data { get; }

        public bool Success { get; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyList<ErrorResponse> Messages { get; }
    }

    public class ErrorResponse
    {
        public ErrorResponse(string propertyName, List<string> errorMessages)
        {
            PropertyName = propertyName;
            ErrorMessages = errorMessages.AsReadOnly();
        }

        public string PropertyName { get; }

        public IReadOnlyList<string> ErrorMessages { get; }
    }
}
