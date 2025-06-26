using Chrome.DTO;
using Chrome.DTO.AccountManagementDTO;
using Chrome.DTO.BOMMasterDTO;
using Chrome.DTO.ManufacturingOrderDTO;
using Chrome.DTO.OrderTypeDTO;
using Chrome.DTO.ProductMasterDTO;
using Chrome.DTO.StatusMasterDTO;
using Chrome.DTO.WarehouseMasterDTO;
using Chrome.Models;
using System.Threading.Tasks;

namespace Chrome.Services.ManufacturingOrderService
{
    public interface IManufacturingOrderService
    {
        Task<ServiceResponse<PagedResponse<ManufacturingOrderResponseDTO>>> GetAllManufacturingOrdersAsync(string[] warehouseCodes, int page, int pageSize);
        Task<ServiceResponse<PagedResponse<ManufacturingOrderResponseDTO>>> GetAllManufacturingOrdersWithStatusAsync(string[] warehouseCodes, int statusId, int page, int pageSize);
        Task<ServiceResponse<ManufacturingOrderResponseDTO>> GetManufacturingOrderByCodeAsync(string manufacturingCode);
        Task<ServiceResponse<PagedResponse<ManufacturingOrderResponseDTO>>> SearchManufacturingOrdersAsync(string[] warehouseCodes, string textToSearch, int page, int pageSize);
        Task<ServiceResponse<bool>> AddManufacturingOrderAsync(ManufacturingOrderRequestDTO manufacturingOrder);
        Task<ServiceResponse<bool>> UpdateManufacturingOrderAsync(ManufacturingOrderRequestDTO manufacturingOrder);
        Task<ServiceResponse<bool>> DeleteManufacturingOrderAsync(string manufacturingCode);
        Task<ServiceResponse<bool>> ConfirmManufacturingOrder(string manufacturingCode);
        Task<ServiceResponse<bool>> CreateBackOrder(string manufacturingCode);
        Task<ServiceResponse<bool>> CheckAndUpdateBackOrderStatus(string manufacturingCode);
        Task<ServiceResponse<List<OrderTypeResponseDTO>>> GetListOrderTypeAsync(string prefix);
        Task<ServiceResponse<List<AccountManagementResponseDTO>>> GetListResponsibleAsync();
        Task<ServiceResponse<List<StatusMasterResponseDTO>>> GetListStatusMasterAsync();
        Task<ServiceResponse<List<ProductMasterResponseDTO>>> GetListProductMasterIsFGAndSFG();
        Task<ServiceResponse<List<WarehouseMasterResponseDTO>>> GetListWarehousePermissionAsync(string[] warehouseCodes);
        Task<ServiceResponse<BOMMasterResponseDTO>> GetListBomMasterAsync(string productCode);
    }
}