using System.Text.Json.Serialization;

namespace PaymentGateway.Infrastructure.ExternalServices.AcquiringBanks.BankSimulator.Models;

public class BankSimulatorResponse
{
    [JsonPropertyName("authorized")]
    public required bool Authorized { get; init; }

    [JsonPropertyName("authorization_code")]
    public required string AuthorizationCode { get; init; }
}