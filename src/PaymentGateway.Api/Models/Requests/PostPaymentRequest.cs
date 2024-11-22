namespace PaymentGateway.Api.Models.Requests;

public class PostPaymentRequest
{
    public int CardNumberLastFour { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    // There is a note in requirements to ensure I validate against no more than 3 currency codes
    // I think its asking me to only allow 3 ccy so gbp and maybe 2 others
    // but will other currencies have different card structures? see what the mock bank supports....
    public required string Currency { get; set; }
    public int Amount { get; set; }
    public int Cvv { get; set; }
}