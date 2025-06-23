using Chrome.DTO;
using Chrome.DTO.MovementDetailDTO;
using Chrome.DTO.ProductMasterDTO;
using System.Threading.Tasks;

namespace Chrome.Services.MovementDetailService
{
    public interface IMovementDetailService
    {
        Task<ServiceResponse<PagedResponse<MovementDetailResponseDTO>>> GetAllMovementDetailsAsync(string[] warehouseCodes, int page = 1, int pageSize = 10);
        Task<ServiceResponse<PagedResponse<MovementDetailResponseDTO>>> GetMovementDetailsByMovementCodeAsync(string movementCode, int page = 1, int pageSize = 10);
        Task<ServiceResponse<PagedResponse<MovementDetailResponseDTO>>> SearchMovementDetailsAsync(string[] warehouseCodes, string movementCode, string textToSearch, int page = 1, int pageSize = 10);
        Task<ServiceResponse<bool>> AddMovementDetail(MovementDetailRequestDTO movementDetail);
        Task<ServiceResponse<bool>> UpdateMovementDetail(MovementDetailRequestDTO movementDetail); 
        Task<ServiceResponse<bool>> DeleteMovementDetail(string movementCode, string productCode);
        Task<ServiceResponse<ProductMasterResponseDTO>> GetProductByLocationCode (string locationCode);
    }
}
