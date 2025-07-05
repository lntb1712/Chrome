using Chrome.DTO;
using Chrome.DTO.AccountManagementDTO;
using Chrome.DTO.OrderTypeDTO;
using Chrome.DTO.StatusMasterDTO;
using Chrome.DTO.StockInDTO;
using Chrome.DTO.SupplierMasterDTO;
using Chrome.DTO.WarehouseMasterDTO;
using Chrome.Models;

namespace Chrome.Services.StockInService
{
    public interface IStockInService
    {
        Task<ServiceResponse<PagedResponse<StockInResponseDTO>>> GetAllStockIns(string[] warehouseCodes, int page, int pageSize);
        Task<ServiceResponse<StockInResponseDTO>> GetStockInByCode(string stockInCode);
        Task<ServiceResponse<PagedResponse<StockInResponseDTO>>> GetAllStockInWithResponsible(string[] warehouseCodes, string responsible, int page, int pageSize);
        Task<ServiceResponse<PagedResponse<StockInResponseDTO>>> GetAllStockInsWithStatus(string[] warehouseCodes,int statusId, int page, int pageSize);
        Task<ServiceResponse<PagedResponse<StockInResponseDTO>>> SearchStockInAsync(string[] warehouseCodes, string textToSearch ,int page, int pageSize);
        Task<ServiceResponse<PagedResponse<StockInResponseDTO>>> SearchStockInWithResponsible(string[] warehouseCodes, string responsible, string textToSearch, int page, int pageSize);
        Task<ServiceResponse<bool>> AddStockIn(StockInRequestDTO stockIn);
        Task<ServiceResponse<bool>> DeleteStockInAsync(string stockInCode);
        Task<ServiceResponse<bool>> UpdateStockIn(StockInRequestDTO stockIn);
        Task<ServiceResponse<List<OrderTypeResponseDTO>>> GetListOrderType(string prefix);
        Task<ServiceResponse<List<SupplierMasterResponseDTO>>>GetListSupplierMasterAsync();
        Task<ServiceResponse<List<AccountManagementResponseDTO>>> GetListResponsibleAsync(string warehouseCode);
        Task<ServiceResponse<List<StatusMasterResponseDTO>>> GetListStatusMaster();
        Task<ServiceResponse<List<WarehouseMasterResponseDTO>>> GetListWarehousePermission(string[] warehouseCodes);
    }
}
