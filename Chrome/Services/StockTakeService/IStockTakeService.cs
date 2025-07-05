using Chrome.DTO;
using Chrome.DTO.AccountManagementDTO;
using Chrome.DTO.InventoryDTO;
using Chrome.DTO.StatusMasterDTO;
using Chrome.DTO.StockTakeDetailDTO;
using Chrome.DTO.StockTakeDTO;
using Chrome.DTO.WarehouseMasterDTO;

namespace Chrome.Services.StockTakeService
{
    public interface IStockTakeService
    {
        Task<ServiceResponse<PagedResponse<StockTakeResponseDTO>>> GetAllStockTakesAsync(string[] warehouseCodes, int page = 1, int pageSize = 10);
        Task<ServiceResponse<PagedResponse<StockTakeResponseDTO>>> GetAllStockTakesAsyncWithResponsible(string[] warehouseCodes,string responsible, int page = 1, int pageSize = 10);
        Task<ServiceResponse<PagedResponse<StockTakeResponseDTO>>> GetStockTakesByStatusAsync(string[] warehouseCodes, int statusId, int page = 1, int pageSize = 10);
        Task<ServiceResponse<PagedResponse<StockTakeResponseDTO>>> SearchStockTakesAsync(string[] warehouseCodes, string textToSearch, int page = 1, int pageSize = 10);
        Task<ServiceResponse<PagedResponse<StockTakeResponseDTO>>> SearchStockTakesAsyncWithResponsible(string[] warehouseCodes,string responsible, string textToSearch, int page = 1, int pageSize = 10);
        Task<ServiceResponse<bool>> AddStockTake(StockTakeRequestDTO StockTake);
        Task<ServiceResponse<bool>> UpdateStockTake(StockTakeRequestDTO StockTake);
        Task<ServiceResponse<bool>> DeleteStockTakeAsync(string StockTakeCode);
        Task<ServiceResponse<bool>> ConfirmnStockTake(StockTakeRequestDTO stockTake);
        Task<ServiceResponse<List<AccountManagementResponseDTO>>> GetListResponsibleAsync(string warehouseCode);
        Task<ServiceResponse<List<StatusMasterResponseDTO>>> GetListStatusMaster();
        Task<ServiceResponse<List<WarehouseMasterResponseDTO>>> GetListWarehousePermission(string[] warehouseCodes);
    }
}
