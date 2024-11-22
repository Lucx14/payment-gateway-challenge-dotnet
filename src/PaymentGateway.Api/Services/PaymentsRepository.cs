using System;
using System.Collections.Generic;
using System.Linq;

using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public class PaymentsRepository
{
    private readonly List<PostPaymentResponse> _payments = [];

    public void Add(PostPaymentResponse payment)
    {
        _payments.Add(payment);
    }

    public PostPaymentResponse? Get(Guid id)
    {
        var dummyResponse = new PostPaymentResponse
        {
            Id = id,
            Status = PaymentStatus.Authorized,
            CardNumberLastFour = 1234,
            ExpiryMonth = 12,
            ExpiryYear = 2027,
            Currency = "SGD",
            Amount = 100,
        };

        _payments.Add(dummyResponse);

        return _payments.FirstOrDefault(p => p.Id == id);
    }
}