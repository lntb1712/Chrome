using Chrome.DTO;
using Chrome.DTO.LocationMasterDTO;
using Chrome.Models;
using Chrome.Repositories.LocationMasterRepository;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Services.LocationMasterService
{
    public class LocationMasterService : ILocationMasterService
    {
        private readonly ILocationMasterRepository _locationMasterRepository;
        private readonly ChromeContext _context;
        public LocationMasterService(ILocationMasterRepository locationMasterRepository, ChromeContext context)
        {
            _locationMasterRepository = locationMasterRepository;
            _context = context;
        }
        public async Task<ServiceResponse<bool>> AddLocationMaster(LocationMasterRequestDTO locationMasterRequestDTO)
        {
            if(locationMasterRequestDTO == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu không hợp lệ");
            }
            var locationMaster = new LocationMaster
            {
                LocationCode = locationMasterRequestDTO.LocationCode,
                LocationName = locationMasterRequestDTO.LocationName,
                WarehouseCode = locationMasterRequestDTO.WarehouseCode,
                StorageProductId = locationMasterRequestDTO.StorageProductId,
                IsEmpty = true // Mặc định là true nếu không có giá trị
            };
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _locationMasterRepository.AddAsync(locationMaster,saveChanges:false);
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi thêm: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteLocationMaster(string warehouseCode,string locationCode)
        {
            if(string.IsNullOrEmpty(locationCode)||string.IsNullOrEmpty(warehouseCode))
            {
                return new ServiceResponse<bool>(false, "Mã kho và mã vị trí không được để trống");
            }
            var locationMaster = await _locationMasterRepository.GetLocationMasterWithCode(warehouseCode,locationCode);
            if(locationMaster == null)
            {
                return new ServiceResponse<bool>(false, "Vị trí không tồn tại");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _locationMasterRepository.DeleteAsync(locationCode,saveChanges:false);
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
                            return new ServiceResponse<bool>(false, "Không thể xóa vị trí vì có dữ liệu tham chiếu");
                        }
                    }
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xóa: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<PagedResponse<LocationMasterResponseDTO>>> GetAllLocationMaster(string warehouseCode,int page, int pageSize)
        {
            if(page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<LocationMasterResponseDTO>>(false, "Trang hoặc kích thước trang không hợp lệ");
            }
            var totalCount = await _locationMasterRepository.GetTotalLocationMasterCount(warehouseCode);
            var locationMasters = await _locationMasterRepository.GetAllLocationMaster(warehouseCode,page, pageSize);
            if(locationMasters == null || !locationMasters.Any())
            {
                return new ServiceResponse<PagedResponse<LocationMasterResponseDTO>>(false, "Không có dữ liệu vị trí");
            }
            var locationMasterResponses = new List<LocationMasterResponseDTO>();
            foreach(var locationMaster in locationMasters)
            {
                bool isEmpty = locationMaster.Inventories
                         .Sum(x => x.Quantity / (x.ProductCodeNavigation.BaseQuantity)) <= locationMaster.StorageProduct!.MaxQuantity;

                var response = new LocationMasterResponseDTO
                {
                    LocationCode = locationMaster.LocationCode,
                    LocationName = locationMaster.LocationName,
                    WarehouseCode = locationMaster.WarehouseCodeNavigation?.WarehouseCode,
                    WarehouseName = locationMaster.WarehouseCodeNavigation!.WarehouseName,
                    StorageProductId = locationMaster.StorageProductId,
                    StorageProductName = locationMaster.StorageProduct!.StorageProductName,
                    IsEmpty = isEmpty
                };
                locationMasterResponses.Add(response);
            }
            var pagedResponse = new PagedResponse<LocationMasterResponseDTO>(locationMasterResponses, page, pageSize, totalCount);
            return new ServiceResponse<PagedResponse<LocationMasterResponseDTO>>(true, "Lấy dữ liệu thành công",pagedResponse);
        }

        public async Task<ServiceResponse<LocationMasterResponseDTO>> GetLocationMasterWithCode(string warehouseCode, string locationCode)
        {
            if (string.IsNullOrEmpty(locationCode))
            {
                return new ServiceResponse<LocationMasterResponseDTO>(false, "Mã vị trí không hợp lệ");
            }
            if (string.IsNullOrEmpty(warehouseCode))
            {
                return new ServiceResponse<LocationMasterResponseDTO>(false, "Mã kho không hợp lệ");
            }    
            var locationMaster = await _locationMasterRepository.GetLocationMasterWithCode(warehouseCode, locationCode);
            if(locationMaster == null)
            {
                return new ServiceResponse<LocationMasterResponseDTO>(false, "Vị trí không tồn tại");
            }
            bool isEmpty = locationMaster.Inventories
                        .Sum(x => x.Quantity / (x.ProductCodeNavigation.BaseQuantity)) <= locationMaster.StorageProduct!.MaxQuantity;
            var locationMasterResponse = new LocationMasterResponseDTO
            {
                LocationCode = locationMaster.LocationCode,
                LocationName = locationMaster.LocationName,
                WarehouseCode = locationMaster.WarehouseCodeNavigation?.WarehouseCode,
                WarehouseName = locationMaster.WarehouseCodeNavigation!.WarehouseName,
                StorageProductId = locationMaster.StorageProductId,
                StorageProductName = locationMaster.StorageProduct!.StorageProductName,
                IsEmpty = isEmpty
            };
            return new ServiceResponse<LocationMasterResponseDTO>(true, "Lấy dữ liệu thành công", locationMasterResponse);
        }

        public async Task<ServiceResponse<int>> GetTotalLocationMasterCount(string warehouseCode)
        {
            try
            {
                var totalCount = await _locationMasterRepository.GetTotalLocationMasterCount(warehouseCode);
                return new ServiceResponse<int>(true, "Lấy tổng số lượng vị trí thành công", totalCount);
            }
            catch
            {
                return new ServiceResponse<int>(false, "Lỗi khi lấy tổng số lượng vị trí");
            }
        }

        public async Task<ServiceResponse<bool>> UpdateLocationMaster(LocationMasterRequestDTO locationMasterRequestDTO)
        {
            if(locationMasterRequestDTO== null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu không hợp lệ");
            }    
            var existingLocationMaster = await _locationMasterRepository.GetLocationMasterWithCode(locationMasterRequestDTO.WarehouseCode!,locationMasterRequestDTO.LocationCode);
            if(existingLocationMaster == null)
            {
                return new ServiceResponse<bool>(false, "Vị trí không tồn tại");
            }
            using(var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    existingLocationMaster.LocationName = locationMasterRequestDTO.LocationName;
                    existingLocationMaster.WarehouseCode = locationMasterRequestDTO.WarehouseCode;
                    existingLocationMaster.StorageProductId = locationMasterRequestDTO.StorageProductId;
                    existingLocationMaster.IsEmpty = existingLocationMaster.Inventories
                        .Sum(x => x.Quantity / (x.ProductCodeNavigation.BaseQuantity)) <= existingLocationMaster.StorageProduct!.MaxQuantity;
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật vị trí: {ex.Message}");
                }
            }
        }
    }
}
