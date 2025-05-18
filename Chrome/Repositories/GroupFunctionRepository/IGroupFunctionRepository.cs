using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.GroupFunctionRepository
{
    public interface IGroupFunctionRepository:IRepositoryBase<GroupFunction>
    {
        Task<List<Function>> GetFunctionsAsync();
        Task<List<GroupFunction>> GetAllGroupsFunctionWithGroupId(string groupId);
        Task<List<string>> GetListFunctionIDOfGroup(string groupId);
    }
}
