using Chrome.DTO;
using Chrome.DTO.BOMComponentDTO;
using Chrome.Models;
using Chrome.Repositories.BOMComponentRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Chrome.Services.BOMComponentService
{
    public class BOMComponentService : IBOMComponentService
    {
        private readonly IBOMComponentRepository _bomComponentRepository;
        private readonly ChromeContext _context;

        public BOMComponentService(IBOMComponentRepository bomComponentRepository, ChromeContext context)
        {
            _context = context;
            _bomComponentRepository = bomComponentRepository;
        }

        public async Task<ServiceResponse<bool>> AddBomComponent(BOMComponentRequestDTO bomComponent)
        {
            if (bomComponent == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var bomComponentResponse = new BomComponent
            {
                Bomcode = bomComponent.BOMCode,
                BomVersion = bomComponent.BOMVersion,
                ComponentCode = bomComponent.ComponentCode,
                ConsumpQuantity = bomComponent.ConsumpQuantity,
                ScrapRate = bomComponent.ScrapRate,
            };
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _bomComponentRepository.AddAsync(bomComponentResponse, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Thêm định mức nguyên vật liệu thành công");
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
                    return new ServiceResponse<bool>(false, $"Lỗi không xác định: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteBomComponent(string bomCode, string componentCode, string bomVersion)
        {
            if (string.IsNullOrEmpty(bomCode) || string.IsNullOrEmpty(componentCode) || string.IsNullOrEmpty(bomVersion))
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var bomComponent = await _bomComponentRepository.GetBomComponent(bomCode, componentCode, bomVersion);
            if (bomComponent == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy định mức nguyên vật liệu để xóa");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    Expression<Func<BomComponent, bool>> predicate = x => x.Bomcode == bomCode && x.ComponentCode == componentCode && x.BomVersion == bomVersion;
                    await _bomComponentRepository.DeleteFirstByConditionAsync(predicate);
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
                            return new ServiceResponse<bool>(false, "Không thể xóa BOM Component vì có dữ liệu tham chiếu");
                        }
                    }
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi không xác định: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<List<BOMComponentResponseDTO>>> GetAllBOMComponent(string bomCode, string bomVersion)
        {
            if (string.IsNullOrEmpty(bomCode) || string.IsNullOrEmpty(bomVersion))
            {
                return new ServiceResponse<List<BOMComponentResponseDTO>>(false, "Mã bom và mã version không được để trống");
            }
            var lstBomComponents = await _bomComponentRepository.GetAllBOMComponent(bomCode, bomVersion);
            var lstBomComponentResponse = lstBomComponents.Select(x => new BOMComponentResponseDTO
            {
                BOMCode = bomCode,
                BOMVersion = bomVersion,
                ComponentCode = x.ComponentCode,
                ComponentName = x.ComponentCodeNavigation.ProductName!,
                ConsumpQuantity = x.ConsumpQuantity,
                ScrapRate = x.ScrapRate,


            }).ToList();
            return new ServiceResponse<List<BOMComponentResponseDTO>>(true, "Lấy danh sách định múc nguyên vật liệu thành công", lstBomComponentResponse);

        }

        public async Task<ServiceResponse<BOMComponentResponseDTO>> GetBOMComponent(string bomCode, string componentCode, string bomVersion)
        {
            if (string.IsNullOrEmpty(bomCode) || string.IsNullOrEmpty(componentCode) || string.IsNullOrEmpty(bomVersion))
            {
                return new ServiceResponse<BOMComponentResponseDTO>(false, "Dữ liệu nhận vào bị trống");
            }
            var bomComponent = await _bomComponentRepository.GetBomComponent(bomCode, componentCode, bomVersion);
            var bomResponse = new BOMComponentResponseDTO
            {
                BOMCode = bomComponent.Bomcode,
                BOMVersion = bomComponent.BomVersion,
                ComponentCode = bomComponent.ComponentCode,
                ComponentName = bomComponent.ComponentCodeNavigation.ProductName!,
                ConsumpQuantity = bomComponent.ConsumpQuantity,
                ScrapRate = bomComponent.ScrapRate
            };
            return new ServiceResponse<BOMComponentResponseDTO>(true, "Lấy thông tin định mức thành công", bomResponse);
        }

        public async Task<ServiceResponse<IEnumerable<BOMNodeDTO>>> GetRecursiveBOMAsync(string topLevelBOM, string topLevelVersion)
        {
            if (string.IsNullOrEmpty(topLevelBOM) || string.IsNullOrEmpty(topLevelVersion))
            {
                return new ServiceResponse<IEnumerable<BOMNodeDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var lstBomNodes = await _bomComponentRepository.GetRecursiveBOMAsync(topLevelBOM, topLevelVersion);
            return new ServiceResponse<IEnumerable<BOMNodeDTO>>(true, "Lấy dữ liệu BOM thành công", lstBomNodes);
        }

        public async Task<ServiceResponse<bool>> UpdateBomComponent(BOMComponentRequestDTO bomComponent)
        {
            if (bomComponent == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var existingBomComponent = await _bomComponentRepository.GetBomComponent(bomComponent.BOMCode, bomComponent.ComponentCode, bomComponent.BOMVersion);
            if (existingBomComponent == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy định mức nguyên vật liệu");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    existingBomComponent.ConsumpQuantity = bomComponent.ConsumpQuantity;
                    existingBomComponent.ScrapRate = bomComponent.ScrapRate;
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
                    return new ServiceResponse<bool>(false, $"Lỗi không xác định: {ex.Message}");
                }
            }
        }
    }
}