using Chrome.DTO;
using Chrome.DTO.ProductCustomerDTO;
using Chrome.DTO.ProductSupplierDTO;

namespace Chrome.Services.ProductCustomerService
{
    public interface IProductCustomerService
    {
        Task<ServiceResponse<PagedResponse<ProductCustomerResponseDTO>>> GetAllProductCustomer(string productCode, int page, int pageSize);
        Task<ServiceResponse<bool>> AddProductCustomer(ProductCustomerRequestDTO productCustomer);
        Task<ServiceResponse<bool>> DeleteProductCustomer(string productCode, string customerCode);
        Task<ServiceResponse<bool>> UpdateProductCustomer(ProductCustomerRequestDTO productCustomer);
    }
}
