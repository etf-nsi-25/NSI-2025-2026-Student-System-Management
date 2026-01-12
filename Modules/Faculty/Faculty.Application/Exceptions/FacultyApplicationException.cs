using System.Net;

namespace Faculty.Application.Exceptions;

public class FacultyApplicationException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public FacultyApplicationException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public FacultyApplicationException(string message, HttpStatusCode statusCode, Exception? innerException)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}
