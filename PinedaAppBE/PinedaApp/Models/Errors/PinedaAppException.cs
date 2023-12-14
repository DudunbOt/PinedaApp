using Microsoft.AspNetCore.Connections.Features;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace PinedaApp.Models.Errors
{
    public class PinedaAppException : Exception
    {
        public int ErrorCode { get; }
        public PinedaAppException(string message) : base(message) { }
        public PinedaAppException(string message, int errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }
        public PinedaAppException(string message, int errorCode, Exception innerException) : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
}