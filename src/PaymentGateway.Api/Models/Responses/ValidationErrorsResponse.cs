using System.Collections.ObjectModel;

namespace PaymentGateway.Api.Models.Responses;


public record ValidationErrorResponse(
    string Type,
    string Title,
    int Status,
    ReadOnlyCollection<ValidationError> Errors
);

public record ValidationError(
    string PropertyName, 
    string ErrorMessage
);