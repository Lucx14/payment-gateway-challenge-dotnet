namespace PaymentGateway.Application.Models.Requests;

public class InitiatePaymentRequest
{
    public required string CardNumber { get; set; }
    public required string ExpiryMonth { get; set; }
    public required string ExpiryYear { get; set; }
    // There is a note in requirements to ensure I validate against no more than 3 currency codes
    // I think its asking me to only allow 3 ccy so gbp and maybe 2 others
    // but will other currencies have different card structures? see what the mock bank supports....
    public required string Currency { get; set; }
    public required int Amount { get; set; }
    public required string Cvv { get; set; }
}