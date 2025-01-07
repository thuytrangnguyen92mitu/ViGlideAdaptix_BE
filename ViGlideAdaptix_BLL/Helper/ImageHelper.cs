using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViGlideAdaptix_BLL.Helper
{
	public class ImageHelper
	{
		public string? ConvertImageToBase64(string? imagePath)
		{
			if (string.IsNullOrEmpty(imagePath)) return null;

			var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", imagePath);

			if (!File.Exists(fullPath)) return null;

			var imageBytes = File.ReadAllBytes(fullPath);
			return Convert.ToBase64String(imageBytes);
		}
	}
}
