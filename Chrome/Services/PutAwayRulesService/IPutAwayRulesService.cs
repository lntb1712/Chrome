using Chrome.DTO;
using Chrome.DTO.PutAwayRulesDTO;

namespace Chrome.Services.PutAwayRulesService
{
    public interface IPutAwayRulesService
    {
        Task<ServiceResponse<PagedResponse<PutAwayRulesResponseDTO>>> GetAllPutAwayRules(int page, int pageSize);
        Task<ServiceResponse<PutAwayRulesResponseDTO>> GetPutAwayRuleWithCode(string putAwayRuleCode);
        Task<ServiceResponse<int>> GetTotalPutAwayRuleCount();
        Task<ServiceResponse<PagedResponse<PutAwayRulesResponseDTO>>> SearchPutAwayRules(string textToSearch, int page, int pageSize);
        Task<ServiceResponse<bool>> AddPutAwayRule(PutAwayRulesRequestDTO putAwayRuleRequestDTO);
        Task<ServiceResponse<bool>> DeletePutAwayRule(string putAwayRuleCode);
        Task<ServiceResponse<bool>> UpdatePutAwayRule(PutAwayRulesRequestDTO putAwayRuleRequestDTO);

    }
}
