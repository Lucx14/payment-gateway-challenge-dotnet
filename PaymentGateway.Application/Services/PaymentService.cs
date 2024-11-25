using System;
using System.Threading;
using System.Threading.Tasks;

using PaymentGateway.Application.DTOs;
using PaymentGateway.Application.Exceptions;
using PaymentGateway.Application.Extensions;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Application.Models.Requests;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Enums;
using PaymentGateway.Domain.Repositories;

namespace PaymentGateway.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IAcquiringBankApiClient _acquiringBankApiClient;

    public PaymentService(IPaymentRepository paymentRepository, IAcquiringBankApiClient acquiringBankApiClient)
    {
        _paymentRepository = paymentRepository;
        _acquiringBankApiClient = acquiringBankApiClient;
    }
    
    public async Task<Payment?> GetPaymentByIdAsync(Guid paymentId, CancellationToken cancellationToken)
    {
        return await Task
            .FromResult(_paymentRepository.GetById(paymentId))
            .ConfigureAwait(false);
    }

    public async Task<PaymentDto> CreatePaymentAsync(InitiatePaymentRequest initiatePaymentRequest, CancellationToken cancellationToken)
    {
        try
        {
            var createPaymentResponse = await _acquiringBankApiClient
                .CreatePaymentAsync(initiatePaymentRequest.ToCreatePaymentRequest(), cancellationToken)
                .ConfigureAwait(false);
            
            var newPayment = new Payment
            {
                Id = Guid.NewGuid(),
                Status = createPaymentResponse.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined,
                CardNumberLastFour = initiatePaymentRequest.CardNumber[^4..],
                ExpiryMonth = initiatePaymentRequest.ExpiryMonth,
                ExpiryYear = initiatePaymentRequest.ExpiryYear,
                Currency = initiatePaymentRequest.Currency,
                Amount = initiatePaymentRequest.Amount,
            };
            
            _paymentRepository.CreatePayment(newPayment);
            
            return newPayment.ToPaymentDto();
        }
        catch (ProviderException ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
}