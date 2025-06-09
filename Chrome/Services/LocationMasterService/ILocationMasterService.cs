using Chrome.DTO;
using Chrome.DTO.LocationMasterDTO;

namespace Chrome.Services.LocationMasterService
{
    public interface ILocationMasterService
    {
        Task<ServiceResponse<PagedResponse<LocationMasterResponseDTO>>> GetAllLocationMaster(string warehouseCode,int page, int pageSize);
        Task<ServiceResponse<LocationMasterResponseDTO>> GetLocationMasterWithCode(string warehouseCode,string locationCode);
        Task<ServiceResponse<int>> GetTotalLocationMasterCount(string warehouseCode);
        Task<ServiceResponse<bool>> AddLocationMaster(LocationMasterRequestDTO locationMasterRequestDTO);
        Task<ServiceResponse<bool>> DeleteLocationMaster(string warehouseCode, string locationCode);
        Task<ServiceResponse<bool>> UpdateLocationMaster(LocationMasterRequestDTO locationMasterRequestDTO);
    }
}
