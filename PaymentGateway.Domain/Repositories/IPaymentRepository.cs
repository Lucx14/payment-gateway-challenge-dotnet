using System;

using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Domain.Repositories;

public interface IPaymentRepository
{
    Payment? GetById(Guid id);
    
    Payment CreatePayment(Payment payment);
}