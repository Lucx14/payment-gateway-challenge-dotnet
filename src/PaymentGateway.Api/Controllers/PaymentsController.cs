using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FluentValidation;
using FluentValidation.Results;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

using PaymentGateway.Api.Extensions;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Application.Interfaces;


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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] PostPaymentRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var validationResult = await _createPaymentRequestValidator
            .ValidateAsync(request, cancellationToken)
            .ConfigureAwait(false);

        if (!validationResult.IsValid)
        {
            return HandleValidationFailure(validationResult);
        }
        
        var createPaymentResult = await _paymentService
            .CreatePaymentAsync(request.ToInitiatePaymentRequest(), cancellationToken)
            .ConfigureAwait(false);

        return CreatedAtAction(nameof(Get), new { id = createPaymentResult.Id }, createPaymentResult.ToPostPaymentResponse());
    }
    
    [HttpGet("{id:guid}", Name = "GetPayment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var getPaymentResult = await _paymentService.GetPaymentByIdAsync(id, cancellationToken).ConfigureAwait(false);

        if (getPaymentResult == null)
        {
            return NotFound();
        }

        return Ok(getPaymentResult.ToGetPaymentResponse());
    }
    
    [Route("/error")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult HandleError() => Problem();
    
    [Route("/error-development")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult HandleErrorDevelopment([FromServices] IHostEnvironment hostEnvironment)
    {
        if (!hostEnvironment.IsDevelopment())
        {
            return NotFound();
        }
    
        var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>()!;
    
        return Problem(
            detail: exceptionHandlerFeature.Error.StackTrace,
            title: exceptionHandlerFeature.Error.Message);
    }

    private BadRequestObjectResult HandleValidationFailure(ValidationResult validationResult)
    {
        var errorResponse = new ValidationErrorResponse(
            "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            "Validation Error",
            400,
            validationResult.Errors
                .Select(e =>
                {
                    var propertyName = string.IsNullOrWhiteSpace(e.PropertyName) ? "ExpiryDate" : e.PropertyName;
                    return new ValidationError(propertyName, e.ErrorMessage);
                })
                .ToList()
                .AsReadOnly()
        );
        
        return BadRequest(errorResponse);
    }
}
