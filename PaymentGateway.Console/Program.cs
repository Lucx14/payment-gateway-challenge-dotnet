// A console helper to help test out the bank client is working

using System;
using System.Net.Http;
using System.Threading;

using PaymentGateway.Infrastructure.ExternalServices.AcquiringBanks;

Console.WriteLine("Testing the bank simulator is working.....");

using var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri("http://localhost:8080");
var bankSimulatorClient = new BankSimulatorApiClient(httpClient);

var payload = new BankSimulatorRequest
{
    CardNumber = "2222405343248877",
    ExpiryDate = "04/2025",
    Currency = "GBP",
    Amount = 100,
    Cvv = "123"
};

try
{
    Console.WriteLine("Sending a request to the bank simulator...");
    var response = await bankSimulatorClient.CreatePaymentAsync(payload, CancellationToken.None).ConfigureAwait(false);
    Console.WriteLine("Request was successfully sent to the bank simulator.....");

    Console.WriteLine("-----------------");
    Console.WriteLine($"Status received: {response.Authorized}");
    Console.WriteLine($"Authorization Code: {response.AuthorizationCode}");
}
catch (ProviderException e)
{
    Console.WriteLine($"Exception was thrown: {e.Message}");
    Console.WriteLine($"Provider error: {e.ProviderError}");
}
catch
{
    Console.WriteLine("Unexpected error occured.");
    throw;
}

