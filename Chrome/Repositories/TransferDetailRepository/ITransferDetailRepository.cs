using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.TransferDetailRepository
{
    public interface ITransferDetailRepository : IRepositoryBase<TransferDetail>
    {
        IQueryable<TransferDetail> GetAllTransferDetailsAsync(string[] warehouseCodes);
        IQueryable<TransferDetail> GetTransferDetailsByTransferCodeAsync(string transferCode);
        IQueryable<TransferDetail> SearchTransferDetailsAsync(string[] warehouseCodes, string transferCode, string textToSearch);
    }
}
