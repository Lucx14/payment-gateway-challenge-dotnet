using System;

using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Domain.Entities;

public class Payment
{
    public required Guid Id { get; init; }
    public required PaymentStatus Status { get; set; }
    public required string CardNumberLastFour { get; init; }
    public required string ExpiryMonth { get; init; }
    public required string ExpiryYear { get; init; }
    public required string Currency { get; init; }
    public required int Amount { get; init; }

    public void UpdateStatus(PaymentStatus status)
    {
        Status = status;
    }
}
