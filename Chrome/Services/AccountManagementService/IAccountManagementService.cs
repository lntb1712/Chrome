using Chrome.DTO;
using Chrome.DTO.AccountManagementDTO;

namespace Chrome.Services.AccountManagementService
{
    public interface IAccountManagementService
    {
        Task<List<AccountManagementResponseDTO>> GetAllAccount();
        Task<AccountManagementResponseDTO> GetUserInformation(string userName);
        Task<ServiceResponse<bool>> AddAccountManagement(AccountManagementRequestDTO account);
        Task<ServiceResponse<bool>> DeleteAccountManagement(string id);
        Task<ServiceResponse<bool>> UpdateAccountManagement(AccountManagementRequestDTO account);
        Task<List<AccountManagementResponseDTO>> GetAllAccountWithGroupId(string groupId);
        Task<List<AccountManagementResponseDTO>> SearchAccount(string textToSearch);
    }
}
