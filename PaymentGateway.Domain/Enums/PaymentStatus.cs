namespace PaymentGateway.Domain.Enums;

public enum PaymentStatus
{
    Initiated,
    Authorized,
    Declined,
    Rejected,
    Failed
}
