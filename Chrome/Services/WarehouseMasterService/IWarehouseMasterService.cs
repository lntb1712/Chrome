using Chrome.DTO;
using Chrome.DTO.WarehouseMasterDTO;

namespace Chrome.Services.WarehouseMasterService
{
    public interface IWarehouseMasterService
    {
        Task<ServiceResponse<PagedResponse<WarehouseMasterResponseDTO>>>GetAllWarehouseMaster(int page, int pageSize);
        Task<ServiceResponse<bool>> AddWarehouseMaster(WarehouseMasterRequestDTO warehouse);
        Task<ServiceResponse<bool>> DeleteWarehouseMaster(string warehouseCode);
        Task<ServiceResponse<bool>> UpdateWarehouseMaster(WarehouseMasterRequestDTO warehouse);
        Task<ServiceResponse<WarehouseMasterResponseDTO>> GetWarehouseMasterWithCode(string warehouseCode);
        Task<ServiceResponse<PagedResponse<WarehouseMasterResponseDTO>>> SearchWarehouse(string textToSearch, int page, int pageSize);
        Task<ServiceResponse<int>> GetTotalWarehouseCount();

    }
}
