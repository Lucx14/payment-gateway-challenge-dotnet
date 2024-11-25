using System;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Application.DTOs;
using PaymentGateway.Application.Models.Requests;

namespace PaymentGateway.Api.Extensions;

public static class DtoExtensions
{
    public static PostPaymentResponse ToPostPaymentResponse(this PaymentDto paymentDto)
    {
        ArgumentNullException.ThrowIfNull(paymentDto);

        return new PostPaymentResponse
        {
            Id = paymentDto.Id,
            Status = paymentDto.Status.ToString(),
            CardNumberLastFour = paymentDto.CardNumberLastFour,
            ExpiryMonth = paymentDto.ExpiryMonth,
            ExpiryYear = paymentDto.ExpiryYear,
            Currency = paymentDto.Currency,
            Amount = paymentDto.Amount,
        };
    }

    public static InitiatePaymentRequest ToInitiatePaymentRequest(this PostPaymentRequest postPaymentRequest)
    {
        ArgumentNullException.ThrowIfNull(postPaymentRequest);

        return new InitiatePaymentRequest
        {
            Amount = postPaymentRequest.Amount,
            Currency = postPaymentRequest.Currency,
            CardNumber = postPaymentRequest.CardNumber,
            ExpiryMonth = postPaymentRequest.ExpiryMonth,
            ExpiryYear = postPaymentRequest.ExpiryYear,
            Cvv = postPaymentRequest.Cvv,
        };
    }

    public static GetPaymentResponse ToGetPaymentResponse(this PaymentDto paymentDto)
    {
        ArgumentNullException.ThrowIfNull(paymentDto);
        
        return new GetPaymentResponse
        {
            Id = paymentDto.Id,
            Status = paymentDto.Status.ToString(),
            CardNumberLastFour = paymentDto.CardNumberLastFour,
            ExpiryMonth = paymentDto.ExpiryMonth,
            ExpiryYear = paymentDto.ExpiryYear,
            Currency = paymentDto.Currency,
            Amount = paymentDto.Amount,
        };
    }
}