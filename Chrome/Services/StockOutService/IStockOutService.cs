using Chrome.DTO;
using Chrome.DTO.AccountManagementDTO;
using Chrome.DTO.CustomerMasterDTO;
using Chrome.DTO.OrderTypeDTO;
using Chrome.DTO.ReservationDTO;
using Chrome.DTO.StatusMasterDTO;
using Chrome.DTO.StockOutDTO;
using Chrome.DTO.WarehouseMasterDTO;

namespace Chrome.Services.StockOutService
{
    public interface IStockOutService
    {
        Task<ServiceResponse<PagedResponse<StockOutResponseDTO>>> GetAllStockOuts(string[] warehouseCodes, int page, int pageSize);
        Task<ServiceResponse<PagedResponse<StockOutResponseDTO>>> GetAllStockOutsWithStatus(string[] warehouseCodes, int statusId, int page, int pageSize);
        Task<ServiceResponse<PagedResponse<StockOutResponseDTO>>> SearchStockOutAsync(string[] warehouseCodes, string textToSearch, int page, int pageSize);
        Task<ServiceResponse<bool>> AddStockOut(StockOutRequestDTO stockOut);
        Task<ServiceResponse<bool>> DeleteStockOutAsync(string stockOutCode);
        Task<ServiceResponse<bool>> UpdateStockOut(StockOutRequestDTO stockOut);
        Task<ServiceResponse<List<OrderTypeResponseDTO>>> GetListOrderType(string prefix);
        Task<ServiceResponse<List<CustomerMasterResponseDTO>>> GetListCustomerMasterAsync();
        Task<ServiceResponse<List<AccountManagementResponseDTO>>> GetListResponsibleAsync();
        Task<ServiceResponse<List<StatusMasterResponseDTO>>> GetListStatusMaster();
        Task<ServiceResponse<List<WarehouseMasterResponseDTO>>> GetListWarehousePermission(string[] warehouseCodes);
    }
}
