using System;
using System.Threading;
using System.Threading.Tasks;

using PaymentGateway.Application.DTOs;
using PaymentGateway.Application.Models.Requests;

namespace PaymentGateway.Application.Interfaces;

public interface IPaymentService
{
    Task<PaymentDto?> GetPaymentByIdAsync(Guid paymentId, CancellationToken cancellationToken);
    Task<PaymentDto> CreatePaymentAsync(InitiatePaymentRequest initiatePaymentRequest, CancellationToken cancellationToken);
}