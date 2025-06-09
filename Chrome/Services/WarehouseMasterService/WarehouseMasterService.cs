using Chrome.DTO;
using Chrome.DTO.WarehouseMasterDTO;
using Chrome.Models;
using Chrome.Repositories.WarehouseMasterRepository;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Services.WarehouseMasterService
{
    public class WarehouseMasterService : IWarehouseMasterService
    {
        private readonly IWarehouseMasterRepository _warehouseMasterRepository;
        private readonly ChromeContext _context;

        public WarehouseMasterService(IWarehouseMasterRepository warehouseMasterRepository, ChromeContext context)
        {
            _warehouseMasterRepository = warehouseMasterRepository;
            _context = context;
        }

        public async Task<ServiceResponse<bool>> AddWarehouseMaster(WarehouseMasterRequestDTO warehouse)
        {
            if (warehouse == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ.");
            }
            var warehouseMaster = new WarehouseMaster
            {
                WarehouseCode = warehouse.WarehouseCode,
                WarehouseName = warehouse.WarehouseName,
                WarehouseDescription = warehouse.WarehouseDescription,
                WarehouseAddress = warehouse.WarehouseAddress,
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _warehouseMasterRepository.AddAsync(warehouseMaster, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Thêm kho thành công.");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi thêm kho: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteWarehouseMaster(string warehouseCode)
        {
            if (string.IsNullOrEmpty(warehouseCode))
            {
                return new ServiceResponse<bool>(false, "Mã kho không được để trống.");
            }
            var warehouseMaster = await _warehouseMasterRepository.GetWarehouseMasterWithCode(warehouseCode);
            if (warehouseMaster == null)
            {
                return new ServiceResponse<bool>(false, "Kho không tồn tại.");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _warehouseMasterRepository.DeleteAsync(warehouseCode, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa kho thành công.");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("foreign key"))
                        {
                            return new ServiceResponse<bool>(false, "Kho này đang được sử dụng và không thể xóa.");
                        }
                    }
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xóa kho: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<PagedResponse<WarehouseMasterResponseDTO>>> GetAllWarehouseMaster(int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<WarehouseMasterResponseDTO>>(false, "Trang và kích thước trang phải lớn hơn 0.");
            }
            var warehouseMasters = await _warehouseMasterRepository.GetWarehouseMasters(page, pageSize);
            if (warehouseMasters == null || !warehouseMasters.Any())
            {
                return new ServiceResponse<PagedResponse<WarehouseMasterResponseDTO>>(false, "Không có kho nào được tìm thấy.");
            }
            var totalCount = await _warehouseMasterRepository.GetTotalWarehouse();
            var warehouseMasterResponses = warehouseMasters.Select(warehouse => new WarehouseMasterResponseDTO
            {
                WarehouseCode = warehouse.WarehouseCode,
                WarehouseName = warehouse.WarehouseName,
                WarehouseDescription = warehouse.WarehouseDescription,
                WarehouseAddress = warehouse.WarehouseAddress,
            }).ToList();
            var pagedResponse = new PagedResponse<WarehouseMasterResponseDTO>(warehouseMasterResponses, page, pageSize, totalCount);
            return new ServiceResponse<PagedResponse<WarehouseMasterResponseDTO>>(true, "Lấy danh sách kho thành công.", pagedResponse);
        }

        public async Task<ServiceResponse<int>> GetTotalWarehouseCount()
        {
            try
            {
                var totalCount = await _warehouseMasterRepository.GetTotalWarehouse();
                return new ServiceResponse<int>(true, "Lấy tổng số kho thành công.", totalCount);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, $"Lỗi khi lấy tổng số kho: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<WarehouseMasterResponseDTO>> GetWarehouseMasterWithCode(string warehouseCode)
        {
            if (string.IsNullOrEmpty(warehouseCode))
            {
                return new ServiceResponse<WarehouseMasterResponseDTO>(false, "Mã kho không được để trống.");
            }
            var warehouseMaster = await _warehouseMasterRepository.GetWarehouseMasterWithCode(warehouseCode);
            if (warehouseMaster == null)
            {
                return new ServiceResponse<WarehouseMasterResponseDTO>(false, "Kho không tồn tại.");
            }
            var response = new WarehouseMasterResponseDTO
            {
                WarehouseCode = warehouseMaster.WarehouseCode,
                WarehouseName = warehouseMaster.WarehouseName,
                WarehouseDescription = warehouseMaster.WarehouseDescription,
                WarehouseAddress = warehouseMaster.WarehouseAddress,
            };
            return new ServiceResponse<WarehouseMasterResponseDTO>(true, "Lấy thông tin kho thành công.", response);
        }

        public async Task<ServiceResponse<PagedResponse<WarehouseMasterResponseDTO>>> SearchWarehouse(string textToSearch, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(textToSearch) || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<WarehouseMasterResponseDTO>>(false, "Từ khóa tìm kiếm, trang và kích thước trang phải hợp lệ.");
            }
            var warehouses = await _warehouseMasterRepository.SearchWarehouse(textToSearch, page, pageSize);
            if (warehouses == null || !warehouses.Any())
            {
                return new ServiceResponse<PagedResponse<WarehouseMasterResponseDTO>>(false, "Không tìm thấy kho nào phù hợp với từ khóa tìm kiếm.");
            }
            var totalCount = await _warehouseMasterRepository.GetTotalSearchCount(textToSearch);
            var warehouseResponses = warehouses.Select(warehouse => new WarehouseMasterResponseDTO
            {
                WarehouseCode = warehouse.WarehouseCode,
                WarehouseName = warehouse.WarehouseName,
                WarehouseDescription = warehouse.WarehouseDescription,
                WarehouseAddress = warehouse.WarehouseAddress,
            }).ToList();
            var pagedResponse = new PagedResponse<WarehouseMasterResponseDTO>(warehouseResponses, page, pageSize, totalCount);
            return new ServiceResponse<PagedResponse<WarehouseMasterResponseDTO>>(true, "Tìm kiếm kho thành công.", pagedResponse);
        }

        public async Task<ServiceResponse<bool>> UpdateWarehouseMaster(WarehouseMasterRequestDTO warehouse)
        {
            if (warehouse == null || string.IsNullOrEmpty(warehouse.WarehouseCode))
            {
                return new ServiceResponse<bool>(false, "Dữ liệu kho không hợp lệ.");
            }
            var existingWarehouse = await _warehouseMasterRepository.GetWarehouseMasterWithCode(warehouse.WarehouseCode);
            if (existingWarehouse == null)
            {
                return new ServiceResponse<bool>(false, "Kho không tồn tại.");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    existingWarehouse.WarehouseName = warehouse.WarehouseName;
                    existingWarehouse.WarehouseDescription = warehouse.WarehouseDescription;
                    existingWarehouse.WarehouseAddress = warehouse.WarehouseAddress;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Cập nhật kho thành công.");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật kho: {ex.Message}");
                }
            }
        }
    }
}
