using Chrome.DTO;
using Chrome.DTO.InventoryDTO;

namespace Chrome.Services.InventoryService
{
    public interface IInventoryService
    {
        Task<ServiceResponse<PagedResponse<InventorySummaryDTO>>> GetListProductInventory(string[] warehouseCodes, int page, int pageSize);
        Task<ServiceResponse<PagedResponse<ProductWithLocationsDTO>>> GetProductWithLocationsAsync(string[] warehouseCodes,string productCode, int page, int pageSize);
        Task<ServiceResponse<PagedResponse<InventorySummaryDTO>>> GetListProductInventoryByCategoryIds(string[] warehouseCodes, string [] categoryIds, int page , int pageSize);
        Task<ServiceResponse<PagedResponse<InventorySummaryDTO>>> SearchProductInventoryAsync(string[] warehouseCodes, string textToSearch, int page, int pageSize);
        Task<ServiceResponse<bool>> AddInventory(InventoryRequestDTO inventoryRequestDTO, bool saveChanges = true);
        Task<ServiceResponse<bool>> DeleteInventoryAsync(string warehouseCode, string locationCode, string productCode, string lotNo);
        Task<ServiceResponse<bool>> UpdateInventoryAsync(InventoryRequestDTO inventoryRequestDTO , bool saveChanges = true);
    }
}
