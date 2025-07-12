using Chrome.DTO;
using Chrome.DTO.PurchaseOrderDTO;
using Chrome.DTO.StatusMasterDTO;
using Chrome.DTO.SupplierMasterDTO;
using Chrome.DTO.WarehouseMasterDTO;

namespace Chrome.Services.PurchaseOrderService
{
    public interface IPurchaseOrderService
    {
        Task<ServiceResponse<PagedResponse<PurchaseOrderResponseDTO>>> GetAllPurchaseOrders(string[] warehouseCodes, int page, int pageSize);
        Task<ServiceResponse<PurchaseOrderResponseDTO>> GetPurchaseOrderByCode(string purchaseOrderCode);
        Task<ServiceResponse<PagedResponse<PurchaseOrderResponseDTO>>> GetAllPurchaseOrdersWithStatus(string[] warehouseCodes, int statusId, int page, int pageSize);
        Task<ServiceResponse<PagedResponse<PurchaseOrderResponseDTO>>> SearchPurchaseOrderAsync(string[] warehouseCodes, string textToSearch, int page, int pageSize);
        Task<ServiceResponse<bool>> AddPurchaseOrder(PurchaseOrderRequestDTO purchaseOrder);
        Task<ServiceResponse<bool>> DeletePurchaseOrderAsync(string purchaseOrderCode);
        Task<ServiceResponse<bool>> UpdatePurchaseOrder(PurchaseOrderRequestDTO purchaseOrder);
        Task<ServiceResponse<List<StatusMasterResponseDTO>>> GetListStatusMaster();
        Task<ServiceResponse<List<SupplierMasterResponseDTO>>> GetListSupplierMasterAsync();
        Task<ServiceResponse<List<WarehouseMasterResponseDTO>>> GetListWarehousePermission(string[] warehouseCodes);
    }
}
