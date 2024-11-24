using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using AutoFixture;

using FluentAssertions;

using Moq;
using Moq.Protected;

using PaymentGateway.Infrastructure.ExternalServices.AcquiringBanks;

namespace PaymentGateway.Infrastructure.Tests.ExternalServices.AcquiringBanks;

public class BankSimulatorApiClientTests : IDisposable
{
    private readonly Fixture _fixture;
    private const string BaseTestUrl = "http://test";
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly BankSimulatorApiClient _sut;
    private bool _disposed;
    
    public BankSimulatorApiClientTests()
    {
        _fixture = new Fixture();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri(BaseTestUrl)
        };
        _sut = new BankSimulatorApiClient(_httpClient);
    }

    [Fact]
    public async Task CreatePaymentAsyncReturnsExpectedResult()
    {
        // Arrange
        const string expectedUrl = $"{BaseTestUrl}/payments";
        
        var mockedResponse = _fixture.Build<BankSimulatorResponse>()
            .With(response => response.Authorized, true)
            .With(response => response.AuthorizationCode, "mocked-code")
            .Create();
        
        using var httpResponse = CreateHttpResponse(HttpStatusCode.OK, JsonSerializer.Serialize(mockedResponse));
        
        var testRequest = new BankSimulatorRequest
        {
            CardNumber = "mocked-number",
            ExpiryDate = "mocked-date",
            Currency = "mocked-currency",
            Amount = 10,
            Cvv = "mocked-Cvv",
        };
        
        SetupHttpMessageHandlerMock(expectedUrl, httpResponse, HttpMethod.Post);

        // can update this later if the client returns a mapped domain response 
        var expectedClientResponse = new BankSimulatorResponse
        {
            Authorized = true, 
            AuthorizationCode = "mocked-code",
        };
        
        // Act
        var clientResponse = await _sut.CreatePaymentAsync(testRequest, CancellationToken.None);
        
        // Assert
        clientResponse.Should().BeEquivalentTo(expectedClientResponse);
    }

    [Fact]
    public async Task CreatePaymentAsyncMapsHttpRequestExceptionToProviderException()
    {
        // Arrange
        const string expectedUrl = $"{BaseTestUrl}/payments";
        
        var invalidRequest = new BankSimulatorRequest
        {
            CardNumber = "mocked-number",
            ExpiryDate = "mocked-date",
            Currency = "mocked-currency",
            Amount = 10,
            Cvv = "mocked-Cvv",
        };

        using var httpResponse = CreateHttpResponse(HttpStatusCode.BadRequest, "{\"errorMessage\": \"mocked-message\"}");
        
        SetupHttpMessageHandlerMock(expectedUrl, httpResponse, HttpMethod.Post);

        // Act
        var act = () => _sut.CreatePaymentAsync(invalidRequest, CancellationToken.None);

        // Assert
        await act
            .Should()
            .ThrowAsync<ProviderException>()
            .Where(ex => ex.ProviderError == ProviderError.BadRequest);
    }
    
    [Fact]
    public async Task CreatePaymentAsyncWhenResponseIsDelayedThrowsTimeoutException()
    {
        // Arrange
        const string expectedUrl = $"{BaseTestUrl}/payments";
        
        var validRequest = new BankSimulatorRequest
        {
            CardNumber = "mocked-number",
            ExpiryDate = "mocked-date",
            Currency = "mocked-currency",
            Amount = 10,
            Cvv = "mocked-Cvv",
        };

        using var httpResponse = CreateHttpResponse(HttpStatusCode.OK, string.Empty);
        
        SetupHttpMessageHandlerMock(expectedUrl, httpResponse, HttpMethod.Post, withDelay: true);

        _httpClient.Timeout = TimeSpan.FromMilliseconds(1);

        // Act
        var act = () => _sut.CreatePaymentAsync(validRequest, CancellationToken.None);

        // Assert
        await act
            .Should()
            .ThrowAsync<ProviderException>()
            .Where(ex => ex.ProviderError == ProviderError.Timeout);
    }

    [Fact]
    public async Task CreatePaymentAsyncWhenDeserializationFailsThrowsProviderException()
    {
        // Arrange
        const string expectedUrl = $"{BaseTestUrl}/payments";
        
        var validRequest = new BankSimulatorRequest
        {
            CardNumber = "mocked-number",
            ExpiryDate = "mocked-date",
            Currency = "mocked-currency",
            Amount = 10,
            Cvv = "mocked-Cvv",
        };

        using var httpResponse = CreateHttpResponse(HttpStatusCode.OK, "{\"invalid_response\": \"invalid_response\"}");
        
        SetupHttpMessageHandlerMock(expectedUrl, httpResponse, HttpMethod.Post);
        
        // Act
        var act = () => _sut.CreatePaymentAsync(validRequest, CancellationToken.None);

        // Assert
        await act
            .Should()
            .ThrowAsync<ProviderException>()
            .Where(ex => ex.ProviderError == ProviderError.InvalidData);
    }
    
    private static HttpResponseMessage CreateHttpResponse(HttpStatusCode statusCode, string content)
    {
        return new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = new StringContent(content)
        };
    }
    
    private void SetupHttpMessageHandlerMock(string url, HttpResponseMessage response, HttpMethod httpMethod, bool withDelay = false)
    {
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(x =>
                    x.RequestUri == new Uri(url) &&
                    x.Method == httpMethod),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                if (withDelay)
                {
                    Task.Delay(10).Wait();

                }
                return response;
            });
    }
    
    public void Dispose()
    {
        Dispose(true);
        // figure out if i need this later
        // GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _httpClient.Dispose();
        }

        _disposed = true;
    }
}