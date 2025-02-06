using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViGlideAdaptix_BLL.DTO.PaymentDTOs
{
    public class PaymentRequestDto
    {
        public string Method { get; set; } // "MoMo" or "VNPay"
        public decimal Amount { get; set; }
        public string OrderInfo { get; set; }
        public string ExtraData { get; set; } // Optional field for additional info
        public string ClientIp { get; set; } // IP address of the client (for VNPay)
    }
}
