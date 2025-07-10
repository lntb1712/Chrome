using Chrome.DTO;
using Chrome.DTO.ProductMasterDTO;
using Chrome.DTO.ReplenishDTO;
using Chrome.Models;
using Chrome.Repositories.ProductMasterRepository;
using Chrome.Repositories.ReplenishRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Chrome.Services.ReplenishService
{
    public class ReplenishService : IReplenishService
    {
        private readonly IReplenishRepository _replenishRepository;
        private readonly IProductMasterRepository _productMasterRepository;
        private readonly ChromeContext _context;
        
        public ReplenishService(IReplenishRepository replenishRepository,IProductMasterRepository productMasterRepository, ChromeContext context)
        {
            _replenishRepository = replenishRepository;
            _productMasterRepository = productMasterRepository;
            _context = context;
        }

        public async Task<ServiceResponse<bool>> AddReplenishAsync(ReplenishRequestDTO replenishRequestDTO)
        {
            if(replenishRequestDTO == null)
            {
                return new ServiceResponse<bool>(false,"Dữ liệu nhận vào không hợp lệ");
            }

            var replenish = new Replenish
            {
                ProductCode = replenishRequestDTO.ProductCode,
                WarehouseCode = replenishRequestDTO.WarehouseCode,
                MinQuantity = replenishRequestDTO.MinQuantity,
                MaxQuantity = replenishRequestDTO.MaxQuantity
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _replenishRepository.AddAsync(replenish,saveChanges:false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Thêm thành công");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi thêm dữ liệu: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteReplenishAsync(string productCode, string warehouseCode)
        {
            if(string.IsNullOrEmpty(productCode) || string.IsNullOrEmpty(warehouseCode))
            {
                return new ServiceResponse<bool>(false, "Mã sản phẩm hoặc mã kho không hợp lệ");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    Expression<Func<Replenish, bool>> predicate = r => r.ProductCode == productCode && r.WarehouseCode == warehouseCode;
                    await _replenishRepository.DeleteFirstByConditionAsync(predicate, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("foreign key"))
                        {
                            return new ServiceResponse<bool>(false, "Luật bổ sung đang được sử dụng và không thể xóa");
                        }
                    }
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xóa dữ liệu: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<PagedResponse<ReplenishResponseDTO>>> GetAllReplenishAsync(string warehouseCode, int page, int pageSize)
        {
            if(string.IsNullOrEmpty(warehouseCode))
            {
                return new ServiceResponse<PagedResponse<ReplenishResponseDTO>>(false, "Mã kho không hợp lệ");
            }
          
            var query = _replenishRepository.GetAllReplenishAsync(warehouseCode);
            var totalCount = await query.CountAsync();
            var replenishList = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new ReplenishResponseDTO
                {
                    ProductCode = r.ProductCode,
                    WarehouseCode = r.WarehouseCode,
                    MinQuantity = r.MinQuantity,
                    MaxQuantity = r.MaxQuantity,
                    ProductName = r.ProductCodeNavigation!.ProductName! ,
                    WarehouseName = r.WarehouseCodeNavigation!.WarehouseName!,
                    TotalOnHand = (float)(r.ProductCodeNavigation.Inventories.Where(t => t.ProductCode == r.ProductCode).Sum(i => i.Quantity) ?? 0.00)/r.ProductCodeNavigation.BaseQuantity,
                })
                .ToListAsync();
            
            var pagedResponse = new PagedResponse<ReplenishResponseDTO>(replenishList,page, pageSize, totalCount);
            return new ServiceResponse<PagedResponse<ReplenishResponseDTO>>(true, "Lấy dữ liệu thành công",pagedResponse);
        }

        public async Task<ServiceResponse<ReplenishResponseDTO>> GetReplenishByCodeAsync(string productCode, string warehouseCode)
        {
            if(string.IsNullOrEmpty(productCode) || string.IsNullOrEmpty(warehouseCode))
            {
                return new ServiceResponse<ReplenishResponseDTO>(false, "Mã sản phẩm hoặc mã kho không hợp lệ");
            }

            var replenish = await _replenishRepository.GetReplenishByCode(productCode, warehouseCode);
            if (replenish == null)
            {
                return new ServiceResponse<ReplenishResponseDTO>(false, "Không tìm thấy dữ liệu bổ sung cho sản phẩm và kho này");
            }
            var response = new ReplenishResponseDTO
            {
                ProductCode = replenish.ProductCode,
                WarehouseCode = replenish.WarehouseCode,
                MinQuantity = replenish.MinQuantity,
                MaxQuantity = replenish.MaxQuantity,
                ProductName = replenish.ProductCodeNavigation!.ProductName!,
                WarehouseName = replenish.WarehouseCodeNavigation!.WarehouseName!,
                TotalOnHand = (float)(replenish.ProductCodeNavigation.Inventories.Where(t => t.ProductCode == replenish.ProductCode).Sum(i => i.Quantity) ?? 0.00)/replenish.ProductCodeNavigation.BaseQuantity,
            };
            return new ServiceResponse<ReplenishResponseDTO>(true, "Lấy dữ liệu thành công", response);
        }

        public async Task<ServiceResponse<int>> GetTotalReplenishCountAsync(string warehouseCode)
        {
            if(string.IsNullOrEmpty(warehouseCode))
            {
                return new ServiceResponse<int>(false, "Mã kho không hợp lệ");
            }

            var query = _replenishRepository.GetAllReplenishAsync(warehouseCode);
            var totalCount = await query.CountAsync();
            return new ServiceResponse<int>(true, "Lấy tổng số lượng bổ sung thành công", totalCount);
        }

        public async Task<ServiceResponse<PagedResponse<ReplenishResponseDTO>>> SearchReplenishAsync(string warehouseCode, string textToSearch, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(warehouseCode) || string.IsNullOrEmpty(textToSearch))
            {
                return new ServiceResponse<PagedResponse<ReplenishResponseDTO>>(false, "Mã kho hoặc từ khóa tìm kiếm không hợp lệ");
            }

            var query = _replenishRepository.SearchReplenishAsync(warehouseCode, textToSearch);
            var totalCount = await query.CountAsync();
            var replenishList = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new ReplenishResponseDTO
                {
                    ProductCode = r.ProductCode,
                    WarehouseCode = r.WarehouseCode,
                    MinQuantity = r.MinQuantity,
                    MaxQuantity = r.MaxQuantity,
                    ProductName = r.ProductCodeNavigation!.ProductName!,
                    WarehouseName = r.WarehouseCodeNavigation!.WarehouseName!,
                    TotalOnHand = (float)(r.ProductCodeNavigation.Inventories.Where(t => t.ProductCode == r.ProductCode).Sum(i => i.Quantity) ?? 0.00)/r.ProductCodeNavigation.BaseQuantity,
                })
                .ToListAsync();
            var pagedResponse = new PagedResponse<ReplenishResponseDTO>(replenishList, page, pageSize, totalCount);
            return new ServiceResponse<PagedResponse<ReplenishResponseDTO>>(true, "Tìm kiếm thành công", pagedResponse);
        }

        public async Task<ServiceResponse<bool>> UpdateReplenishAsync(ReplenishRequestDTO replenishRequestDTO)
        {
            if (replenishRequestDTO == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu bổ sung không hợp lệ");
            }

            var existingReplenish = await _replenishRepository.GetReplenishByCode(replenishRequestDTO.ProductCode, replenishRequestDTO.WarehouseCode);
            if (existingReplenish == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy dữ liệu bổ sung để cập nhật");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    existingReplenish.MinQuantity = replenishRequestDTO.MinQuantity;
                    existingReplenish.MaxQuantity = replenishRequestDTO.MaxQuantity;
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật dữ liệu: {ex.Message}");
                }
            }
        }
        public async Task<ServiceResponse<List<string>>> CheckReplenishWarningsAsync(string warehouseCode)
        {
            if (string.IsNullOrWhiteSpace(warehouseCode))
            {
                return new ServiceResponse<List<string>>(false, "Mã kho không hợp lệ");
            }

            try
            {
                var result = new List<string>();

                var replenishes = await _replenishRepository
                    .GetAllReplenishAsync(warehouseCode)
                    .Select(r => new
                    {
                        r.ProductCode,
                        r.ProductCodeNavigation.ProductName,
                        r.WarehouseCode,
                        r.MinQuantity,
                        r.MaxQuantity,
                        TotalOnHand = (float)(r.ProductCodeNavigation.Inventories
                                            .Where(i => i.ProductCode == r.ProductCode)
                                            .Sum(i => i.Quantity) ?? 0.00f) / r.ProductCodeNavigation.BaseQuantity
                    })
                    .ToListAsync();

                foreach (var r in replenishes)
                {
                    if (r.TotalOnHand < r.MinQuantity)
                    {
                        result.Add($"[Thiếu hàng] {r.ProductName} (Mã: {r.ProductCode})  \nHiện có: {r.TotalOnHand} < Ngưỡng thấp nhất: {r.MinQuantity}");
                    }
                    else if (r.TotalOnHand > r.MaxQuantity)
                    {
                        result.Add($"[Dư hàng] {r.ProductName} (Mã: {r.ProductCode}) \nHiện có: {r.TotalOnHand} > Ngưỡng cao nhất: {r.MaxQuantity}");
                    }
                }

                return new ServiceResponse<List<string>>(true, "Kiểm tra cảnh báo bổ sung thành công", result);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<string>>(false, $"Lỗi khi kiểm tra bổ sung: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<List<ProductMasterResponseDTO>>> GetListProductToReplenish()
        {
            var lstProduct = await _productMasterRepository.GetAllProduct(1, int.MaxValue);
            var lstProductForReplenish = lstProduct
                                            .Select(p => new ProductMasterResponseDTO
                                            {
                                                ProductCode = p.ProductCode,
                                                ProductName = p.ProductName,
                                                ProductDescription = p.ProductDescription,
                                                ProductImage = p.ProductImage,
                                                CategoryId = p.CategoryId,
                                                CategoryName = p.Category?.CategoryName ?? "Không có danh mục",
                                                BaseQuantity = p.BaseQuantity,
                                                Uom = p.Uom,
                                                BaseUom = p.BaseUom,
                                                Valuation = (float?)p.Valuation,
                                            }).ToList();
            return new ServiceResponse<List<ProductMasterResponseDTO>>(true, "Lấy danh sách sản phẩm thành công", lstProductForReplenish);
        }
    }
}
