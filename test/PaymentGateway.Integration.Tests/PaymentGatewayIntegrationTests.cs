using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using AutoFixture;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Infrastructure.ExternalServices.AcquiringBanks.BankSimulator.Models;
using PaymentGateway.Integration.Tests.TestSetup;

using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace PaymentGateway.Integration.Tests;

public class PaymentGatewayIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private readonly Fixture _fixture;
    
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    public PaymentGatewayIntegrationTests(CustomWebApplicationFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        
        _fixture = new Fixture();
        _factory = factory;
        _client = factory.CreateClient();
        _factory.WireMockServer.Reset();
    }
    
    [Fact]
    public async Task Returns404IfPaymentNotFound()
    {
        // Arrange
        var uri = new Uri($"/api/Payments/{Guid.NewGuid()}");
        
        // Act
        var response = await _client.GetAsync(uri);
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Returns201IfCreatePaymentRequestIsSuccessful()
    {
        // Arrange
        var mockedBankApiResponse = _fixture.Build<BankSimulatorResponse>()
            .With(response => response.Authorized, true)
            .With(response => response.AuthorizationCode, "12334")
            .Create();
        
        _factory.WireMockServer
            .Given(
                Request.Create()
                    .WithPath("/payments")
                    .UsingPost())
            .RespondWith(
                Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(JsonSerializer.Serialize(mockedBankApiResponse)));
        
        var testApiRequest = new PostPaymentRequest
        {
            CardNumber = "2222405343248123",
            ExpiryMonth = "04",
            ExpiryYear = "2025",
            Currency = "GBP",
            Amount = 100,
            Cvv = "123"
        };
        
        var body = JsonSerializer.Serialize(testApiRequest, _jsonSerializerOptions);
        
        // Act
        using var content = new StringContent(body, Encoding.UTF8, "application/json");
        var createPaymentResponse = await _client.PostAsync(new Uri("/api/Payments"), content);
        var responseContent = await createPaymentResponse.Content.ReadAsStringAsync();
        var actualResponse = JsonSerializer.Deserialize<PostPaymentResponse>(responseContent, _jsonSerializerOptions);
        var newPaymentId = actualResponse!.Id;
        var getPaymentResponse = await _client.GetAsync(new Uri($"/api/Payments/{newPaymentId}"));

        // Assert
        createPaymentResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        getPaymentResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        actualResponse.Status.Should().Be("Authorized");
        actualResponse.CardNumberLastFour.Should().Be("8123");
        actualResponse.Amount.Should().Be(100);
        actualResponse.Currency.Should().Be("GBP");
        actualResponse.ExpiryMonth.Should().Be("04");
        actualResponse.ExpiryYear.Should().Be("2025");
    }
}
