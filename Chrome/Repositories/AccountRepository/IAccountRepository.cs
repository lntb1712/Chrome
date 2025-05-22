using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.AccountRepository
{
    public interface IAccountRepository :IRepositoryBase<AccountManagement>
    {
        Task<AccountManagement> GetAccountWithUserNameAndPassword(string userName, string password);
        Task<List<AccountManagement>> GetAllAccount(int page, int pageSize);
        Task<int> GetTotalAccountCount();
        Task<AccountManagement> GetAccountWithUserName(string userName);
        Task<List<AccountManagement>> GetAllWithRole(string groupID,int page, int pageSize);
        Task<int> GetTotalWithRoleCount(string groupID);
        Task<List<AccountManagement>> SearchAccount (string textToSearch,int page,int pageSize);
        Task<int> GetTotalSearchCount(string textToSearch);

    }
}
