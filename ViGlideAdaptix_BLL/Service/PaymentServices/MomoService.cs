using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ViGlideAdaptix_BLL.DTO.PaymentDTOs;
using static System.Net.Mime.MediaTypeNames;

namespace ViGlideAdaptix_BLL.Service.PaymentServices
{
    public class MomoService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        public MomoService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> InitiatePayment(PaymentRequestDto paymentRequest)
        {
            var endpoint = _configuration["MoMo:Endpoint"];
            var partnerCode = _configuration["MoMo:PartnerCode"];
            var accessKey = _configuration["MoMo:AccessKey"];
            var secretKey = _configuration["MoMo:SecretKey"];
            var redirectUrl = _configuration["Momo:RedirectUrl"];
            var ipnUrl = _configuration["Momo:IpnUrl"];

            var requestId = Guid.NewGuid().ToString();
            var orderId = Guid.NewGuid().ToString();
            var requestType = "payWithATM";

            var rawHash = $"accessKey={accessKey}&amount={paymentRequest.Amount}&extraData={paymentRequest.ExtraData}&ipnUrl={ipnUrl}&orderId={orderId}&orderInfo={paymentRequest.OrderInfo}&partnerCode={partnerCode}&redirectUrl={redirectUrl}&requestId={requestId}&requestType={requestType}";
            var signature = GenerateSignature(rawHash, secretKey);

            var payload = new
            {
                partnerCode,
                partnerName = "Test",
                storeId = "ViGlideAdaptix",
                requestType,
                ipnUrl,
                redirectUrl,
                orderId,
                amount = paymentRequest.Amount,
                lang = "vi",
                orderInfo = paymentRequest.OrderInfo,
                requestId,
                extraData = paymentRequest.ExtraData,
                signature
            };

            using var client = new HttpClient();
            var response = await client.PostAsJsonAsync(endpoint, payload);

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Content: {responseContent}");

            //if (!responseContent.IsSuccessStatusCode) return null;

            var responseData = await response.Content.ReadFromJsonAsync<MoMoResponseDto>();
            return responseData?.PayUrl;
        }

        public Task<bool> ProcessCallback(PaymentCallbackDto callbackData)
        {
            throw new NotImplementedException();
        }

        private object GenerateSignature(string rawHash, string? secretKey)
        {
            // change according to your needs, an UTF8Encoding
            // could be more suitable in certain situations
            ASCIIEncoding encoding = new ASCIIEncoding();

            Byte[] textBytes = encoding.GetBytes(rawHash);
            Byte[] keyBytes = encoding.GetBytes(secretKey);

            Byte[] hashBytes;

            using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        //public async Task<bool> ProcessCallback(PaymentCallbackDto callbackData)
        //{
        //    var signature = GenerateSignature(callbackData.RawHash, _configuration["MoMo:SecretKey"]);
        //    if (signature != callbackData.Signature) return false;

        //    // Log or update transaction status in the database here
        //    return true;
        //}
    }
}
