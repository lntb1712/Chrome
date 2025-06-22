using Chrome.DTO;
using Chrome.DTO.PickListDetailDTO;

namespace Chrome.Services.PickListDetailService
{
    public interface IPickListDetailService
    {
        Task<ServiceResponse<PagedResponse<PickListDetailResponseDTO>>> GetAllPickListDetailsAsync(string[] warehouseCodes, int page = 1, int pageSize = 10);
        Task<ServiceResponse<PagedResponse<PickListDetailResponseDTO>>> GetPickListDetailsByPickNoAsync(string pickNo, int page = 1, int pageSize = 10);
        Task<ServiceResponse<PagedResponse<PickListDetailResponseDTO>>> SearchPickListDetailsAsync(string[] warehouseCodes,string pickNo, string textToSearch, int page = 1, int pageSize = 10);
        Task<ServiceResponse<bool>> UpdatePickListDetail(PickListDetailRequestDTO pickListDetail);
        Task<ServiceResponse<bool>> DeletePickListDetail(string pickNo, string productCode);
    }
}
