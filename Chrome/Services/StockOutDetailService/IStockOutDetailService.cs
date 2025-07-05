using Chrome.DTO;
using Chrome.DTO.ProductMasterDTO;
using Chrome.DTO.StockOutDetailDTO;

namespace Chrome.Services.StockOutDetailService
{
    public interface IStockOutDetailService
    {
        Task<ServiceResponse<PagedResponse<StockOutDetailResponseDTO>>> GetAllStockOutDetails(string stockOutCode, int page, int pageSize);
        Task<ServiceResponse<bool>> AddStockOutDetail(StockOutDetailRequestDTO stockOutDetail);
        Task<ServiceResponse<bool>> UpdateStockOutDetail(StockOutDetailRequestDTO stockOutDetail);
        Task<ServiceResponse<bool>> DeleteStockOutDetail(string stockOutCode, string productCode);
        Task<ServiceResponse<bool>> ConfirmStockOut(string stockOutCode);
        Task<ServiceResponse<bool>> CreateBackOrder(string stockOutCode, string backOrderDescription);
        Task<ServiceResponse<bool>> CheckAndUpdateBackOrderStatus(string stockOutCode);
        Task<ServiceResponse<List<ProductMasterResponseDTO>>> GetListProductToSO();
        Task<ServiceResponse<ForecastStockOutDetailDTO>> GetForecastStockOutDetail(string stockOutCode, string productCode);
    }
}
