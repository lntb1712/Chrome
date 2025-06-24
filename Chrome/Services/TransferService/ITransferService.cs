using Chrome.DTO;
using Chrome.DTO.AccountManagementDTO;
using Chrome.DTO.OrderTypeDTO;
using Chrome.DTO.StatusMasterDTO;
using Chrome.DTO.TransferDTO;
using Chrome.DTO.WarehouseMasterDTO;

namespace Chrome.Services.TransferService
{
    public interface ITransferService
    {
        Task<ServiceResponse<PagedResponse<TransferResponseDTO>>> GetAllTransfers(string[] warehouseCodes, int page, int pageSize);
        Task<ServiceResponse<PagedResponse<TransferResponseDTO>>> GetAllTransfersWithStatus(string[] warehouseCodes, int statusId, int page, int pageSize);
        Task<ServiceResponse<PagedResponse<TransferResponseDTO>>> SearchTransfersAsync(string[] warehouseCodes, string textToSearch, int page, int pageSize);
        Task<ServiceResponse<bool>> AddTransfer(TransferRequestDTO transfer);
        Task<ServiceResponse<bool>> DeleteTransferAsync(string transferCode);
        Task<ServiceResponse<bool>> UpdateTransfer(TransferRequestDTO transfer);
        Task<ServiceResponse<List<OrderTypeResponseDTO>>> GetListOrderType(string prefix);
        Task<ServiceResponse<List<AccountManagementResponseDTO>>> GetListFromResponsibleAsync(string warehouseCode);
        Task<ServiceResponse<List<AccountManagementResponseDTO>>> GetListToResponsibleAsync(string warehouseCode);
        Task<ServiceResponse<List<StatusMasterResponseDTO>>> GetListStatusMaster();
        Task<ServiceResponse<List<WarehouseMasterResponseDTO>>> GetListWarehousePermission(string[] warehouseCodes);
    }
}
