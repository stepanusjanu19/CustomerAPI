using System.Text.Json.Serialization;

namespace Application.Response
{
    public class ApiResponse<T>
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("transactionId")]
        public Guid TransactionId { get; set; }

        [JsonPropertyName("data")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T Data { get; set; }

        [JsonPropertyName("errors")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<string?> Errors { get; set; }

        public ApiResponse(string message, T data = default, IEnumerable<string> errors = null)
        {
            this.Message = message;
            this.TransactionId = Guid.NewGuid();
            this.Data = data;
            this.Errors = errors;
        }
    }
}
