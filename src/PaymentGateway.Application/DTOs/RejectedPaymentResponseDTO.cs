namespace PaymentGateway.Application.DTOs;

public class RejectedPaymentResponseDTO
{
    public string Status { get; set; }
    public string Errors { get; set; }
}