using Chrome.DTO.BOMComponentDTO;
using Chrome.Models;
using Chrome.Repositories.RepositoryBase;

namespace Chrome.Repositories.BOMComponentRepository
{
    public interface IBOMComponentRepository: IRepositoryBase<BomComponent>
    {
        Task<List<BomComponent>> GetAllBOMComponent(string bomCode, string bomVersion);
        Task<BomComponent> GetBomComponent(string bomCode, string componentCode,string bomVersion);
        Task<List<BOMNodeDTO>> GetRecursiveBOMAsync(string topLevelBOM, string topLevelVersion);
    }
}
