using System;
using System.Threading;
using System.Threading.Tasks;

using PaymentGateway.Application.Interfaces;
using PaymentGateway.Application.Models.Requests;
using PaymentGateway.Application.Models.Responses;
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
        // pass down the cancellation token
        
        
        // handle what if there is null payment - this already can be null
        return await Task.FromResult(_paymentRepository.GetById(paymentId)).ConfigureAwait(false);
    }

    // need to decide on what the api will send us and where the validation will happen
    public async Task<Payment> CreatePaymentAsync(InitiatePaymentRequest payment, CancellationToken cancellationToken)
    {
        // pass the cancellation token
        // What do we need to return back to the api and then to the merchant
        var createPaymentRequest = new CreatePaymentRequest
        {
            CardNumber = "1234",
            ExpiryDate = "02/2024",
            Currency = "USD",
            Amount = 100,
            Cvv = "123",
        };

        CreatePaymentResponse createPaymentResponse;

        try
        {
            // need to create the payment at the bank  - wrap with a try catch
            createPaymentResponse = await _acquiringBankApiClient.CreatePaymentAsync(createPaymentRequest, cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        // Should i also store the authorization code?
        var newPayment = new Payment
        {
            Id = Guid.NewGuid(),
            Status = PaymentStatus.Authorized,
            CardNumberLastFour = "1234",
            ExpiryMonth = "12",
            ExpiryYear = "2022",
            Currency = "GBP",
            Amount = 100,
        };
        
        // and then save it down
        _paymentRepository.CreatePayment(newPayment);
        
        return newPayment;
    }
}