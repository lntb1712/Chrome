using Chrome.DTO;
using Chrome.DTO.CustomerMasterDTO;

namespace Chrome.Services.CustomerMasterService
{
    public interface ICustomerMasterService
    {
        Task<ServiceResponse<PagedResponse<CustomerMasterResponseDTO>>> GetAllCustomerMaster(int page, int pageSize);
        Task<ServiceResponse<bool>> AddCustomerMaster(CustomerMasterRequestDTO customer);
        Task<ServiceResponse<bool>> DeleteCustomerMaster(string customerCode);
        Task<ServiceResponse<bool>> UpdateCustomerMaster(CustomerMasterRequestDTO customer);
        Task<ServiceResponse<CustomerMasterResponseDTO>> GetCustomerWithCustomerCode(string customerCode);
        Task<ServiceResponse<PagedResponse<CustomerMasterResponseDTO>>> SearchCustomer(string textToSearch, int page, int pageSize);
        Task<ServiceResponse<int>> GetTotalCustomerCount();
    }
}
