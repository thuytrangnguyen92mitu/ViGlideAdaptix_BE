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
                double averageRating = (ratings == null || ratings.Count() == 0)
                                        ? 0
                                        : ratings.Average();

                pagedProducts.Add(new AllProductResponseDTO
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductDescription = p.ProductDescription,
                    ProductImage = p.ProductImage,
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

        public async Task<List<AllProductResponseDTO?>> GetNewArrivalProducts()
        {
            var products = (await _unitOfWork.ProductRepository.GetAllWithIncludeAsync(p => p.Category))
                                                               .Where(x => x.CategoryId == 3)
                                                               .OrderByDescending(x => x.ProductId)
                                                               .Take(5)
                                                               .ToList();

            var convertedProduct = new List<AllProductResponseDTO>();

            foreach (var p in products)
            {
                var ratings = await GetAllRatingOfProduct(p.ProductId);
                double averageRating = (ratings == null || ratings.Count() == 0)
                                        ? 0
                                        : ratings.Average();

                convertedProduct.Add(new AllProductResponseDTO
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductDescription = p.ProductDescription,
                    ProductImage = p.ProductImage,
                    CategoryId = p.CategoryId,
                    BrandId = p.BrandId,
                    Purchases = p.Purchases,
                    Quantity = p.Quantity,
                    RatingScore = averageRating,
                    UnitPrice = p.UnitPrice,
                });
            }

            return convertedProduct;
        }

        public async Task<List<AllProductResponseDTO?>> GetBestSellerProducts()
        {
            var products = (await _unitOfWork.ProductRepository.GetAllWithIncludeAsync(p => p.Category))
                                                               .OrderByDescending(x => x.Purchases)
                                                               .Take(4)
                                                               .ToList();

            var convertedProduct = new List<AllProductResponseDTO>();

            foreach (var p in products)
            {
                var ratings = await GetAllRatingOfProduct(p.ProductId);
                double averageRating = (ratings == null || ratings.Count() == 0)
                                        ? 0
                                        : ratings.Average();

                convertedProduct.Add(new AllProductResponseDTO
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductDescription = p.ProductDescription,
                    ProductImage = p.ProductImage,
                    CategoryId = p.CategoryId,
                    BrandId = p.BrandId,
                    Purchases = p.Purchases,
                    Quantity = p.Quantity,
                    RatingScore = averageRating,
                    UnitPrice = p.UnitPrice,
                });
            }

            return convertedProduct;
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

                // imageFileName will now only store the file name (e.g., uniqueFileName)
                string? imageFileName = null;
                if (!string.IsNullOrEmpty(request.ProductImage))
                {
                    try
                    {
                        // No need to handle full path here, just store the image file name
                        imageFileName = request.ProductImage;  // This is the file name only, no path
                    }
                    catch (Exception ex)
                    {
                        return (false, $"Failed to save image. Error: {ex.Message}");
                    }
                }

                // Creating the product object with the file name for the image
                var newProduct = new Product()
                {
                    ProductName = request.ProductName,
                    ProductImage = imageFileName, // Store only the file name
                    ProductDescription = request.ProductDescription,
                    UnitPrice = request.UnitPrice,
                    CategoryId = request.CategoryId,
                    Quantity = request.Quantity,
                    BrandId = request.BrandId,
                    Status = 1
                };

                // Adding product to the repository and saving changes
                await _unitOfWork.ProductRepository.AddAsync(newProduct);
                await _unitOfWork.SaveChangesAsync();

                // Committing the transaction
                await _unitOfWork.CommitTransactionAsync();
                return (true, "Create product successfully");
            }
            catch (Exception ex)
            {
                // If any exception occurs, roll back the transaction
                await _unitOfWork.RollbackTransactionAsync();
                throw new ApplicationException("Error creating product", ex);
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

        public async Task<List<CategoryResponseDTO>?> GetCateList()
        {
            var listCate = await _unitOfWork.CategoryRepository.GetAllAsync();
            if (listCate == null)
                return null;

            var responseList = new List<CategoryResponseDTO>();
            foreach (var item in listCate)
            {
                var convertCate = new CategoryResponseDTO
                {
                    CategoryId = item.CategoryId,
                    CategoryName = item.CategoryName,
                };
                responseList.Add(convertCate);
            }
            return responseList;
        }
        public async Task<string?> GetCateName(int categoryId)
        {
            var foundCate = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId);
            if (foundCate != null)
                return foundCate.CategoryName;
            return null;
        }
        public async Task<List<BrandResponseDTO>?> GetBrandList()
        {
            var listBrand = await _unitOfWork.BrandRepository.GetAllAsync();
            if (listBrand == null)
                return null;

            var responseList = new List<BrandResponseDTO>();
            foreach (var item in listBrand)
            {
                var convertBrand = new BrandResponseDTO
                {
                    BrandId = item.BrandId,
                    BrandName = item.BrandName,
                };
                responseList.Add(convertBrand);
            }
            return responseList;
        }

        public async Task<string?> GetBrandName(int brandId)
        {
            var foundBrand = await _unitOfWork.BrandRepository.GetByIdAsync(brandId);
            if (foundBrand != null)
                return foundBrand.BrandName;
            return null;
        }
    }
}
