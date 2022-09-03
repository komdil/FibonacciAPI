using System.Text.Json.Serialization;

namespace FibonacciAPI.Responses
{
    public class ServerResponse<T>
    {
        /// <summary>
        /// Return with success response
        /// </summary>
        public ServerResponse(T data)
        {
            Success = true;
            Data = data;
        }

        /// <summary>
        /// Return with fail response
        /// </summary>
        public ServerResponse(IReadOnlyList<ErrorResponse> messages)
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
