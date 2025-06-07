using Chrome.DTO.GroupFunctionDTO;
using Chrome.Models;
using Chrome.Repositories.GroupFunctionRepository;
using Chrome.Repositories.RepositoryBase;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.EntityFrameworkCore;

namespace ProductionInventoryManagmentSystem_API.Repositories.GroupFunctionRepository
{
    public class GroupFunctionRepository : RepositoryBase<GroupFunction>, IGroupFunctionRepository
    {
        private readonly ChromeContext _context;

        public GroupFunctionRepository(ChromeContext context) : base(context)
        {
            _context = context;
        }


        public async Task<List<GroupFunction>> GetAllGroupsFunctionWithGroupId(string groupId)
        {
            return await _context.GroupFunctions
                .Where(x => x.GroupId == groupId)
                .Include(x => x.Function)
                .ToListAsync()
                .ContinueWith(t => t.Result
                    .GroupBy(x => x.FunctionId)
                    .Select(g => g.First())
                    .ToList());
        }



        public async Task<List<string>> GetListFunctionIDOfGroup(string groupId)
        {
            var lstFunction = await _context.GroupFunctions
                                            .Where(x => x.GroupId == groupId && x.IsEnable == true)
                                            .Select(x => x.FunctionId)
                                            .Distinct()
                                            .ToListAsync();
            return lstFunction;
        }
        public async Task<List<string>> GetListApplicableLocationOfGroup(string groupId)
        {
            var lstLocations = await _context.GroupFunctions
                                             .Where(x => x.GroupId == groupId && x.IsEnable == true)
                                             .Select(x => x.ApplicableLocation)
                                             .Distinct() // nếu muốn loại trùng
                                             .ToListAsync();
            return lstLocations!;
        }

        public async  Task<List<ApplicableLocationResponseDTO>> GetListApplicableSelected()
        {
            var lstApplicableLocations = await _context.WarehouseMasters
                .Select(wh => new ApplicableLocationResponseDTO
                {
                    ApplicableLocation = wh.WarehouseCode,
                    IsSelected = _context.GroupFunctions.Any(gf => gf.ApplicableLocation == wh.WarehouseCode)
                })
                .Distinct()
                .ToListAsync();

            return lstApplicableLocations;

        }
    }
}
