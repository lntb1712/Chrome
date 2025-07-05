using Chrome.DTO;
using Chrome.DTO.ManufacturingOrderDetailDTO;

namespace Chrome.Services.ManufacturingOrderDetailService
{
    public interface IManufacturingOrderDetailService
    {
        Task<ServiceResponse<PagedResponse<ManufacturingOrderDetailResponseDTO>>> GetManufacturingOrderDetail(string manufacturingOrderCode);
        Task<ServiceResponse<ManufacturingOrderDetailResponseDTO>> GetManufacturingOrderDetail(string manufacturingOrderCode, string productCode);
        Task<ServiceResponse<ForecastManufacturingOrderDetailDTO>> GetForecastManufacturingOrderDetail(string manufacturingOrderCode, string productCode);
    }
}
