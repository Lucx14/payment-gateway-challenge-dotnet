using System;

using PaymentGateway.Application.Models.Responses;
using PaymentGateway.Infrastructure.ExternalServices.AcquiringBanks.BankSimulator.Models;

namespace PaymentGateway.Infrastructure.ExternalServices.AcquiringBanks.BankSimulator;

public static class BankSimulatorResponseExtensions
{
    public static CreatePaymentResponse ToCreatePaymentResponse(this BankSimulatorResponse bankSimulatorResponse)
    {
        ArgumentNullException.ThrowIfNull(bankSimulatorResponse);

        return new CreatePaymentResponse
        {
            Authorized = bankSimulatorResponse.Authorized,
            AuthorizationCode = bankSimulatorResponse.AuthorizationCode,
        };
    }
}