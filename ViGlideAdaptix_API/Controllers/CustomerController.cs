using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ViGlideAdaptix_BLL.DTO;
using ViGlideAdaptix_BLL.Service.CustomerService;

namespace ViGlideAdaptix_API.Controllers
{
    [Route("api/customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Login with email and password
        /// LoginRequestDTO
        /// Return token, customerId, Role
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> LoginWithEmailAndPassword([FromBody] LoginRequestDTO request)
        {
            // Check required fields
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return BadRequest();

            //Authenticate request Customer
            var (isAuthenticated, foundCustomer, customerId, cartId, roleName, token) = await _customerService.Authenticate(request);

            if (!isAuthenticated)
                return Unauthorized();
            return Ok(new
            {
                Token = token,
                CustomerId = customerId,
                CartId = cartId,
                Role = roleName
            });
        }

        [HttpPost("profile/{customerId}")]
        public async Task<IActionResult> GetCustomerInfo(int customerId)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var foundCustomer = await _customerService.GetCustomerInformation(customerId);
            if (foundCustomer != null)
                return Ok(foundCustomer);

            return BadRequest();

        }
        /// <summary>
        /// Register new Customer account
        /// RegisterRequestDTO
        /// Return status + message
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAccount([FromBody] RegisterRequestDTO request)
        {
            //Check required fields
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password) ||
               string.IsNullOrEmpty(request.CustomerName) || string.IsNullOrEmpty(request.ConfirmPassword) ||
               string.IsNullOrEmpty(request.Address) || string.IsNullOrEmpty(request.PhoneNumber))
                return BadRequest();

            //Check matched between password and confirm password
            if (request.Password != request.ConfirmPassword)
                return BadRequest("Not matched confirm password! Try again");

            //Register new Customer account
            var (isRegister, mess) = await _customerService.RegisterCustomerAccount(request);

            if (isRegister)
                return Ok(mess);

            return BadRequest(mess);
        }

        /// <summary>
        /// Update Customer profile
        /// UpdateRequestDTO
        /// Return status + message
        /// </summary>
        [HttpPost("profile")]
        [Authorize(Roles = "customer")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateRequestDTO request)
        {
            //Check required fields
            if (string.IsNullOrEmpty(request.Address) || string.IsNullOrEmpty(request.PhoneNumber) ||
               string.IsNullOrEmpty(request.CustomerName) || string.IsNullOrEmpty(request.Password))
                return BadRequest();

            //Update Customer account
            var (isUpdated, mess) = await _customerService.UpdateCustomerProfile(request);
            if (isUpdated)
                return Ok(mess);

            return BadRequest(mess);
        }
    }
}
