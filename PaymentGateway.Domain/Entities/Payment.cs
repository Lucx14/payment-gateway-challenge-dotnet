using System;

using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Domain.Entities;

public class Payment
{
    public Guid Id { get; init; }
    public PaymentStatus Status { get; set; }
    public int CardNumberLastFour { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public required string Currency { get; set; }
    public int Amount { get; set; }
}