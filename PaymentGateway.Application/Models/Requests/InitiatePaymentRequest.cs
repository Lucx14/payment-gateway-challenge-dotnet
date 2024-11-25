namespace PaymentGateway.Application.Models.Requests;

public class InitiatePaymentRequest
{
    public required string CardNumber { get; init; }
    public required string ExpiryMonth { get; init; }
    public required string ExpiryYear { get; init; }
    public required string Currency { get; init; }
    public required int Amount { get; init; }
    public required string Cvv { get; init; }
}