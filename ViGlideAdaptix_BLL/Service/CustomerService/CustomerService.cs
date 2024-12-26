using Microsoft.Extensions.Configuration;
using ViGlideAdaptix_BLL.DTO;
using ViGlideAdaptix_BLL.Helper;
using ViGlideAdaptix_DAL.Models;
using ViGlideAdaptix_DAL.Repository;

namespace ViGlideAdaptix_BLL.Service.CustomerService
{
	public class CustomerService : ICustomerService
	{
		private readonly IGenericRepository<Customer> _customerRepo;
		private readonly IGenericRepository<ShoppingCart> _cartRepo;

		private readonly IConfiguration _configuration;
		private readonly JWTToken _generateJWTToken;
		public CustomerService(IGenericRepository<Customer> customerRepo, IConfiguration configuration, JWTToken generateJWTToken, IGenericRepository<ShoppingCart> cartRepo)
		{
			_customerRepo = customerRepo;
			_configuration = configuration;
			_generateJWTToken = generateJWTToken;
			_cartRepo = cartRepo;
		}

		/// <summary>
		/// Use to authenticate Customer with Email and Password
		/// Return token to authentication and authorization later
		/// </summary>
		public async Task<(bool, Customer?, int, int, string?, string?)> Authenticate(LoginRequestDTO request)
		{
			try
			{
				//Check if Email or Password is null - Return 
				if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
					return (false, null, 0, 0, null, null);

				//Hash input password with SHA256 into encoded password to storage
				request.Password = HashSha256.ComputeSha256(request.Password + _configuration["SPComputeKey:Key"]);

				//Get all the list of Customer
				var customerList = await _customerRepo.GetAllWithIncludeAsync(c => ((Customer)c).Role,
																			   c => ((Customer)c).ShoppingCarts.Where(
																									c => c.IsActive == true));

				//Found the customer with email and password and not banned from the system
				var foundCustomer = customerList.FirstOrDefault(x => x.Email.Trim() == request.Email.Trim() &&
																	 x.Password.Trim() == request.Password.Trim() &&
																	 x.IsBanned == false);
				//If not found any - Return
				if (foundCustomer == null)
					return (false, null, 0, 0, null, null);

				//If found generate new access token for that user
				var token = _generateJWTToken.GenerateJWTToken(foundCustomer);
				var customerCart = await FindCartOfCustomer(foundCustomer);

				return (true, foundCustomer, foundCustomer.CustomerId, customerCart.CartId, foundCustomer.Role.RoleName.Trim(), token);
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Help to find cart of customer
		/// Create new if not have yet
		/// </summary>
		private async Task<ShoppingCart> FindCartOfCustomer(Customer customer)
		{
			var customerCart = customer.ShoppingCarts.FirstOrDefault(x => x.IsActive == true);
			if (customerCart == null)
			{
				customerCart = new ShoppingCart()
				{
					CreatedDate = DateTime.UtcNow,
					CustomerId = customer.CustomerId,
					SubTotal = 0,
					IsActive = true
				};
				await _cartRepo.AddAsync(customerCart);
				await _cartRepo.SaveChangeAsync();
			}

			return customerCart;
		}

		/// <summary>
		/// Use to Register new Customer account
		/// Return success and message
		/// </summary>
		public async Task<(bool, string)> RegisterCustomerAccount(RegisterRequestDTO request)
		{
			try
			{
				//Check duplicated by email
				var isDuplicated = await CheckDuplicatedCustomer(request.Email);
				if (isDuplicated)
					return (false, "Duplicated email! Try again");

				//Transfer DTO into customer
				var requestCus = new Customer()
				{
					Email = request.Email,
					Password = HashSha256.ComputeSha256(request.Password + _configuration["SPComputeKey:Key"]),
					Address = request.Address,
					CustomerName = request.CustomerName,
					PhoneNumber = request.PhoneNumber,
					RoleId = 2,
					IsBanned = false
				};
				//Add new Customer
				await _customerRepo.AddAsync(requestCus);
				await _customerRepo.SaveChangeAsync();

				return (true, "Register successfully!");
			}
			catch (Exception)
			{
				return (false, "Register failed!");
			}
		}

		/// <summary>
		/// Check duplicated Customer by Email
		/// Return true <> false
		/// </summary>
		private async Task<bool> CheckDuplicatedCustomer(string email)
		{
			var customerList = await _customerRepo.GetAllAsync();
			var foundCustomer = customerList.FirstOrDefault(x => x.Email.Trim() == email);
			if (foundCustomer != null)
				return true;
			return false;
		}

		/// <summary>
		/// Use to update customer profile
		/// return success and message
		/// </summary>
		public async Task<(bool, string)> UpdateCustomerProfile(UpdateRequestDTO request)
		{
			try
			{
				//Find match customer with ID
				var foundCustomer = await _customerRepo.GetByIdAsync(request.CustomerId);
				if (foundCustomer == null)
					return (false, "Not found any customer with that ID!");

				//Update customer profile
				foundCustomer.CustomerName = request.CustomerName.Trim();
				foundCustomer.Password = HashSha256.ComputeSha256(request.Password + _configuration["SPComputeKey:Key"]);
				foundCustomer.Address = request.Address;
				foundCustomer.PhoneNumber = request.PhoneNumber;

				_customerRepo.Update(foundCustomer);
				await _customerRepo.SaveChangeAsync();
				return (true, "Update successfully");
			}
			catch (Exception)
			{
				return (false, "Update failed");
			}
		}
	}
}
