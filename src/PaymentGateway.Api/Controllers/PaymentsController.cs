using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Application.Models.Requests;
using PaymentGateway.Domain.Enums;


namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly IPaymentService _paymentService;
    
    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }
    
    // Create Payment
    [HttpPost(Name = "CreatePayment")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] PostPaymentRequest request, CancellationToken cancellationToken)
    {
        // Validation of the request
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        
        // API layer validates that we can process the request
        // Service layer validates that we should process the request
        
        // Translate the request into a service level request 
        var initiatePaymentRequest = new InitiatePaymentRequest
        {
            Amount = request.Amount,
            Currency = request.Currency,
            CardNumber = request.CardNumber,
            ExpiryMonth = request.ExpiryMonth,
            ExpiryYear = request.ExpiryYear,
            Cvv = request.Cvv,
        };
        
        // This should return a dto back to the api level really
        var result = await _paymentService
            .CreatePaymentAsync(initiatePaymentRequest, cancellationToken)
            .ConfigureAwait(false);
        
        // Translate the dto into a postPaymentResponse
        return Ok(result);
    }
    
    // Get Payment
    [HttpGet("{id:guid}", Name = "GetPayment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var payment = await _paymentService.GetPaymentByIdAsync(id, cancellationToken).ConfigureAwait(false);

        if (payment == null)
        {
            return NotFound();
        }
        
        var getPaymentResponse = new GetPaymentResponse
        {
            Id = payment.Id,
            Status = PaymentStatus.Authorized,
            CardNumberLastFour = payment.CardNumberLastFour,
            ExpiryMonth = payment.ExpiryMonth,
            ExpiryYear = payment.ExpiryYear,
            Currency = payment.Currency,
            Amount = payment.Amount,
        };

        return Ok(getPaymentResponse);
    }
}