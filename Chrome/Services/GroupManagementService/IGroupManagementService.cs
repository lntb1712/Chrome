using Chrome.DTO;
using Chrome.DTO.GroupManagementDTO;

namespace Chrome.Services.GroupManagementService
{
    public interface IGroupManagementService
    {
        Task<ServiceResponse<List<GroupManagementResponseDTO>>> GetAllGroupManagement();
        Task<ServiceResponse<bool>> AddGroupManagement(GroupManagementRequestDTO group);
        Task<ServiceResponse<bool>> DeleteGroupManagement(string groupId);
        Task<ServiceResponse<bool>> UpdateGroupManagement(GroupManagementRequestDTO group);
        Task<ServiceResponse<GroupManagementResponseDTO>> GetGroupManagementWithGroupId(string groupId);
        Task<ServiceResponse<List<GroupManagementResponseDTO>>> SearchGroup(string textToSearch);
    }
}
