using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using PaymentGateway.Application.Exceptions;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Application.Models.Requests;
using PaymentGateway.Application.Models.Responses;
using PaymentGateway.Infrastructure.ExternalServices.AcquiringBanks.BankSimulator.Models;

namespace PaymentGateway.Infrastructure.ExternalServices.AcquiringBanks.BankSimulator;

public class BankSimulatorApiClient : IAcquiringBankApiClient
{
    private readonly HttpClient _httpClient;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public BankSimulatorApiClient(HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(httpClient.BaseAddress);
        
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(10);
    }

    public async Task<CreatePaymentResponse> CreatePaymentAsync(CreatePaymentRequest payload, CancellationToken cancellationToken)
    {
        var uri = new Uri(_httpClient.BaseAddress!, "payments");
        var body = JsonSerializer.Serialize(payload, _jsonSerializerOptions);
        
        using var content = new StringContent(body, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(uri, content, cancellationToken).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var data = await response.Content
                .ReadFromJsonAsync<BankSimulatorResponse>(cancellationToken)
                .ConfigureAwait(false) ?? throw new ProviderException("Data deserialization error", ProviderError.InvalidData);

            return data.ToCreatePaymentResponse();
        }
        catch (JsonException ex)
        {
            throw new ProviderException(ex.Message, ex, ProviderError.InvalidData);
        }
        catch (HttpRequestException ex)
        {
            return HandleHttpRequestException(ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new ProviderException(ex.Message, ex, ProviderError.Timeout);
        }
    }
    
    private static CreatePaymentResponse HandleHttpRequestException(HttpRequestException ex)
    {
        throw ex.StatusCode switch
        {
            HttpStatusCode.BadRequest => new ProviderException(ex.Message, ex, ProviderError.BadRequest),
            HttpStatusCode.InternalServerError => new ProviderException(ex.Message, ex, ProviderError.ServerError),
            _ => new ProviderException(ex.Message, ex, ProviderError.Unknown)
        };
    }
}
