using Chrome.DTO;
using Chrome.DTO.AccountManagementDTO;
using Chrome.DTO.InventoryDTO;
using Chrome.DTO.StatusMasterDTO;
using Chrome.DTO.StockTakeDetailDTO;
using Chrome.DTO.StockTakeDTO;
using Chrome.DTO.WarehouseMasterDTO;
using Chrome.Models;
using Chrome.Repositories.AccountRepository;
using Chrome.Repositories.InventoryRepository;
using Chrome.Repositories.StatusMasterRepository;
using Chrome.Repositories.StockTakeDetailRepository;
using Chrome.Repositories.StockTakeRepository;
using Chrome.Repositories.WarehouseMasterRepository;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Services.StockTakeService
{
    public class StockTakeService : IStockTakeService
    {
        private readonly IStockTakeRepository _StockTakeRepository;
        private readonly IStockTakeDetailRepository _StockTakeDetailRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IStatusMasterRepository _statusMasterRepository;
        private readonly IWarehouseMasterRepository _warehouseMasterRepository;
        private readonly ChromeContext _context;

        public StockTakeService(
            IStockTakeRepository StockTakeRepository,
            IStockTakeDetailRepository StockTakeDetailRepository,
            IInventoryRepository inventoryRepository,
            IAccountRepository accountRepository,
            IStatusMasterRepository statusMasterRepository,
            IWarehouseMasterRepository warehouseMasterRepository,
            ChromeContext context)
        {
            _StockTakeRepository = StockTakeRepository ?? throw new ArgumentNullException(nameof(StockTakeRepository));
            _StockTakeDetailRepository = StockTakeDetailRepository ?? throw new ArgumentNullException(nameof(StockTakeDetailRepository));
            _inventoryRepository = inventoryRepository ?? throw new ArgumentNullException(nameof(inventoryRepository));
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _statusMasterRepository = statusMasterRepository ?? throw new ArgumentNullException(nameof(statusMasterRepository));
            _warehouseMasterRepository = warehouseMasterRepository ?? throw new ArgumentNullException(nameof(warehouseMasterRepository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ServiceResponse<PagedResponse<StockTakeResponseDTO>>> GetAllStockTakesAsync(string[] warehouseCodes, int page = 1, int pageSize = 10)
        {
            try
            {
                if (warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
                {
                    return new ServiceResponse<PagedResponse<StockTakeResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
                }

                var query = _StockTakeRepository.GetAllStockTakesAsync(warehouseCodes);
                var totalItems = await query.CountAsync();
                var StockTakes = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(st => new StockTakeResponseDTO
                    {
                        StocktakeCode = st.StocktakeCode,
                        StocktakeDate = st.StocktakeDate!.Value.ToString("dd/MM/yyyy"),
                        WarehouseCode = st.WarehouseCode,
                        WarehouseName = st.WarehouseCodeNavigation!.WarehouseName,
                        Responsible = st.Responsible,
                        FullNameResponsible = st.ResponsibleNavigation!.FullName,
                        StatusId = st.StatusId,
                        StatusName = st.Status!.StatusName
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<StockTakeResponseDTO>(StockTakes, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<StockTakeResponseDTO>>(true, "Lấy danh sách kiểm kho thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<StockTakeResponseDTO>>(false, $"Lỗi khi lấy danh sách kiểm kho: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<StockTakeResponseDTO>>> GetStockTakesByStatusAsync(string[] warehouseCodes, int statusId, int page = 1, int pageSize = 10)
        {
            try
            {
                if (warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
                {
                    return new ServiceResponse<PagedResponse<StockTakeResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
                }

                var query = _StockTakeRepository.GetAllStockTakesAsync(warehouseCodes)
                    .Where(st => st.StatusId == statusId);
                var totalItems = await query.CountAsync();
                var StockTakes = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(st => new StockTakeResponseDTO
                    {
                        StocktakeCode = st.StocktakeCode,
                        StocktakeDate = st.StocktakeDate!.Value.ToString("dd/MM/yyyy"),
                        WarehouseCode = st.WarehouseCode,
                        WarehouseName = st.WarehouseCodeNavigation!.WarehouseName,
                        Responsible = st.Responsible,
                        FullNameResponsible = st.ResponsibleNavigation!.FullName,
                        StatusId = st.StatusId,
                        StatusName = st.Status!.StatusName
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<StockTakeResponseDTO>(StockTakes, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<StockTakeResponseDTO>>(true, "Lấy danh sách kiểm kho theo trạng thái thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<StockTakeResponseDTO>>(false, $"Lỗi khi lấy danh sách kiểm kho theo trạng thái: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<StockTakeResponseDTO>>> SearchStockTakesAsync(string[] warehouseCodes, string textToSearch, int page = 1, int pageSize = 10)
        {
            try
            {
                if (warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
                {
                    return new ServiceResponse<PagedResponse<StockTakeResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
                }

                var query = _StockTakeRepository.SearchStockTakesAsync(warehouseCodes, null, textToSearch);
                var totalItems = await query.CountAsync();
                var StockTakes = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(st => new StockTakeResponseDTO
                    {
                        StocktakeCode = st.StocktakeCode,
                        StocktakeDate = st.StocktakeDate!.Value.ToString("dd/MM/yyyy"),
                        WarehouseCode = st.WarehouseCode,
                        WarehouseName = st.WarehouseCodeNavigation!.WarehouseName,
                        Responsible = st.Responsible,
                        FullNameResponsible = st.ResponsibleNavigation!.FullName,
                        StatusId = st.StatusId,
                        StatusName = st.Status!.StatusName
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<StockTakeResponseDTO>(StockTakes, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<StockTakeResponseDTO>>(true, "Tìm kiếm kiểm kho thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<StockTakeResponseDTO>>(false, $"Lỗi khi tìm kiếm kiểm kho: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> AddStockTake(StockTakeRequestDTO StockTake)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validate input
                if (StockTake == null || string.IsNullOrEmpty(StockTake.StocktakeCode) || string.IsNullOrEmpty(StockTake.WarehouseCode) || string.IsNullOrEmpty(StockTake.Responsible))
                {
                    return new ServiceResponse<bool>(false, "Dữ liệu đầu vào không hợp lệ. StockTakeCode, WarehouseCode và Responsible là bắt buộc.");
                }

                // Validate date format
                string[] formats = { "dd/MM/yyyy", "M/d/yyyy h:mm:ss tt", "MM/dd/yyyy hh:mm:ss tt" };
                if (!DateTime.TryParseExact(StockTake.StocktakeDate, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                {
                    return new ServiceResponse<bool>(false, "Ngày kiểm kho không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy.");
                }

                // Check if StockTake already exists
                var existingStockTake = await _context.Stocktakes
                    .FirstOrDefaultAsync(st => st.StocktakeCode == StockTake.StocktakeCode);
                if (existingStockTake != null)
                {
                    return new ServiceResponse<bool>(false, "Mã kiểm kho đã tồn tại.");
                }

                // Create StockTake
                var newStockTake = new Stocktake
                {
                    StocktakeCode = StockTake.StocktakeCode,
                    StocktakeDate = parsedDate,
                    WarehouseCode = StockTake.WarehouseCode,
                    Responsible = StockTake.Responsible,
                    StatusId =  1 // Default to 1 if not provided
                };
                await _StockTakeRepository.AddAsync(newStockTake, saveChanges: false);

                // Get all products in inventory for the warehouse
                var inventoryItems = await _inventoryRepository.GetInventoryByWarehouseCodeAsync(StockTake.WarehouseCode)
                    
                    .Select(g => new StockTakeDetailResponseDTO
                    {
                        ProductCode = g.ProductCode,
                        ProductName = g.ProductCodeNavigation.ProductName!,
                        LocationCode = g.LocationCode,
                        LocationName = g.LocationCodeNavigation.LocationName!,
                        Lotno = g.Lotno,
                        Quantity = g.Quantity,
                        CountedQuantity =0,
                    })
                    .ToListAsync();

                if (!inventoryItems.Any())
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Không tìm thấy sản phẩm trong tồn kho cho kho {StockTake.WarehouseCode}.");
                }

                // Create StockTakeDetail entries
                foreach (var item in inventoryItems)
                {
                    var StockTakeDetail = new StocktakeDetail
                    {
                        StocktakeCode = StockTake.StocktakeCode,
                        ProductCode = item.ProductCode,
                        Lotno = item.Lotno,
                        Quantity= item.Quantity,
                        CountedQuantity=0,
                        LocationCode= item.LocationCode,
                        
                    };
                    await _context.StocktakeDetails.AddAsync(StockTakeDetail);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResponse<bool>(true, "Thêm kiểm kho và chi tiết kiểm kho thành công");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                if (dbEx.InnerException != null)
                {
                    string error = dbEx.InnerException.Message.ToLower();
                    if (error.Contains("unique") || error.Contains("duplicate") || error.Contains("primary key"))
                    {
                        return new ServiceResponse<bool>(false, "Mã kiểm kho hoặc chi tiết kiểm kho đã tồn tại");
                    }
                    if (error.Contains("foreign key"))
                    {
                        return new ServiceResponse<bool>(false, "Dữ liệu tham chiếu không đúng (WarehouseCode, Responsible hoặc StatusID)");
                    }
                }
                return new ServiceResponse<bool>(false, $"Lỗi database khi thêm kiểm kho: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi khi thêm kiểm kho: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> UpdateStockTake(StockTakeRequestDTO StockTake)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (StockTake == null || string.IsNullOrEmpty(StockTake.StocktakeCode))
                {
                    return new ServiceResponse<bool>(false, "Dữ liệu đầu vào không hợp lệ. StockTakeCode là bắt buộc.");
                }

                var existingStockTake = await _context.Stocktakes
                    .FirstOrDefaultAsync(st => st.StocktakeCode == StockTake.StocktakeCode);
                if (existingStockTake == null)
                {
                    return new ServiceResponse<bool>(false, "Kiểm kho không tồn tại.");
                }

                // Update allowed fields
                if (!string.IsNullOrEmpty(StockTake.StocktakeDate))
                {
                    string[] formats = { "dd/MM/yyyy", "M/d/yyyy h:mm:ss tt", "MM/dd/yyyy hh:mm:ss tt" };
                    if (DateTime.TryParseExact(StockTake.StocktakeDate, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                    {
                        existingStockTake.StocktakeDate = parsedDate;
                    }
                    else
                    {
                        return new ServiceResponse<bool>(false, "Ngày kiểm kho không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy.");
                    }
                }
                existingStockTake.WarehouseCode = StockTake.WarehouseCode;
                existingStockTake.Responsible = StockTake.Responsible;
                existingStockTake.StatusId = StockTake.StatusId;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResponse<bool>(true, "Cập nhật kiểm kho thành công");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                if (dbEx.InnerException != null)
                {
                    string error = dbEx.InnerException.Message.ToLower();
                    if (error.Contains("foreign key"))
                    {
                        return new ServiceResponse<bool>(false, "Dữ liệu tham chiếu không đúng (WarehouseCode, Responsible hoặc StatusID)");
                    }
                }
                return new ServiceResponse<bool>(false, $"Lỗi database khi cập nhật kiểm kho: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật kiểm kho: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> DeleteStockTakeAsync(string StockTakeCode)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrEmpty(StockTakeCode))
                {
                    return new ServiceResponse<bool>(false, "Mã kiểm kho không được để trống");
                }

                var StockTake = await _context.Stocktakes
                    .FirstOrDefaultAsync(st => st.StocktakeCode == StockTakeCode);
                if (StockTake == null)
                {
                    return new ServiceResponse<bool>(false, "Kiểm kho không tồn tại.");
                }

                // Delete associated StockTakeDetails
                var StockTakeDetails = await _context.StocktakeDetails
                    .Where(sd => sd.StocktakeCode == StockTakeCode)
                    .ToListAsync();
                _context.StocktakeDetails.RemoveRange(StockTakeDetails);

                await _StockTakeRepository.DeleteAsync(StockTakeCode, saveChanges: false);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResponse<bool>(true, "Xóa kiểm kho và chi tiết kiểm kho thành công");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                if (dbEx.InnerException != null)
                {
                    string error = dbEx.InnerException.Message.ToLower();
                    if (error.Contains("foreign key"))
                    {
                        return new ServiceResponse<bool>(false, "Không thể xóa kiểm kho vì có dữ liệu tham chiếu.");
                    }
                }
                return new ServiceResponse<bool>(false, $"Lỗi database khi xóa kiểm kho: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi khi xóa kiểm kho: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<List<AccountManagementResponseDTO>>> GetListResponsibleAsync(string warehouseCode)
        {
            try
            {
                if (string.IsNullOrEmpty(warehouseCode))
                {
                    return new ServiceResponse<List<AccountManagementResponseDTO>>(false, "Mã kho không được để trống");
                }

                var accounts = await _accountRepository.GetAllAccount(1, int.MaxValue);
                var responsibleList = accounts
                    .Where(x => !x.GroupId!.StartsWith("ADMIN") && !x.GroupId.StartsWith("QLKHO") /* && x.WarehouseCode == warehouseCode */)
                    .Select(x => new AccountManagementResponseDTO
                    {
                        UserName = x.UserName,
                        FullName = x.FullName!,
                        GroupID = x.GroupId!,
                        GroupName = x.Group!.GroupId,
                        Password = x.Password!
                    })
                    .ToList();

                return new ServiceResponse<List<AccountManagementResponseDTO>>(true, "Lấy danh sách nhân viên chịu trách nhiệm thành công", responsibleList);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<AccountManagementResponseDTO>>(false, $"Lỗi khi lấy danh sách nhân viên chịu trách nhiệm: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<List<StatusMasterResponseDTO>>> GetListStatusMaster()
        {
            try
            {
                var statuses = await _statusMasterRepository.GetAllStatuses();
                var statusList = statuses.Select(x => new StatusMasterResponseDTO
                {
                    StatusId = x.StatusId,
                    StatusName = x.StatusName
                }).ToList();

                return new ServiceResponse<List<StatusMasterResponseDTO>>(true, "Lấy danh sách trạng thái thành công", statusList);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StatusMasterResponseDTO>>(false, $"Lỗi khi lấy danh sách trạng thái: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<List<WarehouseMasterResponseDTO>>> GetListWarehousePermission(string[] warehouseCodes)
        {
            try
            {
                if (warehouseCodes.Length == 0)
                {
                    return new ServiceResponse<List<WarehouseMasterResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
                }

                var warehouses = await _warehouseMasterRepository.GetWarehouseMasters(1, int.MaxValue);
                var warehouseList = warehouses
                    .Where(x => warehouseCodes.Contains(x.WarehouseCode))
                    .Select(x => new WarehouseMasterResponseDTO
                    {
                        WarehouseCode = x.WarehouseCode,
                        WarehouseName = x.WarehouseName,
                        WarehouseAddress = x.WarehouseAddress,
                        WarehouseDescription = x.WarehouseDescription
                    })
                    .ToList();

                return new ServiceResponse<List<WarehouseMasterResponseDTO>>(true, "Lấy danh sách kho dựa theo quyền thành công", warehouseList);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<WarehouseMasterResponseDTO>>(false, $"Lỗi khi lấy danh sách kho: {ex.Message}");
            }
        }

    }
}
