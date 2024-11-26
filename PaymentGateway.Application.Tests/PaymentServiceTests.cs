using FluentAssertions;

using Moq;

using PaymentGateway.Application.Extensions;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Application.Models.Requests;
using PaymentGateway.Application.Models.Responses;
using PaymentGateway.Application.Services;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Enums;
using PaymentGateway.Domain.Repositories;

namespace PaymentGateway.Application.Tests;

public class PaymentServiceTests
{
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
    private readonly Mock<IAcquiringBankApiClient> _acquiringBankApiClientMock;

    private readonly PaymentService _sut;

    public PaymentServiceTests()
    {
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _acquiringBankApiClientMock = new Mock<IAcquiringBankApiClient>();
        
        _sut = new PaymentService(_paymentRepositoryMock.Object, _acquiringBankApiClientMock.Object);
    }
    
    [Fact]
    public async Task GetPaymentByIdAsyncWhenPaymentIsNotFoundReturnsNull()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        
        _paymentRepositoryMock
            .Setup(x => x.GetById(It.IsAny<Guid>()))
            .Returns(null as Payment);
        
        // Act
        var response = await _sut.GetPaymentByIdAsync(paymentId, CancellationToken.None);
        
        // Assert
        response.Should().BeNull();
    }
    
    [Fact]
    public async Task GetPaymentByIdAsyncWhenPaymentExistsReturnsPaymentDto()
    {
        // Arrange
        var paymentId = Guid.NewGuid();

        var existingPayment = new Payment
        {
            Id = paymentId,
            Amount = 100,
            CardNumberLastFour = "1234",
            Currency = "GBP",
            ExpiryMonth = "12",
            ExpiryYear = "2026",
            Status = PaymentStatus.Initiated,
        };

        var expectedResponse = existingPayment.ToPaymentDto();
        
        _paymentRepositoryMock
            .Setup(x => x.GetById(It.IsAny<Guid>()))
            .Returns(existingPayment);
        
        // Act
        var response = await _sut.GetPaymentByIdAsync(paymentId, CancellationToken.None);
        
        // Assert
        response.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task CreatePaymentAsyncWhenSuccessfulReturnsPaymentDto()
    {
        var paymentInitiation = new InitiatePaymentRequest
        {
            Amount = 100,
            Currency = "GBP",
            CardNumber = "1234",
            ExpiryMonth = "12",
            ExpiryYear = "2026",
            Cvv = "123"
        };

        var dummyPayment = new Payment
        {
            Id = Guid.NewGuid(),
            Amount = 100,
            CardNumberLastFour = "1234",
            Currency = "GBP",
            ExpiryMonth = "12",
            ExpiryYear = "2026",
            Status = PaymentStatus.Initiated,
        };

        var dummyBankSimResponse = new CreatePaymentResponse() { Authorized = true, AuthorizationCode = "1234" };
        
        _paymentRepositoryMock
            .Setup(x => x.CreatePayment(It.IsAny<Payment>()))
            .Returns(dummyPayment);
        
        _acquiringBankApiClientMock
            .Setup(x => x.CreatePaymentAsync(It.IsAny<CreatePaymentRequest>(), CancellationToken.None))
            .ReturnsAsync(dummyBankSimResponse);
        
        // Act
        var response = await _sut.CreatePaymentAsync(paymentInitiation, CancellationToken.None);
        
        // Assert
        response.Should().BeEquivalentTo(dummyPayment.ToPaymentDto());
    }
}