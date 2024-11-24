using System;

using FluentAssertions;

using PaymentGateway.Domain.Entities;
using PaymentGateway.Infrastructure.Repositories;

namespace PaymentGateway.Infrastructure.Tests.Repositories;

public class InMemoryPaymentRepositoryTests
{
    private readonly InMemoryPaymentRepository _repository = new();

    [Fact]
    public void AddShouldStorePayment()
    {
        // Arrange
        var payment = new Payment { Id = Guid.NewGuid(), Currency = "GBP"};

        // Act
        _repository.Add(payment);

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