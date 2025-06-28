using Chrome.Models;
using Chrome.Repositories.PickListRepository;
using Chrome.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Chrome.Repositories.PickListRepository
{
    public class PickListRepository : RepositoryBase<PickList>, IPickListRepository
    {
        private readonly ChromeContext _context;

        public PickListRepository(ChromeContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IQueryable<PickList> GetAllPickListAsync(string[] warehouseCodes)
        {
            var lstPickList = _context.PickLists
                                      .Include(x => x.ReservationCodeNavigation)
                                      .Include(x => x.WarehouseCodeNavigation)
                                      .Include(x => x.Status)
                                      .Where(x => warehouseCodes.Contains(x.WarehouseCode));

            return lstPickList;
        }

        public IQueryable<PickList> GetAllPickListWithStatus(string[] warehouseCodes, int statusId)
        {
            var lstPickList = _context.PickLists
                                      .Include(x => x.ReservationCodeNavigation)
                                      .Include(x => x.WarehouseCodeNavigation)
                                      .Include(x => x.Status)
                                      .Where(x => warehouseCodes.Contains(x.WarehouseCode) && x.StatusId == statusId);

            return lstPickList;
        }

        public async Task<PickList> GetPickListContainCode(string orderCode)
        {
            if (string.IsNullOrEmpty(orderCode))
            {
                throw new ArgumentNullException(nameof(orderCode), "Mã lệnh không được để trống.");
            }

            var pickList = await _context.PickLists
                                         .Include(x => x.ReservationCodeNavigation)
                                         .Include(x => x.WarehouseCodeNavigation)
                                         .Include(x=>x.PickListDetails)
                                         .Include(x => x.Status)
                                         .FirstOrDefaultAsync(x => x.PickNo.Contains(orderCode));

            return pickList!;
        }

        public async Task<PickList> GetPickListWithCode(string pickNo)
        {
            if (string.IsNullOrEmpty(pickNo))
            {
                throw new ArgumentNullException(nameof(pickNo), "Mã pick list không được để trống.");
            }

            var pickList = await _context.PickLists
                                         .Include(x => x.ReservationCodeNavigation)
                                         .Include(x => x.WarehouseCodeNavigation)
                                         .Include(x => x.Status)
                                         .FirstOrDefaultAsync(x => x.PickNo == pickNo);

            return pickList!;
        }

        public IQueryable<PickList> SearchPickListAsync(string[] warehouseCodes, string textToSearch)
        {
            var lstPickList = _context.PickLists
                                      .Include(x => x.ReservationCodeNavigation)
                                      .Include(x => x.WarehouseCodeNavigation)
                                      .Include(x => x.Status)
                                      .Where(x => warehouseCodes.Contains(x.WarehouseCode)
                                          && (x.PickNo.Contains(textToSearch)
                                              || x.WarehouseCode!.Contains(textToSearch)
                                              || x.WarehouseCodeNavigation!.WarehouseName!.Contains(textToSearch)
                                              || (x.ReservationCodeNavigation != null && x.ReservationCodeNavigation.ReservationCode!.Contains(textToSearch))
                                              || (x.ReservationCodeNavigation != null && x.ReservationCodeNavigation.OrderTypeCode!.Contains(textToSearch))
                                              || (x.Status != null && x.Status.StatusName!.Contains(textToSearch))));

            return lstPickList;
        }
    }
}