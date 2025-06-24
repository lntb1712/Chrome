using Chrome.DTO;
using Chrome.DTO.StockTakeDetailDTO;

namespace Chrome.Services.StockTakeDetailService
{
    public interface IStockTakeDetailService
    {
        Task<ServiceResponse<PagedResponse<StockTakeDetailResponseDTO>>> GetAllStockTakeDetailsAsync(string[] warehouseCodes, int page = 1, int pageSize = 10);
        Task<ServiceResponse<PagedResponse<StockTakeDetailResponseDTO>>> GetStockTakeDetailsByStockTakeCodeAsync(string StockTakeCode, int page = 1, int pageSize = 10);
        Task<ServiceResponse<PagedResponse<StockTakeDetailResponseDTO>>> SearchStockTakeDetailsAsync(string[] warehouseCodes, string StockTakeCode, string textToSearch, int page = 1, int pageSize = 10);
        Task<ServiceResponse<bool>> UpdateStockTakeDetail(StockTakeDetailRequestDTO StockTakeDetail);
        Task<ServiceResponse<bool>> DeleteStockTakeDetail(string StockTakeCode, string productCode, string lotNo, string locationCode);
    }
}
