using ViGlideAdaptix_BLL.DTO;
using ViGlideAdaptix_DAL.Models;

namespace ViGlideAdaptix_BLL.Service.CustomerService
{
	public interface ICustomerService
	{
		Task<(bool, Customer?, int, int, string?, string?)> Authenticate(LoginRequestDTO request);
		Task<(bool, string)> RegisterCustomerAccount(RegisterRequestDTO request);
		Task<(bool, string)> UpdateCustomerProfile(UpdateRequestDTO request);
	}
}
