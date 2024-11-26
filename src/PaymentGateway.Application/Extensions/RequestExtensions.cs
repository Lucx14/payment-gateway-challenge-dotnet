using System;

using PaymentGateway.Application.Models.Requests;

namespace PaymentGateway.Application.Extensions;

public static class RequestExtensions
{
    public static CreatePaymentRequest ToCreatePaymentRequest(this InitiatePaymentRequest initiatePaymentRequest)
    {
        ArgumentNullException.ThrowIfNull(initiatePaymentRequest);
        
        var cardExpiry = $"{initiatePaymentRequest.ExpiryMonth}/{initiatePaymentRequest.ExpiryYear}";
        
        return new CreatePaymentRequest
        {
            CardNumber = initiatePaymentRequest.CardNumber,
            ExpiryDate = cardExpiry,
            Currency = initiatePaymentRequest.Currency,
            Amount = initiatePaymentRequest.Amount,
            Cvv = initiatePaymentRequest.Cvv,
        };
    }
}