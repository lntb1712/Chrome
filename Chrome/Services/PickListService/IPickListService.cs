using Chrome.DTO;
using Chrome.DTO.PickListDTO;
using System.Threading.Tasks;

namespace Chrome.Services.PickListService
{
    public interface IPickListService
    {
        Task<ServiceResponse<PagedResponse<PickListResponseDTO>>> GetAllPickListsAsync(string[] warehouseCodes, int page = 1, int pageSize = 10);
        Task<ServiceResponse<PagedResponse<PickListResponseDTO>>> GetAllPickListsWithStatusAsync(string[] warehouseCodes, int statusId, int page = 1, int pageSize = 10);
        Task<ServiceResponse<PagedResponse<PickListResponseDTO>>> SearchPickListsAsync(string[] warehouseCodes, string textToSearch, int page = 1, int pageSize = 10);
        Task<ServiceResponse<PickListResponseDTO>> GetPickListByCodeAsync(string pickNo);
        Task<ServiceResponse<bool>> AddPickList(PickListRequestDTO pickList);
        Task<ServiceResponse<bool>> DeletePickList(string pickNo);
        Task<ServiceResponse<bool>> UpdatePickList(PickListRequestDTO pickList);
    }
}