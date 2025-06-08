using Chrome.DTO.GroupFunctionDTO;
using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.GroupFunctionRepository
{
    public interface IGroupFunctionRepository:IRepositoryBase<GroupFunction>
    {
        Task<List<ApplicableLocationResponseDTO>> GetListApplicableSelected(string groudId,string functionId);
        Task<List<GroupFunction>> GetAllGroupsFunctionWithGroupId(string groupId);
        Task<List<string>> GetListFunctionIDOfGroup(string groupId);
        Task<List<string>> GetListApplicableLocationOfGroup(string groupId);
    }
}
