using Chrome.DTO;
using Chrome.DTO.GroupManagementDTO;

namespace Chrome.Services.GroupManagementService
{
    public interface IGroupManagementService
    {
        Task<List<GroupManagementResponseDTO>> GetAllGroupManagement();
        Task<ServiceResponse<bool>> AddGroupManagement(GroupManagementRequestDTO group);
        Task<ServiceResponse<bool>> DeleteGroupManagement(string groupId);
        Task<ServiceResponse<bool>> UpdateGroupManagement(GroupManagementRequestDTO group);
        Task<GroupManagementResponseDTO> GetGroupManagementWithGroupId(string groupId);
        Task<List<GroupManagementResponseDTO>> SearchGroup(string textToSearch);
    }
}
