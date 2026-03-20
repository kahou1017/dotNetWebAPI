namespace Dimensions.Application.Exceptions;

public sealed class DimensionsApplicationException : Exception
{
    public DimensionsApplicationException(int statusCode, string errorCode, string errorMessage)
        : base(errorMessage)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public int StatusCode { get; }

    public string ErrorCode { get; }

    public string ErrorMessage { get; }
}
