using Chrome.DTO;
using Chrome.DTO.ProductSupplierDTO;

namespace Chrome.Services.ProductSupplierSerivce
{
    public interface IProductSupplierService
    {
        Task<ServiceResponse<PagedResponse<ProductSupplierResponseDTO>>> GetAllProductSupplier(string productCode,int page, int pageSize);
        Task<ServiceResponse<bool>> AddProductSupplier(ProductSupplierRequestDTO productSupplierRequestDTO);
        Task<ServiceResponse<bool>> DeleteProductSupplier(string productCode, string supplierCode);
        Task<ServiceResponse<bool>> UpdateProductSupplier(ProductSupplierRequestDTO productSupplierRequestDTO);
    }
}
