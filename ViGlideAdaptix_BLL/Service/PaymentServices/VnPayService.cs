using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ViGlideAdaptix_BLL.DTO.PaymentDTOs;
using ViGlideAdaptix_DAL.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ViGlideAdaptix_BLL.Service.PaymentServices
{
    public class VnPayService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        public VnPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> InitiatePayment(PaymentRequestDto paymentRequest)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();

            var tmnCode = _configuration["VNPay:TmnCode"];
            var hashSecret = _configuration["VNPay:HashSecret"];
            var url = _configuration["VNPay:PaymentUrl"];
            var command = _configuration["VNPay:Command"];
            var currCode = _configuration["VNPay:CurrCode"];
            var version = _configuration["VNPay:Version"];
            var locale = _configuration["VNPay:Locale"];
            var returnUrl = _configuration["VNPay:ReturnUrl"];

            var timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");

            var vnpParams = new SortedDictionary<string, string>
            {
                { "vnp_Version", version },
                { "vnp_Command", command },
                { "vnp_TmnCode", tmnCode },
                { "vnp_Amount", ((long)(paymentRequest.Amount * 100)).ToString() }, // Convert to VND
                { "vnp_BankCode", "VNBANK" },
                { "vnp_CreateDate", timeStamp },
                { "vnp_CurrCode", currCode },
                { "vnp_IpAddr", paymentRequest.ClientIp },
                { "vnp_Locale", locale },
                { "vnp_OrderInfo", paymentRequest.OrderInfo },
                { "vnp_OrderType", "other" },
                { "vnp_ReturnUrl", returnUrl },
                { "vnp_TxnRef", tick },
            };

            // Build the raw data for signature with URL encoding on both keys and values.
            var rawData = string.Join("&", vnpParams
                .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

            // Do NOT remove the last character as string.Join does not add a trailing '&'
            var secureHash = HmacSha512(hashSecret, rawData);


            // Build the query string for the URL (keys typically don’t need encoding if they are known constants)
            var queryString = string.Join("&", vnpParams
                .Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
            var paymentUrl = $"{url}?{queryString}&vnp_SecureHash={secureHash}";


            return await Task.FromResult(paymentUrl);
        }

        public Task<bool> ProcessCallback(PaymentCallbackDto callbackData)
        {
            throw new NotImplementedException();
        }

        private string HmacSha512(string key, string inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }

    }
}
