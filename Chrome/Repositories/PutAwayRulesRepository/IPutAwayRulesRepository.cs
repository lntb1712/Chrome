using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.PutAwayRulesRepository
{
    public interface IPutAwayRulesRepository:IRepositoryBase<PutAwayRule>
    {
        Task<List<PutAwayRule>> GetAllPutAwayRules(int page, int pageSize);
        Task<int> GetTotalPutAwayRuleCount();
        Task<PutAwayRule> GetPutAwayRuleWithCode(string putAwayRuleCode);
        Task<List<PutAwayRule>> SearchPutAwayRules(string textToSearch, int page, int pageSize);
        Task<int> GetTotalSearchCount(string textToSearch);
    }
}
