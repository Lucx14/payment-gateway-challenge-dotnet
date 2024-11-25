using System;

namespace PaymentGateway.Application.Exceptions;

public class ProviderException : Exception
{
    public ProviderError ProviderError { get; }

    public ProviderException(string message) : base(message)
    {
    }

    public ProviderException()
    {
    }

    public ProviderException(string message, Exception innerException, ProviderError providerError) : base(message,
        innerException)
    {
        ProviderError = providerError;
    }

    public ProviderException(string message, ProviderError providerError) : base(message)
    {
        ProviderError = providerError;
    }


    public ProviderException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public enum ProviderError
{
    Unknown,
    BadRequest,
    ServerError,
    InvalidData,
    Timeout
}
