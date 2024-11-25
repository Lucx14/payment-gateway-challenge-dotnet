namespace PaymentGateway.Application.Models.Requests;

public class CreatePaymentRequest
{
    public required string CardNumber { get; init; }
    public required string ExpiryDate { get; init; }
    public required string Currency { get; init; }
    public required int Amount { get; init; }
    public required string Cvv { get; init; }
}