using Chrome.DTO;
using Chrome.DTO.ProductMasterDTO;
using Chrome.DTO.PurchaseOrderDetailDTO;

namespace Chrome.Services.PurchaseOrderDetailService
{
    public interface IPurchaseOrderDetailService
    {
        Task<ServiceResponse<PagedResponse<PurchaseOrderDetailResponseDTO>>> GetAllPurchaseOrderDetails(string purchaseOrderCode, int page, int pageSize);
        Task<ServiceResponse<PurchaseOrderDetailResponseDTO>> GetPurchaseOrderDetailByCode(string purchaseOrderDetailCode,string productCode);
        Task<ServiceResponse<bool>> AddPurchaseOrderDetail(PurchaseOrderDetailRequestDTO purchaseOrderDetail);
        Task<ServiceResponse<bool>> DeletePurchaseOrderDetailAsync(string purchaseOrderDetailCode, string productCode);
        Task<ServiceResponse<bool>> UpdatePurchaseOrderDetail(PurchaseOrderDetailRequestDTO purchaseOrderDetail);
        Task<ServiceResponse<bool>> ConfirmPurchaseOrderDetail(string purchaseOrderDetailCode);
        Task<ServiceResponse<bool>> CreateBackOrder(string purchaseOrderDetailCode, string backOrderDescription, string dateBackOrder);
        Task<ServiceResponse<bool>> CheckAndUpdateBackOrderStatus(string purchaseOrderDetailCode);
        Task<ServiceResponse<List<ProductMasterResponseDTO>>> GetListProductToPO();
    }
}
