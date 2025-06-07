using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.CustomerMasterRepository
{
    public interface ICustomerMasterRepository : IRepositoryBase<CustomerMaster>
    {
        Task <List<CustomerMaster>> GetAllCustomer(int page, int pageSize); 
        Task<int> GetTotalCustomerCount();  
        Task<List<CustomerMaster>> SearchCustomer(string textToSearch, int page, int pageSize);
        Task<int> GetTotalSearchCount(string textToSearch);
        Task<CustomerMaster> GetCustomerMasterWithCustomerCode(string customerCode);
    }
}
