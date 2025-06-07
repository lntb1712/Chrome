using Chrome.DTO;
using Chrome.DTO.GroupFunctionDTO;

namespace Chrome.Services.GroupFunctionService
{
    public interface IGroupFunctionService
    {
        Task<ServiceResponse<List<GroupFunctionResponseDTO>>> GetGroupFunctionWithGroupID(string groupId);
        Task<ServiceResponse<bool>> DeleteGroupFunction(string groupId,string functionId);
        Task<ServiceResponse<List<ApplicableLocationResponseDTO>>> GetListApplicableSelected();
    }
}
