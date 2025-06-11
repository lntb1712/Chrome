using Chrome.DTO;
using Chrome.DTO.ProductMasterDTO;

namespace Chrome.Services.ProductMasterService
{
    public interface IProductMasterService
    {
        Task<ServiceResponse<PagedResponse<ProductMasterResponseDTO>>> GetAllProductMaster(int page, int pageSize);
        Task<ServiceResponse<bool>> AddProductMaster(ProductMasterRequestDTO product);
        Task<ServiceResponse<bool>> DeleteProductMaster(string productCode);
        Task<ServiceResponse<bool>> UpdateProductMaster(ProductMasterRequestDTO product);
        Task<ServiceResponse<ProductMasterResponseDTO>> GetProductMasterWithProductCode(string productCode);
        Task<ServiceResponse<PagedResponse<ProductMasterResponseDTO>>> GetAllProductWithCategoryID(string categoryId, int page, int pageSize);
        Task<ServiceResponse<PagedResponse<ProductMasterResponseDTO>>> SearchProduct(string textToSearch, int page, int pageSize);
        Task<ServiceResponse<int>>GetTotalProductCount();
        Task<ServiceResponse<List<ProductMasterResponseDTO>>> GetProductWithCategoryIds(string[] categoryIds);

    }
}
