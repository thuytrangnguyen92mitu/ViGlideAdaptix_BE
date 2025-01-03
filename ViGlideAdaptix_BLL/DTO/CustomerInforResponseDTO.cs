using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViGlideAdaptix_DAL.Models;

namespace ViGlideAdaptix_BLL.DTO
{
    public class CustomerInforResponseDTO
    {

        public string CustomerName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Address { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

    }
}
