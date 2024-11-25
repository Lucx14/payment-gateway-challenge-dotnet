using System;
using System.Threading;
using System.Threading.Tasks;

using PaymentGateway.Application.Models.Requests;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Application.Interfaces;

public interface IPaymentService
{
    // think about a better response for this if i have time
    Task<Payment?> GetPaymentByIdAsync(Guid paymentId, CancellationToken cancellationToken);
    Task<Payment> CreatePaymentAsync(InitiatePaymentRequest payment, CancellationToken cancellationToken);
}