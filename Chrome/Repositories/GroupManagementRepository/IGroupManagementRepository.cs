using Chrome.DTO.GroupManagementDTO;
using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.GroupManagementRepository
{
    public interface IGroupManagementRepository:IRepositoryBase<GroupManagement>
    {
        Task<List<GroupManagement>> GetAllGroup(int page,int pageSize);
        Task<int> GetTotalGroupCount();
        Task<GroupManagement> GetGroupManagementWithGroupID(string GroupID);
        Task<List<GroupManagement>> SearchGroup(string textToSearch, int page, int pageSize);
        Task<int>GetTotalSearchCount(string textToSearch);
        Task<List<GroupManagementTotalDTO>> GetTotalUserInGroup();
    }
}