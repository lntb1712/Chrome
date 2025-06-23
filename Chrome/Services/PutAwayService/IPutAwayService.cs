using Chrome.DTO;
using Chrome.DTO.PutAwayDTO;
using Microsoft.EntityFrameworkCore.Storage;

namespace Chrome.Services.PutAwayService
{
    public interface IPutAwayService
    {
        Task<ServiceResponse<PagedResponse<PutAwayResponseDTO>>> GetAllPutAwaysAsync(string[] warehouseCodes, int page = 1, int pageSize = 10);
        Task<ServiceResponse<PagedResponse<PutAwayResponseDTO>>> GetAllPutAwaysWithStatusAsync(string[] warehouseCodes, int statusId, int page = 1, int pageSize = 10);
        Task<ServiceResponse<PagedResponse<PutAwayResponseDTO>>> SearchPutAwaysAsync(string[] warehouseCodes, string textToSearch, int page = 1, int pageSize = 10);
        Task<ServiceResponse<PutAwayResponseDTO>> GetPutAwayByCodeAsync(string putAwayCode);
        Task<ServiceResponse<bool>> AddPutAway(PutAwayRequestDTO putAway, IDbContextTransaction transaction = null!);
        Task<ServiceResponse<bool>> DeletePutAway(string putAwayCode);
        Task<ServiceResponse<bool>> UpdatePutAway(PutAwayRequestDTO putAway);
    }
}
