using System;
using System.Collections.Generic;
using System.Linq;

using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Repositories;

namespace PaymentGateway.Infrastructure.Repositories;

public class InMemoryPaymentRepository : IPaymentRepository
{
    private readonly List<Payment> _payments = [];

    public Payment CreatePayment(Payment payment)
    {
        _payments.Add(payment);
        return payment;
    }

    public Payment? GetById(Guid id)
    {
        return _payments.FirstOrDefault(p => p.Id == id);
    }
}
