using Azure;
using Chrome.DTO;
using Chrome.DTO.InventoryDTO;
using Chrome.Models;
using Chrome.Repositories.InventoryRepository;
using Chrome.Repositories.ProductMasterRepository;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Chrome.Services.InventoryService
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IProductMasterRepository _productMasterRepository;
        private readonly ChromeContext _context;

        public InventoryService(
            IInventoryRepository inventoryRepository,
            IProductMasterRepository productMasterRepository,
            ChromeContext context)
        {
            _inventoryRepository = inventoryRepository;
            _productMasterRepository = productMasterRepository;
            _context = context;
        }

        public async Task<ServiceResponse<bool>> AddInventory(InventoryRequestDTO inventoryRequestDTO, bool saveChanges = true)
        {
            if (inventoryRequestDTO == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            // Lấy thông tin sản phẩm để quy đổi đơn vị
            var product = await _productMasterRepository.GetProductMasterWithProductCode(inventoryRequestDTO.ProductCode);
            if (product == null)
            {
                return new ServiceResponse<bool>(false, "Sản phẩm không tồn tại");
            }

            var quantityInBaseUOM = inventoryRequestDTO.Quantity ;

            var inventory = new Inventory
            {
                LocationCode = inventoryRequestDTO.LocationCode,
                ProductCode = inventoryRequestDTO.ProductCode,
                Lotno = inventoryRequestDTO.LotNo,
                Quantity = quantityInBaseUOM, // Lưu theo mét
                ReceiveDate = DateTime.Now
            };

            try
            {
                await _inventoryRepository.AddAsync(inventory, saveChanges: false);
                if (saveChanges)
                {
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                }
                return new ServiceResponse<bool>(true, "Thêm tồn kho thành công");
            }
            catch (DbUpdateException dbEx)
            {
                if (saveChanges)
                {
                    // Rollback only if we started the transaction
                    var transaction = _context.Database.CurrentTransaction;
                    if (transaction != null)
                    {
                        await transaction.RollbackAsync();
                    }
                }
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
                if (saveChanges)
                {
                    var transaction = _context.Database.CurrentTransaction;
                    if (transaction != null)
                    {
                        await transaction.RollbackAsync();
                    }
                }
                return new ServiceResponse<bool>(false, $"Lỗi khi thêm tồn kho: {ex.Message}");
            }
        }
        public async Task<ServiceResponse<bool>> DeleteInventoryAsync(string warehouseCode, string locationCode, string productCode, string lotNo)
        {
            if (string.IsNullOrEmpty(warehouseCode) || string.IsNullOrEmpty(locationCode) || string.IsNullOrEmpty(productCode) || string.IsNullOrEmpty(lotNo))
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không được để trống");
            }

            var inventory = await _inventoryRepository.GetInventoryWithCode(locationCode, productCode, lotNo);
            if (inventory == null)
            {
                return new ServiceResponse<bool>(false, "Tồn kho không tồn tại");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    Expression<Func<Inventory, bool>> predicate = x => x.LocationCode == locationCode && x.ProductCode == productCode && x.Lotno == lotNo;
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

        public async Task<ServiceResponse<List<WarehouseUsageDTO>>> GetInventoryUsedPercent(string[] warehouseCodes)
        {
            if (warehouseCodes.Length == 0)
            {
                return new ServiceResponse<List<WarehouseUsageDTO>>(false, "Không có kho nào được cung cấp");
            }

            var query = from l in _context.LocationMasters
                        where warehouseCodes.Contains(l.WarehouseCode)
                        join i in _context.Inventories on l.LocationCode equals i.LocationCode into invGroup
                        from i in invGroup.DefaultIfEmpty()
                        join p in _context.ProductMasters on i.ProductCode equals p.ProductCode into prodGroup
                        from p in prodGroup.DefaultIfEmpty()
                        join sp in _context.StorageProducts on l.StorageProductId equals sp.StorageProductId into spGroup
                        from sp in spGroup.DefaultIfEmpty()
                        group new { l, i, p, sp } by new
                        {
                            l.WarehouseCode,
                            l.WarehouseCodeNavigation!.WarehouseName, // thêm nếu có navigation
                            l.LocationCode,
                            l.LocationName,
                            ProductCode = i != null ? i.ProductCode : null,
                            ProductName = p != null ? p.ProductName : "",
                            BaseQuantity = p != null ? p.BaseQuantity : 1,
                            MaxQuantity = sp != null ? sp.MaxQuantity : 0
                        } into g
                        select new
                        {
                            g.Key.WarehouseCode,
                            g.Key.WarehouseName,
                            Location = new LocationUsageDTO
                            {
                                LocationCode = g.Key.LocationCode,
                                LocationName = g.Key.LocationName,
                                ProductCode = g.Key.ProductCode,
                                ProductName = g.Key.ProductName,
                                Quantity = Math.Round((double)g.Sum(x => x.i != null ? x.i.Quantity  : 0)!, 2),
                                UsedPercentage = Math.Round((double)(g.Key.MaxQuantity > 0
                                    ? g.Sum(x => x.i != null ? x.i.Quantity  : 0) / g.Key.MaxQuantity * 100
                                    : 0)!, 2)
                            }
                        };

            // Group lại theo WarehouseCode
            var result = query
                .ToList()
                .GroupBy(x => new { x.WarehouseCode, x.WarehouseName })
                .Select(g => new WarehouseUsageDTO
                {
                    WarehouseCode = g.Key.WarehouseCode,
                    WarehouseName = g.Key.WarehouseName,
                    locationUsageDTOs = g.Select(x => x.Location).ToList()
                })
                .ToList();

            return new ServiceResponse<List<WarehouseUsageDTO>>(true, "Lấy danh sách phần trăm sử dụng thành công", result);
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
                    BaseQuantity = g.Sum(x => x.Quantity * (x.ProductCodeNavigation.BaseQuantity)),
                    UOM = g.First().ProductCodeNavigation.Uom!,
                    BaseUOM = g.First().ProductCodeNavigation.BaseUom!,
                    TotalPrice = g.Sum(x => x.Quantity) * g.First().ProductCodeNavigation.Valuation

                })
                .OrderBy(x => x.ProductCode)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Gói kết quả vào PagedResponse
            var pagedResponse = new PagedResponse<InventorySummaryDTO>(result, page, pageSize, totalItems);

            return new ServiceResponse<PagedResponse<InventorySummaryDTO>>(true, "Lấy danh sách thành công", pagedResponse);
        }

        public async Task<ServiceResponse<PagedResponse<InventorySummaryDTO>>> GetListProductInventoryByCategoryIds(string[] warehouseCodes, string[] categoryIds, int page, int pageSize)
        {
            if (warehouseCodes.Length == 0 || categoryIds.Length == 0 || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<InventorySummaryDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            var query = _inventoryRepository.GetInventoriesByCategoryIds(warehouseCodes, categoryIds);

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
                    BaseQuantity = g.Sum(x => x.Quantity * (x.ProductCodeNavigation.BaseQuantity)),
                    UOM = g.First().ProductCodeNavigation.Uom!,
                    BaseUOM = g.First().ProductCodeNavigation.BaseUom!,
                    TotalPrice = g.Sum(x => x.Quantity) * g.First().ProductCodeNavigation.Valuation
                })
                .OrderBy(x => x.ProductCode)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Gói kết quả vào PagedResponse
            var pagedResponse = new PagedResponse<InventorySummaryDTO>(result, page, pageSize, totalItems);

            return new ServiceResponse<PagedResponse<InventorySummaryDTO>>(true, "Lấy danh sách thành công", pagedResponse);
        }

        public async Task<ServiceResponse<PagedResponse<ProductWithLocationsDTO>>> GetProductWithLocationsAsync(string[] warehouseCodes, string productCode, int page, int pageSize)
        {
            if (warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<ProductWithLocationsDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            var query = _inventoryRepository.GetInventories(warehouseCodes);

            var groupQuery = query
                .Where(x => x.ProductCode == productCode)
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
                            x.LocationCodeNavigation.WarehouseCode,
                            x.LocationCodeNavigation.WarehouseCodeNavigation!.WarehouseName,
                            x.LocationCode,
                            x.LocationCodeNavigation.LocationName,
                            x.ProductCodeNavigation.BaseQuantity,
                            x.ProductCodeNavigation.Uom,
                            x.ProductCodeNavigation.BaseUom
                        })
                        .Select(lg => new LocationDetailDTO
                        {
                            WarehouseCode = lg.Key.WarehouseCode!,
                            WarehouseName = lg.Key.WarehouseName!,
                            LocationCode = lg.Key.LocationCode,
                            LocationName = lg.Key.LocationName!,
                            Quantity = lg.Sum(x => x.Quantity),
                            BaseQuantity = lg.Sum(x => x.Quantity * (x.ProductCodeNavigation.BaseQuantity)),
                            UOM = lg.Key.Uom!,
                            BaseUOM = lg.Key.BaseUom!
                        })
                        .ToList()
                });

            var totalItems = await groupQuery.CountAsync();

            var items = await groupQuery
                .OrderBy(x => x.ProductCode)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pagedResponse = new PagedResponse<ProductWithLocationsDTO>(items, page, pageSize, totalItems);

            return new ServiceResponse<PagedResponse<ProductWithLocationsDTO>>(true, "Lấy danh sách thành công", pagedResponse);
        }

        public async Task<ServiceResponse<double>> GetTotalPriceOfWarehouse(string[] warehouseCodes)
        {
            if (warehouseCodes.Length == 0 )
            {
                return new ServiceResponse<double>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            var query = _inventoryRepository.GetInventories(warehouseCodes);

            double totalInventoryValue = (double)await query.SumAsync(x => x.Quantity * x.ProductCodeNavigation.Valuation);
            
            return new  ServiceResponse <double> (true, "Lấy tổng thành công", totalInventoryValue);
        }

        public async Task<ServiceResponse<PagedResponse<InventorySummaryDTO>>> SearchProductInventoryAsync(string[] warehouseCodes, string textToSearch, int page, int pageSize)
        {
            if (warehouseCodes.Length == 0 || string.IsNullOrEmpty(textToSearch) || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<InventorySummaryDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            var query = _inventoryRepository.SearchProductInventories(warehouseCodes, textToSearch);

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
                    BaseQuantity = g.Sum(x => x.Quantity * (x.ProductCodeNavigation.BaseQuantity)),
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

        public async Task<ServiceResponse<bool>> UpdateInventoryAsync(InventoryRequestDTO inventoryRequestDTO, bool saveChanges = true)
        {
            if (inventoryRequestDTO == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            var existingInventory = await _inventoryRepository.GetInventoryWithCode(
                inventoryRequestDTO.LocationCode,
                inventoryRequestDTO.ProductCode,
                inventoryRequestDTO.LotNo);
            if (existingInventory == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy tồn kho");
            }

            // Lấy thông tin sản phẩm để quy đổi đơn vị
            var product = await _productMasterRepository.GetProductMasterWithProductCode(inventoryRequestDTO.ProductCode);
            if (product == null)
            {
                return new ServiceResponse<bool>(false, "Sản phẩm không tồn tại");
            }

            // Quy đổi số lượng từ UOM (thanh) sang BaseUOM (mét)
            var quantityInBaseUOM = inventoryRequestDTO.Quantity ;

            try
            {   
                existingInventory.ReceiveDate = DateTime.Now;
                existingInventory.Quantity += quantityInBaseUOM; // Cập nhật theo mét
                await _inventoryRepository.UpdateAsync(existingInventory, saveChanges: false);
                if (saveChanges)
                {
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                }
                return new ServiceResponse<bool>(true, "Cập nhật thành công");
            }
            catch (DbUpdateException dbEx)
            {
                if (saveChanges)
                {
                    var transaction = _context.Database.CurrentTransaction;
                    if (transaction != null)
                    {
                        await transaction.RollbackAsync();
                    }
                }
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
                if (saveChanges)
                {
                    var transaction = _context.Database.CurrentTransaction;
                    if (transaction != null)
                    {
                        await transaction.RollbackAsync();
                    }
                }
                return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật tồn kho: {ex.Message}");
            }
        }
    }
}