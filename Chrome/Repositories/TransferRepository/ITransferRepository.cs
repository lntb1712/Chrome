using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.TransferRepository
{
    public interface ITransferRepository : IRepositoryBase<Transfer>
    {
        IQueryable<Transfer> GetAllTransfersAsync(string[] warehouseCodes);
        IQueryable<Transfer> GetAllTransfersWithStatus(string[] warehouseCodes, int statusId);
        IQueryable<Transfer> SearchTransfersAsync(string[] warehouseCodes, string textToSearch);
        Task<Transfer> GetTransferWithTransferCode(string transferCode);
    }
}
