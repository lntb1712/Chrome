using Chrome.DTO;
using Chrome.DTO.InventoryDTO;
using Chrome.Models;
using Chrome.Repositories.InventoryRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Chrome.Services.InventoryService
{
    public class InventoryService: IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ChromeContext _context;

        public InventoryService(IInventoryRepository inventoryRepository, ChromeContext context)
        {
            _inventoryRepository = inventoryRepository;
            _context = context;
        }

        public async Task<ServiceResponse<bool>> AddInventory(InventoryRequestDTO inventoryRequestDTO)
        {
            if(inventoryRequestDTO == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }    
            var inventory = new Inventory
            {
                WarehouseCode = inventoryRequestDTO.WarehouseCode,
                LocationCode = inventoryRequestDTO.LocationCode,
                ProductCode = inventoryRequestDTO.ProductCode,
                Lotno=inventoryRequestDTO.LotNo,
                Quantity = inventoryRequestDTO.Quantity,
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _inventoryRepository.AddAsync(inventory, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Thêm tồn kho thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("unique") || error.Contains("duplicate") || error.Contains("primary key"))
                        {
                            return new ServiceResponse<bool>(false, "Dữ liệu đã tồn tại");
                        }
                        if (error.Contains("foreign key"))
                        {
                            return new ServiceResponse<bool>(false, "Dữ liệu tham chiếu không đúng");
                        }
                    }
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi thêm tồn kho: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteInventoryAsync(string warehouseCode, string locationCode, string productCode, string lotNo)
        {
            if(string.IsNullOrEmpty(warehouseCode) || string.IsNullOrEmpty(locationCode)|| string.IsNullOrEmpty(productCode) || string.IsNullOrEmpty(lotNo))
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không được để trống");
            }    
            var inventory = await _inventoryRepository.GetInventoryWithCode(warehouseCode, locationCode, productCode,lotNo);
            if(inventory == null)
            {
                return new ServiceResponse<bool>(false, "Tồn kho không tồn tại");
            }    
            using ( var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    Expression<Func<Inventory, bool>> predicate = x => x.WarehouseCode == warehouseCode && x.LocationCode == locationCode && x.ProductCode == productCode && x.Lotno == lotNo;
                    await _inventoryRepository.DeleteFirstByConditionAsync(predicate);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa tồn kho thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("foreign key"))
                        {
                            return new ServiceResponse<bool>(false, "Không thể xóa tồn kho vì có dữ liệu tham chiếu.");
                        }
                    }
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xóa tồn kho: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<PagedResponse<InventorySummaryDTO>>> GetListProductInventory(string[] warehouseCodes, int page, int pageSize)
        {
            if (warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<InventorySummaryDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            var query = _inventoryRepository.GetInventories(warehouseCodes);

            // Đếm tổng số nhóm ProductCode trước khi phân trang
            var totalItems = await query.GroupBy(x => x.ProductCode).CountAsync();

            // Query lấy data phân trang
            var result = await query
                .GroupBy(x => x.ProductCode)
                .Select(g => new InventorySummaryDTO
                {
                    ProductCode = g.Key,
                    ProductName = g.First().ProductCodeNavigation.ProductName!,
                    CategoryId = g.First().ProductCodeNavigation.CategoryId!,
                    CategoryName = g.First().ProductCodeNavigation.Category!.CategoryName!,
                    Quantity = g.Sum(x => x.Quantity),
                    BaseQuantity = g.Sum(x => x.Quantity / (x.ProductCodeNavigation.BaseQuantity)),
                    UOM = g.First().ProductCodeNavigation.Uom!,
                    BaseUOM = g.First().ProductCodeNavigation.BaseUom!
                })
                .OrderBy(x => x.ProductCode)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Gói kết quả vào PagedResponse
            var pagedResponse = new PagedResponse<InventorySummaryDTO>(result,page,pageSize,totalItems);

            return new ServiceResponse<PagedResponse<InventorySummaryDTO>>(true, "Lấy danh sách thành công", pagedResponse);

        }

        public async Task<ServiceResponse<PagedResponse<InventorySummaryDTO>>> GetListProductInventoryByCategoryIds(string[] warehouseCodes, string[] categoryIds, int page, int pageSize)
        {
            if (warehouseCodes.Length == 0||categoryIds.Length==0 || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<InventorySummaryDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var query = _inventoryRepository.GetInventoriesByCategoryIds(warehouseCodes,categoryIds);

            // Đếm tổng số nhóm ProductCode trước khi phân trang
            var totalItems = await query.GroupBy(x => x.ProductCode).CountAsync();

            // Query lấy data phân trang
            var result = await query
                .GroupBy(x => x.ProductCode)
                .Select(g => new InventorySummaryDTO
                {
                    ProductCode = g.Key,
                    ProductName = g.First().ProductCodeNavigation.ProductName!,
                    CategoryId = g.First().ProductCodeNavigation.CategoryId!,
                    CategoryName = g.First().ProductCodeNavigation.Category!.CategoryName!,
                    Quantity = g.Sum(x => x.Quantity),
                    BaseQuantity = g.Sum(x => x.Quantity / (x.ProductCodeNavigation.BaseQuantity)),
                    UOM = g.First().ProductCodeNavigation.Uom!,
                    BaseUOM = g.First().ProductCodeNavigation.BaseUom!
                })
                .OrderBy(x => x.ProductCode)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Gói kết quả vào PagedResponse
            var pagedResponse = new PagedResponse<InventorySummaryDTO>(result, page, pageSize, totalItems);

            return new ServiceResponse<PagedResponse<InventorySummaryDTO>>(true, "Lấy danh sách thành công", pagedResponse);
        }

        public async Task<ServiceResponse<PagedResponse<ProductWithLocationsDTO>>> GetProductWithLocationsAsync(string[] warehouseCodes,string productCode, int page, int pageSize)
        {
            if (warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<ProductWithLocationsDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            var query = _inventoryRepository.GetInventories(warehouseCodes);

            var groupQuery = query
                .Where(x=>x.ProductCode==productCode)
                .GroupBy(x => new
                {
                    x.ProductCode,
                    x.ProductCodeNavigation.ProductName,
                    x.ProductCodeNavigation.CategoryId,
                    x.ProductCodeNavigation.Category!.CategoryName
                })
                .Select(g => new ProductWithLocationsDTO
                {
                    ProductCode = g.Key.ProductCode,
                    ProductName = g.Key.ProductName!,
                    CategoryId = g.Key.CategoryId!,
                    CategoryName = g.Key.CategoryName!,
                    Locations = g
                        .GroupBy(x => new {
                            x.WarehouseCode,
                            x.WarehouseCodeNavigation.WarehouseName,
                            x.LocationCode,
                            x.LocationCodeNavigation.LocationName,
                            x.ProductCodeNavigation.BaseQuantity,
                            x.ProductCodeNavigation.Uom,
                            x.ProductCodeNavigation.BaseUom
                        })
                        .Select(lg => new LocationDetailDTO
                        {
                            WarehouseCode = lg.Key.WarehouseCode,
                            WarehouseName = lg.Key.WarehouseName!,
                            LocationCode = lg.Key.LocationCode,
                            LocationName = lg.Key.LocationName!,
                            Quantity = lg.Sum(x => x.Quantity),
                            BaseQuantity = lg.Sum(x => x.Quantity / (x.ProductCodeNavigation.BaseQuantity)),
                            UOM = lg.Key.Uom!,
                            BaseUOM = lg.Key.BaseUom!,
                        })
                        .ToList()
                });

            var totalItems = await groupQuery.CountAsync();

            var items = await groupQuery
                .OrderBy(x => x.ProductCode)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pagedResponse = new PagedResponse<ProductWithLocationsDTO>(items,page,pageSize,totalItems);
            

            return new ServiceResponse<PagedResponse<ProductWithLocationsDTO>>(true, "Lấy danh sách thành công", pagedResponse);
        }

        public async Task<ServiceResponse<PagedResponse<InventorySummaryDTO>>> SearchProductInventoryAsync(string[] warehouseCodes, string textToSearch, int page, int pageSize)
        {
            if (warehouseCodes.Length == 0 || string.IsNullOrEmpty(textToSearch)|| page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<InventorySummaryDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            var query = _inventoryRepository.SearchProductInventories(warehouseCodes,textToSearch);

            // Đếm tổng số nhóm ProductCode trước khi phân trang
            var totalItems = await query.GroupBy(x => x.ProductCode).CountAsync();

            // Query lấy data phân trang
            var result = await query
                .GroupBy(x => x.ProductCode)
                .Select(g => new InventorySummaryDTO
                {
                    ProductCode = g.Key,
                    ProductName = g.First().ProductCodeNavigation.ProductName!,
                    CategoryId = g.First().ProductCodeNavigation.CategoryId!,
                    CategoryName = g.First().ProductCodeNavigation.Category!.CategoryName!,
                    Quantity = g.Sum(x => x.Quantity),
                    BaseQuantity = g.Sum(x => x.Quantity / (x.ProductCodeNavigation.BaseQuantity)),
                    UOM = g.First().ProductCodeNavigation.Uom!,
                    BaseUOM = g.First().ProductCodeNavigation.BaseUom!
                })
                .OrderBy(x => x.ProductCode)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Gói kết quả vào PagedResponse
            var pagedResponse = new PagedResponse<InventorySummaryDTO>(result, page, pageSize, totalItems);

            return new ServiceResponse<PagedResponse<InventorySummaryDTO>>(true, "Lấy danh sách thành công", pagedResponse);
        }

        public async Task<ServiceResponse<bool>> UpdateInventoryAsync(InventoryRequestDTO inventoryRequestDTO)
        {
            if(inventoryRequestDTO == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var existingInventory = await _inventoryRepository.GetInventoryWithCode(inventoryRequestDTO.WarehouseCode, inventoryRequestDTO.LocationCode, inventoryRequestDTO.ProductCode, inventoryRequestDTO.LotNo);
            if(existingInventory == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy tồn kho");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    existingInventory.Quantity += inventoryRequestDTO.Quantity;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Cập nhật thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("unique") || error.Contains("duplicate") || error.Contains("primary key"))
                        {
                            return new ServiceResponse<bool>(false, "Dữ liệu đã tồn tại");
                        }
                        if (error.Contains("foreign key"))
                        {
                            return new ServiceResponse<bool>(false, "Dữ liệu tham chiếu không đúng");
                        }
                    }
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật tồn kho: {ex.Message}");
                }
            }
        }
    }
}
