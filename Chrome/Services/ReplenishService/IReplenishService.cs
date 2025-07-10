using Chrome.DTO;
using Chrome.DTO.ProductMasterDTO;
using Chrome.DTO.ReplenishDTO;

namespace Chrome.Services.ReplenishService
{
    public interface IReplenishService
    {
        Task<ServiceResponse<PagedResponse<ReplenishResponseDTO>>> GetAllReplenishAsync(string warehouseCode, int page, int pageSize);
        Task<ServiceResponse<ReplenishResponseDTO>> GetReplenishByCodeAsync(string productCode, string warehouseCode);
        Task<ServiceResponse<PagedResponse<ReplenishResponseDTO>>> SearchReplenishAsync(string warehouseCode, string textToSearch, int page, int pageSize);
        Task<ServiceResponse<bool>> AddReplenishAsync(ReplenishRequestDTO replenishRequestDTO);
        Task<ServiceResponse<bool>> DeleteReplenishAsync(string productCode, string warehouseCode);
        Task<ServiceResponse<bool>> UpdateReplenishAsync(ReplenishRequestDTO replenishRequestDTO);
        Task<ServiceResponse<int>> GetTotalReplenishCountAsync(string warehouseCode); 
        Task<ServiceResponse<List<ProductMasterResponseDTO>>> GetListProductToReplenish();
        Task<ServiceResponse<List<string>>> CheckReplenishWarningsAsync(string warehouseCode);
    }
}
