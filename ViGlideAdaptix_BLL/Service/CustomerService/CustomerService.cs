using Microsoft.Extensions.Configuration;
using ViGlideAdaptix_BLL.DTO;
using ViGlideAdaptix_BLL.Helper;
using ViGlideAdaptix_DAL.Models;
using ViGlideAdaptix_DAL.Repository;

namespace ViGlideAdaptix_BLL.Service.CustomerService
{
    public class CustomerService : ICustomerService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly JWTToken _generateJWTToken;
        public CustomerService(IUnitOfWork unitOfWork, IConfiguration configuration, JWTToken generateJWTToken)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _generateJWTToken = generateJWTToken;
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
                var customerList = await _unitOfWork.CustomerRepository.GetAllWithIncludeAsync(c => ((Customer)c).Role,
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
            try
            {
                await _unitOfWork.BeginTransactionAsync();

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
                    await _unitOfWork.CartRepository.AddAsync(customerCart);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();

                }
                return customerCart;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new ApplicationException("Error find cart of customer", ex);
            }



        }

        /// <summary>
        /// Use to Register new Customer account
        /// Return success and message
        /// </summary>
        public async Task<(bool, string)> RegisterCustomerAccount(RegisterRequestDTO request)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

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
                await _unitOfWork.CustomerRepository.AddAsync(requestCus);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return (true, "Register successfully!");
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return (false, "Register failed!");
            }
        }

        /// <summary>
        /// Check duplicated Customer by Email
        /// Return true <> false
        /// </summary>
        private async Task<bool> CheckDuplicatedCustomer(string email)
        {
            var customerList = await _unitOfWork.CustomerRepository.GetAllAsync();
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
                await _unitOfWork.BeginTransactionAsync();

                //Find match customer with ID
                var foundCustomer = await _unitOfWork.CustomerRepository.GetByIdAsync(request.CustomerId);
                if (foundCustomer == null)
                    return (false, "Not found any customer with that ID!");

                //Update customer profile
                foundCustomer.CustomerName = request.CustomerName.Trim();
                foundCustomer.Password = HashSha256.ComputeSha256(request.Password + _configuration["SPComputeKey:Key"]);
                foundCustomer.Address = request.Address;
                foundCustomer.PhoneNumber = request.PhoneNumber;

                _unitOfWork.CustomerRepository.Update(foundCustomer);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return (true, "Update successfully");
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return (false, "Update failed");
            }
        }

        /// <summary>
        /// GetCustomerInformation
        /// CustomerId
        /// Return CustomerInforResponseDTO
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public async Task<CustomerInforResponseDTO?> GetCustomerInformation(int customerId)
        {
            try
            {
                var foundCustomer = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId);
                if (foundCustomer != null)
                {
                    var responseCustomer = new CustomerInforResponseDTO
                    {
                        CustomerName = foundCustomer.CustomerName.Trim(),
                        Address = foundCustomer.Address.Trim(),
                        Email = foundCustomer.Email.Trim(),
                        Password = foundCustomer.Password.Trim(),
                        PhoneNumber = foundCustomer.PhoneNumber.Trim(),
                    };
                    return responseCustomer;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
