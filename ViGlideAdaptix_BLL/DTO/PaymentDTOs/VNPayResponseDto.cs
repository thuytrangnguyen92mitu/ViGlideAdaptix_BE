using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViGlideAdaptix_BLL.DTO.PaymentDTOs
{
    public class VNPayResponseDto
    {
        public string TransactionNo { get; set; } // Transaction number from VNPay
        public string BankCode { get; set; } // Bank code of the transaction
        public decimal Amount { get; set; } // Transaction amount
        public string ResponseCode { get; set; } // VNPay's response code (00 = Success)
        public string Message { get; set; } // Optional message
    }
}
