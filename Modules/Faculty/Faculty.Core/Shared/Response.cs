using Microsoft.AspNetCore.Http;

namespace Faculty.Core.Shared
{
    public class Response
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string? Error { get; set; }

        public Response(int statusCodes, string message, string? error = null) {
            StatusCode = statusCodes;
            Message = message;
            Error = error;
        }

    }
}
