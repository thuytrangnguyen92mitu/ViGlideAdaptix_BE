﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViGlideAdaptix_BLL.DTO;

namespace ViGlideAdaptix_BLL.Service.ProductService
{
    public interface IProductServices
    {
        Task<PagedResult<AllProductResponseDTO>> GetAllProductAsync(QueryObjectDTO queryObject);
        Task<AllProductResponseDTO?> GetProductDetail(int productId);
        Task<List<AllProductResponseDTO?>> GetNewArrivalProducts();
        Task<List<AllProductResponseDTO?>> GetBestSellerProducts();
        Task<(bool, string)> ModCreateProduct(CreateProductResquestDTO request);
        Task<List<CategoryResponseDTO>?> GetCateList();
        Task<string?> GetCateName(int categoryId);
        Task<List<BrandResponseDTO>?> GetBrandList();
        Task<string?> GetBrandName(int brandId);
    }
}
