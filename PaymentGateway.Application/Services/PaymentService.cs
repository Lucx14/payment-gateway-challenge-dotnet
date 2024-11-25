using System;
using System.Collections.Generic;
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
        ArgumentNullException.ThrowIfNull(initiatePaymentRequest);
        
        var newPaymentId = Guid.NewGuid();

        try
        {
            var newPayment = _paymentRepository.CreatePayment(new Payment
            {
                Id = newPaymentId,
                Status = PaymentStatus.Initiated,
                CardNumberLastFour = initiatePaymentRequest.CardNumber[^4..],
                ExpiryMonth = initiatePaymentRequest.ExpiryMonth,
                ExpiryYear = initiatePaymentRequest.ExpiryYear,
                Currency = initiatePaymentRequest.Currency,
                Amount = initiatePaymentRequest.Amount,
            });

            var createPaymentResponse = await _acquiringBankApiClient
                .CreatePaymentAsync(initiatePaymentRequest.ToCreatePaymentRequest(), cancellationToken)
                .ConfigureAwait(false);

            newPayment.UpdateStatus(
                createPaymentResponse.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined);

            return newPayment.ToPaymentDto();
        }
        catch (ProviderException ex)
        {
            return HandleProviderException(ex, newPaymentId);
        }
    }
    
    private PaymentDto HandleProviderException(ProviderException ex, Guid newPaymentId)
    {
        var failureStatusMapped = ErrorStatusMap.TryGetValue(ex.ProviderError, out var failureStatus);
        
        if (!failureStatusMapped)
        {
            throw new GatewayApplicationException(
                "Gateway Error: Invalid Internal Error Mapping",
                GatewayApplicationError.InternalServerError);
        }
        
        return HandleUnprocessablePayment(newPaymentId, failureStatus);
    }

    private PaymentDto HandleUnprocessablePayment(Guid paymentId, PaymentStatus paymentStatus)
    {
        var payment = _paymentRepository.GetById(paymentId);
        
        if (payment is null)
        {
            throw new GatewayApplicationException(
                "Gateway Error: Payment not found.",
                GatewayApplicationError.InternalServerError);
        }

        payment.UpdateStatus(paymentStatus);

        return payment.ToPaymentDto();
    }
    
    private static readonly Dictionary<ProviderError, PaymentStatus> ErrorStatusMap = new()
    {
        { ProviderError.InvalidData, PaymentStatus.Failed },
        { ProviderError.Timeout, PaymentStatus.Failed },
        { ProviderError.ServerError, PaymentStatus.Failed },
        { ProviderError.BadRequest, PaymentStatus.Rejected }
    };

}