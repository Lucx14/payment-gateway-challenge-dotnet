namespace PaymentGateway.Application.Models.Responses;

public class CreatePaymentResponse
{
    public required bool Authorized { get; init; }

    public required string AuthorizationCode { get; init; }
}