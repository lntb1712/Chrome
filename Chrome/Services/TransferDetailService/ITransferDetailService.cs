using Chrome.DTO;
using Chrome.DTO.InventoryDTO;
using Chrome.DTO.ProductMasterDTO;
using Chrome.DTO.TransferDetailDTO;

namespace Chrome.Services.TransferDetailService
{
    public interface ITransferDetailService
    {
        Task<ServiceResponse<PagedResponse<TransferDetailResponseDTO>>> GetAllTransferDetailsAsync(string[] warehouseCodes, int page = 1, int pageSize = 10);
        Task<ServiceResponse<PagedResponse<TransferDetailResponseDTO>>> GetTransferDetailsByTransferCodeAsync(string transferCode, int page = 1, int pageSize = 10);
        Task<ServiceResponse<PagedResponse<TransferDetailResponseDTO>>> SearchTransferDetailsAsync(string[] warehouseCodes, string transferCode, string textToSearch, int page = 1, int pageSize = 10);
        Task<ServiceResponse<bool>> AddTransferDetail(TransferDetailRequestDTO transferDetail);
        Task<ServiceResponse<bool>> UpdateTransferDetail(TransferDetailRequestDTO transferDetail);
        Task<ServiceResponse<bool>> DeleteTransferDetail(string transferCode, string productCode);
        Task<ServiceResponse<List<InventorySummaryDTO>>> GetProductByWarehouseCode(string warehouseCode);
    }
}
