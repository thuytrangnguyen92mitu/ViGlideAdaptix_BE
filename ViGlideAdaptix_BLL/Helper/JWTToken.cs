using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ViGlideAdaptix_DAL.Models;

namespace ViGlideAdaptix_BLL.Helper
{
	public class JWTToken
	{
		private readonly IConfiguration _configuration;
		public JWTToken(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		public string GenerateJWTToken(Customer cus)
		{
			var jwtKey = _configuration["JWT:Secret"];
			var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? ""));
			var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

			var claims = new[]
			{
				new Claim(ClaimTypes.NameIdentifier, cus.CustomerId.ToString().Trim()),
				new Claim(ClaimTypes.Role, cus.Role.RoleName.Trim()),
				new Claim(ClaimTypes.Name, cus.CustomerName.Trim())
			};

			var token = new JwtSecurityToken(
				issuer: _configuration["JWT:ValidIssuer"],
				audience: _configuration["JWT:ValidAudience"],
				claims: claims,
				expires: DateTime.Now.AddMonths(1),
				signingCredentials: signingCredentials
				);

			var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
			return accessToken;
		}


	}
}
