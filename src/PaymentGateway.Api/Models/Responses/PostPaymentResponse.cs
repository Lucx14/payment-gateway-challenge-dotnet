﻿using System;

namespace PaymentGateway.Api.Models.Responses;

public class PostPaymentResponse
{
    public required Guid Id { get; init; }
    public required string Status { get; init; }
    public required string CardNumberLastFour { get; init; }
    public required string ExpiryMonth { get; init; }
    public required string ExpiryYear { get; init; }
    public required string Currency { get; init; }
    public required int Amount { get; init; }
}
