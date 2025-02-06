using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ViGlideAdaptix_BLL.DTO.PaymentDTOs;
using ViGlideAdaptix_BLL.Service.PaymentServices;

namespace ViGlideAdaptix_API.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly MomoService _momoService;
        private readonly VnPayService _vnPayService;

        public PaymentController(MomoService momoService, VnPayService vnPayService)
        {
            _momoService = momoService;
            _vnPayService = vnPayService;

        }

        [HttpPost("momo/create")]
        public async Task<IActionResult> CreateMomoPayment([FromBody] PaymentRequestDto paymentRequest)
        {
            var paymentUrl = await _momoService.InitiatePayment(paymentRequest);
            return Ok(new { PaymentUrl = paymentUrl });
        }

        [HttpPost("momo/payment-callback")]
        public async Task<IActionResult> HandleMomoCallback([FromBody] PaymentCallbackDto callbackData)
        {
            var success = await _momoService.ProcessCallback(callbackData);
            return success ? Ok() : BadRequest(new { Message = "Invalid callback" });
        }


        [HttpPost("vnpay/create")]
        public async Task<IActionResult> CreateVnPayPayment([FromBody] PaymentRequestDto paymentRequest)
        {
            var paymentUrl = await _vnPayService.InitiatePayment(paymentRequest);
            return Ok(new { PaymentUrl = paymentUrl });
        }

        [HttpPost("vnpay/payment-callback")]
        public async Task<IActionResult> HandleVnPayCallback([FromBody] PaymentCallbackDto callbackData)
        {
            var success = await _vnPayService.ProcessCallback(callbackData);
            return success ? Ok() : BadRequest(new { Message = "Invalid callback" });
        }
    }
}


//{
//    "method": "momo",
//  "amount": 100000,
//  "orderInfo": "Order123 from ViGlideAdaptix",
//  "extraData": "order from ViGlideAdaptix shop",
//  "clientIp": ""
//}



//{
//    "PartnerCode": "MOMO4MUD20240115_TEST",
//  "OrderId": "ORDER12345",
//  "RequestId": "REQ12345",
//  "Amount": 100000,
//  "ResultCode": 0,
//  "Message": "Success",
//  "PayType": "MoMo Wallet",
//  "ExtraData": "Test",
//  "Signature": "abc123",
//  "TransactionId": "TRANS12345",
//  "TransId": "TRANS12345",
//  "ResponseTime": "2025-01-27T15:00:00"
//}

//https://viglide-adaptix.netlify.app
// 14.191.223.183