using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Domain.Tests;

public class PaymentTests
{
    [Fact]
    public void UpdateStatusChangesStatusSuccessfully()
    {
        // Arrange
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            Status = PaymentStatus.Initiated,
            CardNumberLastFour = "8877",
            ExpiryMonth = "04",
            ExpiryYear = "2025",
            Currency = "GBP",
            Amount = 100
        };

        const PaymentStatus newStatus = PaymentStatus.Authorized;

        // Act
        payment.UpdateStatus(newStatus);

        // Assert
        Assert.Equal(newStatus, payment.Status);
    }
}