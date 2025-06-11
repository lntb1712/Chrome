using Chrome.DTO;
using Chrome.DTO.BOMComponentDTO;

namespace Chrome.Services.BOMComponentService
{
    public interface IBOMComponentService
    {
        Task<ServiceResponse<IEnumerable<BOMNodeDTO>>> GetRecursiveBOMAsync(string topLevelBOM, string topLevelVersion);
        Task<ServiceResponse<BOMComponentResponseDTO>>GetBOMComponent(string bomCode,string componentCode,string bomVersion);
        Task<ServiceResponse<List<BOMComponentResponseDTO>>> GetAllBOMComponent(string bomCode, string bomVersion);
        Task<ServiceResponse<bool>> AddBomComponent(BOMComponentRequestDTO bomComponent);
        Task<ServiceResponse<bool>> DeleteBomComponent(string bomCode,string componentCode,string bomVersion);
        Task<ServiceResponse<bool>> UpdateBomComponent(BOMComponentRequestDTO bomComponent);
    }
}
