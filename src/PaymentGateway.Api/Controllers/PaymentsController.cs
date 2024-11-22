using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly PaymentsRepository _paymentsRepository;

    public PaymentsController(PaymentsRepository paymentsRepository)
    {
        _paymentsRepository = paymentsRepository;
    }

    // This Get Payment Request is the 2nd requirement where the merchant wants to retrieve the details
    // of a previously made payment using its identifier.
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostPaymentResponse?>> GetPaymentAsync(Guid id)
    {
        var payment = await Task.FromResult(_paymentsRepository.Get(id)).ConfigureAwait(false);

        return new OkObjectResult(payment);
    }
}