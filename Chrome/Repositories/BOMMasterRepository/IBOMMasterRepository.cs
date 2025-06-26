using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.BOMMasterRepository
{
    public interface IBOMMasterRepository:IRepositoryBase<Bommaster>
    {
        Task<List<Bommaster>> GetAllBOMMaster(int page, int pageSize);
        Task<int> GetTotalBOMMasterCount();
        Task<Bommaster> GetBOMMasterByCode(string bomCode,string bomVersion); 
        Task<List<Bommaster>> SearchBOMMaster(string textToSearch, int page, int pageSize);
        Task<int> GetTotalSearchBOMMasterCount(string textToSearch);
        Task<List<Bommaster>> GetListVersionByBomCode(string bomCode);
        Task<Bommaster> GetBOMMasterByProductCode(string productCode);
    }
}
