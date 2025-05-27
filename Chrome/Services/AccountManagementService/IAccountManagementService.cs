using Chrome.DTO;
using Chrome.DTO.AccountManagementDTO;

namespace Chrome.Services.AccountManagementService
{
    public interface IAccountManagementService
    {
        Task<ServiceResponse<PagedResponse<AccountManagementResponseDTO>>> GetAllAccount(int page,int pageSize);
        Task<ServiceResponse<int>> GetTotalAccount();
        Task<ServiceResponse<AccountManagementResponseDTO>> GetUserInformation(string userName);
        Task<ServiceResponse<bool>> AddAccountManagement(AccountManagementRequestDTO account);
        Task<ServiceResponse<bool>> DeleteAccountManagement(string id);
        Task<ServiceResponse<bool>> UpdateAccountManagement(AccountManagementRequestDTO account);
        Task<ServiceResponse<PagedResponse<AccountManagementResponseDTO>>> GetAllAccountWithGroupId(string groupId,int page,int pageSize);     
        Task<ServiceResponse<PagedResponse<AccountManagementResponseDTO>>> SearchAccount(string textToSearch, int page, int pageSize);
    }
}
