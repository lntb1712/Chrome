using Chrome.DTO;
using Chrome.DTO.SupplierMasterDTO;

namespace Chrome.Services.SupplierMasterService
{
    public interface ISupplierMasterService
    {
        Task<ServiceResponse<PagedResponse<SupplierMasterResponseDTO>>>GetAllSupplierMaster(int page, int pageSize);
        Task<ServiceResponse<bool>> AddSupplierMaster(SupplierMasterRequestDTO supplier);
        Task<ServiceResponse<bool>> DeleteSupplierMaster(string supplierCode);
        Task<ServiceResponse<bool>> UpdateSupplierMaster(SupplierMasterRequestDTO supplier);
        Task<ServiceResponse<SupplierMasterResponseDTO>> GetSupplierWithSupplierCode(string supplierCode);
        Task<ServiceResponse<PagedResponse<SupplierMasterResponseDTO>>> SearchSupplier(string textToSearch,int page, int pageSize);
        Task<ServiceResponse<int>> GetTotalSupplierCount();
     }
}
