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

			var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "ViGlideAdaptix_BLL", "Images", imagePath);

			if (!File.Exists(fullPath)) return null;

			var imageBytes = File.ReadAllBytes(fullPath);
			return Convert.ToBase64String(imageBytes);
		}
		public string SaveImage(string? base64Image)
		{
			if (string.IsNullOrEmpty(base64Image))
				throw new ArgumentException("Image cannot be null or empty.");

			var imageBytes = Convert.FromBase64String(base64Image);

			var fileName = $"{Guid.NewGuid()}.jpg";
			var savePath = Path.Combine(Directory.GetCurrentDirectory(), "ViGlideAdaptix_BLL", "Images", fileName);
			var path = Path.GetDirectoryName(savePath);
			if(path != null)
			{
				Directory.CreateDirectory(path);
			}

			File.WriteAllBytes(savePath, imageBytes);
			return fileName;
		}
	}
}
