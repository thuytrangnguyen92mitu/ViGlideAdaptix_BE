using System;
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
		Task<(bool, string)> ModCreateProduct(CreateProductResquestDTO request);
	}
}
