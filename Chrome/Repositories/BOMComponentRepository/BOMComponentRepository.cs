using Chrome.DTO.BOMComponentDTO;
using Chrome.Models;
using Chrome.Repositories.RepositoryBase;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Repositories.BOMComponentRepository
{
    public class BOMComponentRepository : RepositoryBase<BomComponent>, IBOMComponentRepository
    {
        private readonly ChromeContext _context;
        public BOMComponentRepository(ChromeContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<BomComponent>> GetAllBOMComponent(string bomCode, string bomVersion)
        {
            var lstBomComponent = await _context.BomComponents
                                                .Include(x => x.ComponentCodeNavigation)
                                                .Where(x => x.Bomcode == bomCode && x.BomVersion == bomVersion)
                                                .ToListAsync();
            return lstBomComponent;
        }

        public async Task<BomComponent> GetBomComponent(string bomCode, string componentCode, string bomVersion)
        {
            var bomComponent = await _context.BomComponents
                                             .Include(x => x.ComponentCodeNavigation)
                                             .FirstOrDefaultAsync(x => x.Bomcode == bomCode
                                             && x.BomVersion == bomVersion
                                             && x.ComponentCode == componentCode);
            return bomComponent!;
        }
    

        public async Task<List<BOMNodeDTO>> GetRecursiveBOMAsync(string topLevelBOM, string topLevelVersion)
        {
            var bomMasters = await _context.Set<Bommaster>()
                                           
                                           .Include(x => x.BomComponents)
                                           .ThenInclude(x=>x.ComponentCodeNavigation)
                                           .Include(x =>x.ProductCodeNavigation)
                                
                                           .ToListAsync();
            var result = new List<BOMNodeDTO>();
            BuildRecursiveBOM(bomMasters, topLevelBOM, topLevelVersion, 1, 1.0f, result);
            return result.OrderBy(r => r.Level)
                         .ThenBy(r => r.BOMCode)
                         .ThenBy(r => r.ComponentCode)
                         .ToList();
        }

        private void BuildRecursiveBOM(List<Bommaster> bomMasters, string bomCode, string bomVersion, int level, float parentQuantity, List<BOMNodeDTO> result)
        {
            var bom = bomMasters.FirstOrDefault(b => b.Bomcode == bomCode && b.Bomversion == bomVersion);
            if (bom == null) return;

            foreach (var component in bom.BomComponents)
            {
                result.Add(new BOMNodeDTO
                {
                    BOMCode = bom.Bomcode,
                    ProductCode = bom.ProductCode!,
                    ProductName = bom.ProductCodeNavigation!.ProductName!,
                    BOMVersion = bom.Bomversion,
                    ComponentCode = component.ComponentCode,
                    ComponentName = component.ComponentCodeNavigation.ProductName!,
                    TotalQuantity = (float)(parentQuantity * component.ConsumpQuantity)!,
                    Level = level,
                });

                var childBom = bomMasters.FirstOrDefault(b => b.ProductCode == component.ComponentCode && b.IsActive==true);
                if (childBom != null)
                {
                    BuildRecursiveBOM(bomMasters, childBom.Bomcode, childBom.Bomversion, level + 1, (float)(parentQuantity * component.ConsumpQuantity)!, result);

                }
            }
        }
    }
}
