using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Infrastructure.ExternalServices.AcquiringBanks;

public class BankSimulatorApiClient
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

    public async Task<BankSimulatorResponse> CreatePaymentAsync(BankSimulatorRequest payload, CancellationToken cancellationToken)
    {
        var uri = new Uri(_httpClient.BaseAddress!, "payments");
        var body = JsonSerializer.Serialize(payload, _jsonSerializerOptions);
        
        using var content = new StringContent(body, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(uri, content, cancellationToken).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<BankSimulatorResponse>(cancellationToken)
                .ConfigureAwait(false) ?? throw new ProviderException("Data deserialization error", ProviderError.InvalidData);
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
    
    private static BankSimulatorResponse HandleHttpRequestException(HttpRequestException ex)
    {
        throw ex.StatusCode switch
        {
            HttpStatusCode.BadRequest => new ProviderException(ex.Message, ex, ProviderError.BadRequest),
            HttpStatusCode.InternalServerError => new ProviderException(ex.Message, ex, ProviderError.ServerError),
            _ => new ProviderException(ex.Message, ex, ProviderError.Unknown)
        };
    }
}

public class BankSimulatorRequest
{
    public required string CardNumber { get; init; }
    public required string ExpiryDate { get; init; }
    public required string Currency { get; init; }
    public required int Amount { get; init; }
    public required string Cvv { get; init; }
}

public class BankSimulatorResponse
{
    [JsonPropertyName("authorized")]
    public required bool Authorized { get; init; }

    [JsonPropertyName("authorization_code")]
    public required string AuthorizationCode { get; init; }
}

public class ProviderException : Exception
{
    public ProviderError ProviderError { get; }

    public ProviderException(string message) : base(message)
    {
    }

    public ProviderException()
    {
    }

    public ProviderException(string message, Exception innerException, ProviderError providerError) : base(message,
        innerException)
    {
        ProviderError = providerError;
    }

    public ProviderException(string message, ProviderError providerError) : base(message)
    {
        ProviderError = providerError;
    }


    public ProviderException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public enum ProviderError
{
    Unknown,
    NotFound,
    BadRequest,
    ServerError,
    TooManyRequests,
    InvalidData,
    Timeout
}
