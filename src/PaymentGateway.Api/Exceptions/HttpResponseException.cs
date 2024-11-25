using System;

namespace PaymentGateway.Api.Exceptions;

public class HttpResponseException : Exception
{
    public HttpResponseException(int statusCode, object? value = null) =>
        (StatusCode, Value) = (statusCode, value);

    public int StatusCode { get; }

    public object? Value { get; }

    public HttpResponseException()
    {
    }

    public HttpResponseException(string message) : base(message)
    {
    }

    public HttpResponseException(string message, Exception innerException) : base(message, innerException)
    {
    }
}