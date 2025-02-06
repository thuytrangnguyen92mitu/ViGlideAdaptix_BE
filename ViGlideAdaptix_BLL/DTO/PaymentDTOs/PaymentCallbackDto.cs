using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViGlideAdaptix_BLL.DTO.PaymentDTOs
{
    public class PaymentCallbackDto
    {
        // Common fields
        public string PartnerCode { get; set; }      // Partner code (MoMo) or merchant code (VNPay)
        public string OrderId { get; set; }          // Unique order ID from your system
        public string RequestId { get; set; }        // Unique request ID (MoMo-specific)
        public long Amount { get; set; }             // Transaction amount
        public int ResultCode { get; set; }          // Result code or status (e.g., 0 for success)
        public string Message { get; set; }          // Description of the result or status
        public string PayType { get; set; }          // Payment method (e.g., MoMo Wallet, VNPay)
        public string ExtraData { get; set; }        // Optional additional data
        public string Signature { get; set; }        // Hash for verifying the callback's authenticity
        public string TransactionId { get; set; }    // Transaction ID (used by both systems)
        public string ResponseTime { get; set; }     // Timestamp of transaction processing (if provided)

        // VNPay-specific fields
        public string BankCode { get; set; }         // Bank code (specific to VNPay)
        public string CardType { get; set; }         // Card type (specific to VNPay)

        // MoMo-specific fields
        public string TransId { get; set; }          // MoMo transaction ID (can overlap with TransactionId)
    }
}
