using System;

namespace PaymentGateway.Application.Exceptions;

public class GatewayApplicationException : Exception
{
    public GatewayApplicationError GatewayApplicationError { get; }

    public GatewayApplicationException(string message) : base(message)
    {
    }

    public GatewayApplicationException()
    {
    }

    public GatewayApplicationException(string message, Exception innerException, GatewayApplicationError gatewayApplicationError) : base(
        message, innerException)
    {
        GatewayApplicationError = gatewayApplicationError;
    }
    
    public GatewayApplicationException(string message, GatewayApplicationError gatewayApplicationError) : base(message)
    {
        GatewayApplicationError = gatewayApplicationError;
        
    }
    
    public GatewayApplicationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}


public enum GatewayApplicationError
{
    NotFound,
    BadRequest,
    InternalServerError,
    ExternalProviderError,
}