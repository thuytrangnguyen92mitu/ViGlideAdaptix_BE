using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViGlideAdaptix_BLL.DTO.PaymentDTOs
{
    public class MoMoResponseDto
    {
        public string PayUrl { get; set; } // URL for redirecting the user to complete payment
        public string ErrorCode { get; set; } // Error code, if any
        public string Message { get; set; } // Message from MoMo API
        public string RequestId { get; set; } // ID of the request
        public string OrderId { get; set; } // Your internal order ID
    }
}
