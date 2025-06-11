using Chrome.DTO;
using Chrome.DTO.BOMMasterDTO;

namespace Chrome.Services.BOMMasterService
{
    public interface IBOMMasterService
    {
        Task<ServiceResponse<PagedResponse<BOMMasterResponseDTO>>> GetAllBOMMaster(int page, int pageSize);
        Task<ServiceResponse<BOMMasterResponseDTO>> GetBOMMasterByCode(string bomCode, string bomVersion);
        Task<ServiceResponse<int>> GetTotalBOMMasterCount();
        Task<ServiceResponse<PagedResponse<BOMMasterResponseDTO>>> SearchBOMMaster(string textToSearch, int page, int pageSize);
        Task<ServiceResponse<bool>> AddBOMMaster(BOMMasterRequestDTO bomMasterRequestDTO);
        Task<ServiceResponse<bool>> UpdateBOMMaster(BOMMasterRequestDTO bomMasterRequestDTO);
        Task<ServiceResponse<bool>> DeleteBOMMaster(string bomCode, string bomVersion);
    }
}
