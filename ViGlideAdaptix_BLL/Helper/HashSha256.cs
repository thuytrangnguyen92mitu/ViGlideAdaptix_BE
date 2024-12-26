using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ViGlideAdaptix_BLL.Helper
{
	public class HashSha256
	{
		public static string ComputeSha256(string input)
		{
			using (SHA256 sha256 = SHA256.Create())
			{
				// Compute the hash as a byte array
				byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

				// Convert the byte array to a hexadecimal string
				StringBuilder builder = new StringBuilder();
				for (int i = 0; i < bytes.Length; i++)
				{
					builder.Append(bytes[i].ToString("x2"));
				}
				return builder.ToString();
			}
		}
	}
}
