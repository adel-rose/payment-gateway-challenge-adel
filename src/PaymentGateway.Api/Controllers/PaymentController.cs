using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Application.DTOs;
using PaymentGateway.Application.Extensions;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Api.Controllers
{
    [ApiController]
    [Route("api/paymentgateway")]
    public class PaymentController : ControllerBase
    {
        private readonly Serilog.ILogger _logger;
        private readonly IPaymentService _paymentService;
        private readonly ICardService _cardService;
        
        public PaymentController(Serilog.ILogger logger, IPaymentService paymentService, ICardService cardService)
        {
            _logger = logger;
            _paymentService = paymentService;
            _cardService = cardService;
        }

        //[Post]/payments
        [HttpPost("processpayment")]
        public async Task<IActionResult> ProcessCardPayment(PaymentRequestDTO paymentRequestDto, CancellationToken cancellationToken = default)
        {
            // Validation of model/DTO happens after successful model binding automatically
            
            var paymentResponse = await _paymentService.ProcessCardPayment(paymentRequestDto, cancellationToken);
            
            if (paymentResponse.Status == PaymentStatus.Declined)
                _logger.Warning($"Payment declined by issuing bank");
            
            var cardDetails = await _cardService.SaveCardDetailsAsync(paymentRequestDto, paymentResponse, cancellationToken);
            
            await _paymentService.SavePaymentDetails(cardDetails, paymentRequestDto, paymentResponse, cancellationToken);
            
            return Ok(paymentResponse.ToPaymentResponseDto());
            
        }

        // [Get]/payments/{id}
        [HttpGet("payment/{id}")]
        public async Task<IActionResult> FindPayment(Guid id, CancellationToken cancellationToken = default)
        {
            // Validation of model/DTO happens after successful model binding automatically
            
             var payment = await _paymentService.RetrievePayment(id, cancellationToken);
             
             return Ok(payment.ToPaymentResponseDto());
        }
        
    }
}
