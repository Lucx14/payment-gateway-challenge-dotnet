using System;
using System.Threading;
using System.Threading.Tasks;

using FluentValidation;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Extensions;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Domain.Enums;


namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly IPaymentService _paymentService;
    private readonly IValidator<PostPaymentRequest> _createPaymentRequestValidator;
    
    public PaymentsController(
        IPaymentService paymentService, 
        IValidator<PostPaymentRequest> createPaymentRequestValidator)
    {
        _paymentService = paymentService;
        _createPaymentRequestValidator = createPaymentRequestValidator;
    }
    
    [HttpPost(Name = "CreatePayment")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] PostPaymentRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var validationResult = await _createPaymentRequestValidator
            .ValidateAsync(request, cancellationToken)
            .ConfigureAwait(false);

        if (!validationResult.IsValid)
        {
            var failureResponse = new PostPaymentResponse
            {
                Id = null,
                // Do we need to convert this to string?? it shows as an enum int in swagger
                Status = PaymentStatus.Rejected,
                CardNumberLastFour = request.CardNumber[^4..],
                ExpiryMonth = request.ExpiryMonth,
                ExpiryYear = request.ExpiryYear,
                Currency = request.Currency,
                Amount = request.Amount,
            };
            
            return Ok(failureResponse);
        }
        
        var createPaymentResult = await _paymentService
            .CreatePaymentAsync(request.ToInitiatePaymentRequest(), cancellationToken)
            .ConfigureAwait(false);
        
        return Ok(createPaymentResult.ToPostPaymentResponse());
    }
    
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
