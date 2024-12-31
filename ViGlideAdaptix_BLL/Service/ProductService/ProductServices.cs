using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViGlideAdaptix_BLL.DTO;
using ViGlideAdaptix_BLL.Helper;
using ViGlideAdaptix_DAL.Models;
using ViGlideAdaptix_DAL.Repository;

namespace ViGlideAdaptix_BLL.Service.ProductService
{
	public class ProductServices : IProductServices
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ImageHelper _imageHelper;
		public ProductServices(IUnitOfWork unitOfWork, ImageHelper base64Encoding)
		{
			_unitOfWork = unitOfWork;
			_imageHelper = base64Encoding;
		}

		/// <summary>
		/// Get all Product with search + sort + paging
		/// QueryObjectDTO
		/// Return PagedResult with AllProductResponseDTO
		/// </summary>
		public async Task<PagedResult<AllProductResponseDTO>> GetAllProductAsync(QueryObjectDTO queryObject)
		{
			//Set default pageSize and pageNumber of not inputed
			int pageSize = queryObject.PageSize > 0 ? queryObject.PageSize : 16;
			int pageNumber = queryObject.PageNumber > 0 ? queryObject.PageNumber : 1;

			//Get all products include category
			var products = (await _unitOfWork.ProductRepository.GetAllWithIncludeAsync(p => p.Category)).AsQueryable();

			//Search with product name
			if (!string.IsNullOrEmpty(queryObject.ProductName))
			{
				products = products.Where(p => p.ProductName != null &&
											   p.ProductName.Contains(queryObject.ProductName, StringComparison.OrdinalIgnoreCase));
			}

			//Search with category
			if (queryObject.CategoryId > 0)
			{
				products = products.Where(p => p.CategoryId == queryObject.CategoryId);
			}

			//Search with brand
			if (queryObject.BrandId > 0)
			{
				products = products.Where(p => p.BrandId == queryObject.BrandId);
			}

			//Count total record
			int totalRecords = products.Count();

			//Paging list of product
			var pagedProducts = new List<AllProductResponseDTO>();

			foreach (var p in products.Skip((pageNumber - 1) * pageSize).Take(pageSize))
			{
				var ratings = await GetAllRatingOfProduct(p.ProductId);
				double averageRating = (ratings == null || ratings.Count() == 0 )
										? 0 
										: ratings.Average();

				pagedProducts.Add(new AllProductResponseDTO
				{
					ProductId = p.ProductId,
					ProductName = p.ProductName,
					ProductDescription = p.ProductDescription,
					ProductImage = _imageHelper.ConvertImageToBase64(p.ProductImage),
					CategoryId = p.CategoryId,
					BrandId = p.BrandId,
					Purchases = p.Purchases,
					Quantity = p.Quantity,
					RatingScore = averageRating,
					UnitPrice = p.UnitPrice,
				});
			}

			return new PagedResult<AllProductResponseDTO>
			{
				Data = pagedProducts,
				TotalRecords = totalRecords,
				PageSize = pageSize,
				PageNumber = pageNumber,
			};
		}

		/// <summary>
		/// Get detail of Product 
		/// productId
		/// Return AllProductResponseDTO
		/// </summary>
		public async Task<AllProductResponseDTO?> GetProductDetail(int productId)
		{
			var foundProduct = await _unitOfWork.ProductRepository.GetByIdWithIncludeAsync(productId, "ProductId", p => p.Category);

			if (foundProduct != null)
			{
				var ratings = await GetAllRatingOfProduct(foundProduct.ProductId);
				double averageRating = (ratings == null || ratings.Count() == 0)
										? 0
										: ratings.Average();

				var convertedProduct = new AllProductResponseDTO
				{
					ProductId = foundProduct.ProductId,
					ProductName = foundProduct.ProductName,
					ProductDescription = foundProduct.ProductDescription,
					ProductImage = _imageHelper.ConvertImageToBase64(foundProduct.ProductImage),
					CategoryId = foundProduct.CategoryId,
					BrandId = foundProduct.BrandId,
					Purchases = foundProduct.Purchases,
					Quantity = foundProduct.Quantity,
					RatingScore = averageRating,
					UnitPrice = foundProduct.UnitPrice,
				};
				return convertedProduct;
			}
			return null;
		}

		/// <summary>
		/// Create product (Mod)
		/// CreateProductResquestDTO
		/// Return success + message
		/// </summary>
		public async Task<(bool, string)> ModCreateProduct(CreateProductResquestDTO request)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();

				if (string.IsNullOrEmpty(request.ProductName))
					return (false, "Invalid input. Required product name");

				if (request.UnitPrice < 0)
					return (false, "Invalid input. UnitPrice must be higher than 0");

				if (request.CategoryId <= 0)
					return (false, "Invalid input. Invalid categoryId");

				if (request.Quantity < 0)
					return (false, "Invalid input. Invalid quantity");

				if (request.BrandId <= 0)
					return (false, "Invalid input. Invalid brandId");

				string? imageFileName = null;
				if (!string.IsNullOrEmpty(request.ProductImage))
				{
					try
					{
						imageFileName = _imageHelper.SaveImage(request.ProductImage);
					}
					catch (Exception ex)
					{
						return (false, $"Failed to save image. Error: {ex.Message}");
					}
				}
				var newProduct = new Product()
				{
					ProductName = request.ProductName,
					ProductImage = imageFileName,
					ProductDescription = request.ProductDescription,
					UnitPrice = request.UnitPrice,
					CategoryId = request.CategoryId,
					Quantity = request.Quantity,
					BrandId = request.BrandId,
					Status = 1
				};

				await _unitOfWork.ProductRepository.AddAsync(newProduct);
				await _unitOfWork.SaveChangesAsync();

				await _unitOfWork.CommitTransactionAsync();
				return (true, "Create product successfully");
			}
			catch (Exception ex)
			{
				await _unitOfWork.RollbackTransactionAsync();
				throw new ApplicationException("Error create product", ex);
			}

		}

		/// <summary>
		/// Help to get all rating score of product
		/// Return List of rating score
		/// </summary>
		private async Task<List<int>?> GetAllRatingOfProduct(int productId)
		{
			var foundProduct = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
			if (foundProduct != null)
			{
				var ratingList = await _unitOfWork.RatingRepository.GetAllAsync();
				var scoreList = ratingList.Where(x => x.ProductId == foundProduct.ProductId).Select(x => x.Score).ToList();

				return scoreList;
			}
			return null;
		}
	}
}
