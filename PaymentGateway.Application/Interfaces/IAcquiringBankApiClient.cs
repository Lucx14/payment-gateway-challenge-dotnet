using System.Threading;
using System.Threading.Tasks;

using PaymentGateway.Application.Models.Requests;
using PaymentGateway.Application.Models.Responses;

namespace PaymentGateway.Application.Interfaces;

public interface IAcquiringBankApiClient
{
    Task<CreatePaymentResponse> CreatePaymentAsync(CreatePaymentRequest createPaymentRequest, CancellationToken cancellationToken);
}