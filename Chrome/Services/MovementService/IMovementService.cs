using Chrome.DTO;
using Chrome.DTO.AccountManagementDTO;
using Chrome.DTO.LocationMasterDTO;
using Chrome.DTO.MovementDTO;
using Chrome.DTO.OrderTypeDTO;
using Chrome.DTO.PickListDTO;
using Chrome.DTO.PutAwayDTO;
using Chrome.DTO.StatusMasterDTO;
using Chrome.DTO.WarehouseMasterDTO;

namespace Chrome.Services.MovementService
{
    public interface IMovementService
    {
        Task<ServiceResponse<PagedResponse<MovementResponseDTO>>> GetAllMovements(string[] warehouseCodes, int page, int pageSize);
        Task<ServiceResponse<PagedResponse<MovementResponseDTO>>> GetAllMovementsWithResponsible(string[] warehouseCodes,string responsible, int page, int pageSize);
        Task<ServiceResponse<PagedResponse<MovementResponseDTO>>> GetAllMovementsWithStatus(string[] warehouseCodes, int statusId, int page, int pageSize);
        Task<ServiceResponse<PagedResponse<MovementResponseDTO>>> SearchMovementAsync(string[] warehouseCodes, string textToSearch, int page, int pageSize);
        Task<ServiceResponse<PagedResponse<MovementResponseDTO>>> SearchMovementAsyncWithResponsible(string[] warehouseCodes,string responsible, string textToSearch, int page, int pageSize);
        Task<ServiceResponse<bool>> AddMovement(MovementRequestDTO movement);
        Task<ServiceResponse<bool>> DeleteMovementAsync(string movementCode);
        Task<ServiceResponse<bool>> UpdateMovement(MovementRequestDTO movement);
        Task<ServiceResponse<List<OrderTypeResponseDTO>>> GetListOrderType(string prefix);
        Task<ServiceResponse<List<AccountManagementResponseDTO>>> GetListResponsibleAsync();
        Task<ServiceResponse<List<StatusMasterResponseDTO>>> GetListStatusMaster();
        Task<ServiceResponse<List<WarehouseMasterResponseDTO>>> GetListWarehousePermission(string[] warehouseCodes);
        Task<ServiceResponse<List<LocationMasterResponseDTO>>> GetListToLocation(string warehouseCode,string fromLocation);
        Task<ServiceResponse<List<LocationMasterResponseDTO>>> GetListFromLocation(string warehouseCode);
        Task<ServiceResponse<PutAwayResponseDTO>> GetPutAwayContainsMovement(string movementCode);
        Task<ServiceResponse<PickListResponseDTO>> GetPickListContainsMovement(string movementCode);
    }
}
