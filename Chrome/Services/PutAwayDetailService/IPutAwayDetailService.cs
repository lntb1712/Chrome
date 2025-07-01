using Chrome.DTO;
using Chrome.DTO.PutAwayDetailDTO;
using Chrome.Models;

namespace Chrome.Services.PutAwayDetailService
{
    public interface IPutAwayDetailService
    {
        Task<ServiceResponse<PagedResponse<PutAwayDetailResponseDTO>>> GetAllPutAwayDetailsAsync(string[] warehouseCodes, int page = 1, int pageSize = 10);
        Task<ServiceResponse<PagedResponse<PutAwayDetailResponseDTO>>> GetPutAwayDetailsByPutawayCodeAsync(string putawayCode, int page = 1, int pageSize = 10);
        Task<ServiceResponse<PagedResponse<PutAwayDetailResponseDTO>>> SearchPutAwayDetailsAsync(string[] warehouseCodes, string putawayCode, string textToSearch, int page = 1, int pageSize = 10);
        Task<ServiceResponse<bool>> UpdatePutAwayDetail(PutAwayDetailRequestDTO putAwayDetail);
        Task<ServiceResponse<bool>> DeletePutAwayDetail(string putawayCode, string productCode);
    }
}