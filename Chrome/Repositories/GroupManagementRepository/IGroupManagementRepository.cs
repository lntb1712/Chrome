using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.GroupManagementRepository
{
    public interface IGroupManagementRepository:IRepositoryBase<GroupManagement>
    {
        Task<List<GroupManagement>> GetAllGroup();
        Task<GroupManagement> GetGroupManagementWithGroupID(string GroupID);
        Task<List<GroupManagement>> SearchGroup(string textToSearch);
    }
}
