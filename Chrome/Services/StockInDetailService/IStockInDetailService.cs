using Chrome.DTO;
using Chrome.DTO.ProductMasterDTO;
using Chrome.DTO.StockInDetailDTO;

namespace Chrome.Services.StockInDetailService
{
    public interface IStockInDetailService
    {
        Task<ServiceResponse<PagedResponse<StockInDetailResponseDTO>>> GetAllStockInDetails(string stockInCode,int page, int pageSize);
        Task<ServiceResponse<bool>> AddStockInDetail(StockInDetailRequestDTO stockInDetail);
        Task<ServiceResponse<bool>> UpdateStockInDetail(StockInDetailRequestDTO stockInDetail);
        Task<ServiceResponse<bool>> DeleteStockInDetail(string stockInCode,string productCode);
        Task<ServiceResponse<bool>> ConfirmStockIn(string stockInCode);
        Task<ServiceResponse<bool>> CreateBackOrder(string stockInCode, string backOrderDescription, string dateBackOrder);
        Task<ServiceResponse<bool>> CheckAndUpdateBackOrderStatus(string stockInCode);
        Task<ServiceResponse<List<ProductMasterResponseDTO>>> GetListProductToSI();
    }
}
