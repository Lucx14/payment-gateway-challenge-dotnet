using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using FluentValidation;

using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Validators;

public class PostPaymentRequestValidator : AbstractValidator<PostPaymentRequest>
{
    private static readonly HashSet<string> SupportedCurrencies = ["GBP", "USD", "EUR"];
    
    public PostPaymentRequestValidator()
    {
        RuleFor(request => request.CardNumber)
            .NotNull().WithMessage("Card number is required")
            .NotEmpty().WithMessage("Card number cannot be empty")
            .Must(BeValidCardNumber).WithMessage("Card number must be between 14 and 19 numeric characters");
        
        RuleFor(request => request.ExpiryMonth)
            .NotNull().WithMessage("Expiry month is required")
            .NotEmpty().WithMessage("Expiry month cannot be empty")
            .Must(BeValidMonth).WithMessage("Expiry month must be between 1 and 12");
        
        RuleFor(request => request.ExpiryYear)
            .NotNull().WithMessage("Expiry year is required")
            .NotEmpty().WithMessage("Expiry year cannot be empty")
            .Must(BeValidYear).WithMessage("Expiry year must be a valid year");
        
        RuleFor(request => request)
            .Must(BeValidFutureExpiryDate).WithMessage("Card expiry date must be in the future");
        
        RuleFor(request => request.Cvv)
            .NotNull()
            .Length(3, 4);
        
        RuleFor(request => request.Currency)
            .NotNull().WithMessage("Currency is required")
            .NotEmpty().WithMessage("Currency cannot be empty")
            .Length(3).WithMessage("Currency must be 3 characters long")
            .Must(BeValidCurrency).WithMessage("Currency name must be GBP, USD, EUR");
        
        RuleFor(request => request.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be a positive integer");
    }
    
    private static bool BeValidCardNumber(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return false;
        }

        return cardNumber.Length is > 14 and < 19 && cardNumber.All(char.IsDigit);
    }
    
    private static bool BeValidMonth(string expiryMonth)
    {
        if (!int.TryParse(expiryMonth, out int month))
        {
            return false;
        }

        return month is >= 1 and <= 12;
    }
    
    private static bool BeValidYear(string expiryYear)
    {
        if (!int.TryParse(expiryYear, out int year))
        {
            return false;
        }

        return year >= DateTime.UtcNow.Year;
    }
    
    private static bool BeValidFutureExpiryDate(PostPaymentRequest request)
    {
        if (!int.TryParse(request.ExpiryMonth, out int month) || !int.TryParse(request.ExpiryYear, out int year))
        {
            return false;
        }

        var expiryDate = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);
        
        return expiryDate > DateTime.UtcNow;
    }
    
    private static bool BeValidCurrency(string currency)
    {
        return SupportedCurrencies.Contains(currency.ToUpper(CultureInfo.InvariantCulture));
    }
}
