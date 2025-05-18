using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;


namespace Chrome.Repositories.AccountRepository
{
    public class AccountRepository: RepositoryBase<AccountManagement>, IAccountRepository
    {
        private readonly ChromeContext _context;
        public AccountRepository(ChromeContext context) :base(context) 
        {
            _context = context;
        }
        public async Task<List<AccountManagement>> GetAllAccount()
        {
            var lstAccount = await _context.AccountManagements
                                .Include(x=>x.Group)
                                .ToListAsync();
            return lstAccount;
        }

        public async Task<List<AccountManagement>> GetAllWithRole(string groupID)
        {
            var lstAccountWithRole =await _context.AccountManagements
                                        .Include(x=>x.Group)
                                        .Where(x=>x.GroupId == groupID)
                                        .ToListAsync();
            return lstAccountWithRole;
        }

        public async Task<AccountManagement> GetAccountWithUserName(string userName)
        {
            var lstAccount = await _context.AccountManagements
                              .Include(x => x.Group)
                               .FirstOrDefaultAsync(x=>x.UserName == userName);
            return lstAccount!;
        }

        public async Task<AccountManagement> GetAccountWithUserNameAndPassword(string userName, string password)
        {
            var account = await FindAsync(x => x.UserName == userName && x.Password == password);
            return account;
        }

        public async Task<List<AccountManagement>> SearchAccount(string textToSearch)
        {
            var account = await _context.AccountManagements
                                .Include(x=>x.Group)
                                .Where(x=>x.UserName.Contains(textToSearch)||x.GroupId!.Contains(textToSearch)||x.FullName!.Contains(textToSearch))
                                .ToListAsync();
            return account;
        }
    }
}
