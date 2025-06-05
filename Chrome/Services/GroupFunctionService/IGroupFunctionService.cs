using Chrome.DTO;
using Chrome.DTO.FunctionDTO;
using Chrome.DTO.GroupFunctionDTO;

namespace Chrome.Services.GroupFunctionService
{
    public interface IGroupFunctionService
    {
        Task<ServiceResponse<List<GroupFunctionResponseDTO>>> GetGroupFunctionWithGroupID(string groupId);
        Task<ServiceResponse<List<FunctionResponseDTO>>> GetAllGroupFunctions();
        Task<ServiceResponse<bool>> DeleteGroupFunction(string groupId,string functionId);
    }
}
