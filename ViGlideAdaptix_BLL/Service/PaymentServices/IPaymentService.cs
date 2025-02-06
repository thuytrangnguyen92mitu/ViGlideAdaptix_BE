using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViGlideAdaptix_BLL.DTO.PaymentDTOs;

namespace ViGlideAdaptix_BLL.Service.PaymentServices
{
    public interface IPaymentService
    {
        Task<string> InitiatePayment(PaymentRequestDto paymentRequest);
        Task<bool> ProcessCallback(PaymentCallbackDto callbackData);
    }
}
