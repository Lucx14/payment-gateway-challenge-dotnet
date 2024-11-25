using System;

using PaymentGateway.Application.DTOs;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Application.Extensions;

public static class PaymentExtensions
{
    public static PaymentDto ToPaymentDto(this Payment payment)
    {
        ArgumentNullException.ThrowIfNull(payment);

        return new PaymentDto
        {
            Id = payment.Id,
            Status = payment.Status,
            CardNumberLastFour = payment.CardNumberLastFour,
            ExpiryMonth = payment.ExpiryMonth,
            ExpiryYear = payment.ExpiryYear,
            Currency = payment.Currency,
            Amount = payment.Amount,
        };
    }
}