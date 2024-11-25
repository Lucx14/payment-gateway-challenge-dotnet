using System;

using FluentAssertions;

using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Enums;
using PaymentGateway.Infrastructure.Repositories;

namespace PaymentGateway.Infrastructure.Tests.Repositories;

public class InMemoryPaymentRepositoryTests
{
    private readonly InMemoryPaymentRepository _repository = new();

    [Fact]
    public void AddShouldStorePayment()
    {
        // Arrange
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            Status = PaymentStatus.Authorized,
            Amount = 100,
            ExpiryMonth = "12",
            ExpiryYear = "2021",
            CardNumberLastFour = "1233",
            Currency = "GBP"
        };

        // Act
        _repository.CreatePayment(payment);

        // Assert
        var retrievedPayment = _repository.GetById(payment.Id);
        retrievedPayment.Should().NotBeNull();
        retrievedPayment!.Currency.Should().Be(payment.Currency);
    }

    [Fact]
    public void GetByIdWhenPaymentDoesNotExistShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = _repository.GetById(nonExistentId);

        // Assert
        result.Should().BeNull();
    }
}