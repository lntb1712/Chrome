using Chrome.DTO;
using Chrome.DTO.BOMMasterDTO;
using Chrome.Models;
using Chrome.Repositories.BOMMasterRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Chrome.Services.BOMMasterService
{
    public class BOMMasterService:IBOMMasterService
    {
        private readonly IBOMMasterRepository _bomMasterRepository;
        private readonly ChromeContext _context;

        public BOMMasterService(IBOMMasterRepository bomMasterRepository, ChromeContext context)
        {
            _bomMasterRepository = bomMasterRepository;
            _context = context;
        }

        public async Task<ServiceResponse<bool>> AddBOMMaster(BOMMasterRequestDTO bomMasterRequestDTO)
        {
            if(bomMasterRequestDTO==null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu BOM Master không hợp lệ");
            }
            var bomMaster = new Bommaster
            {
                Bomcode = bomMasterRequestDTO.BOMCode,
                ProductCode = bomMasterRequestDTO.ProductCode,
                Bomversion = bomMasterRequestDTO.BOMVersion,
                IsActive = false
            };
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _bomMasterRepository.AddAsync(bomMaster, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Thêm BOM Master thành công");
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

        public async Task<ServiceResponse<bool>> DeleteBOMMaster(string bomCode, string bomVersion)
        {
            if(string.IsNullOrEmpty(bomCode) || string.IsNullOrEmpty(bomVersion))
            {
                return new ServiceResponse<bool>(false, "Mã BOM và phiên bản không được để trống");
            }
            var bomMaster = await _bomMasterRepository.GetBOMMasterByCode(bomCode, bomVersion);
            if(bomMaster == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy BOM Master với mã và phiên bản đã cho");
            }
            using(var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    Expression<Func<Bommaster, bool>> predicate = x => x.Bomcode == bomCode && x.Bomversion ==bomVersion;
                    await _bomMasterRepository.DeleteFirstByConditionAsync(predicate,saveChanges:false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa BOM Master thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("foreign key"))
                        {
                            return new ServiceResponse<bool>(false, "Không thể xóa BOM Master vì có dữ liệu tham chiếu");
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

        public async Task<ServiceResponse<PagedResponse<BOMMasterResponseDTO>>> GetAllBOMMaster(int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<BOMMasterResponseDTO>>(false, "Trang và kích thước trang không hợp lệ");
            }

            var bomMasters = await _bomMasterRepository.GetAllBOMMaster(page, pageSize);
            var totalCount = await _bomMasterRepository.GetTotalBOMMasterCount();

            if (bomMasters == null || !bomMasters.Any())
            {
                return new ServiceResponse<PagedResponse<BOMMasterResponseDTO>>(false, "Không tìm thấy dữ liệu BOM Master");
            }

            // Map từng bomMaster -> BOMMasterResponseDTO (dùng Task.WhenAll để đợi toàn bộ)
            var bomMasterResponses = new List<BOMMasterResponseDTO>();

            foreach (var bom in bomMasters)
            {
                var versions = await _bomMasterRepository.GetListVersionByBomCode(bom.Bomcode);
                var response = new BOMMasterResponseDTO
                {
                    BOMCode = bom.Bomcode,
                    ProductCode = bom.ProductCode,
                    ProductName = bom.ProductCodeNavigation!.ProductName!,
                    BOMVersionResponses = versions.Select(v => new BOMVersionResponseDTO
                    {
                        BOMVersion = v.Bomversion,
                        IsActive = v.IsActive
                    }).ToList()
                };
                bomMasterResponses.Add(response);
            }


            var pagedResponse = new PagedResponse<BOMMasterResponseDTO>(bomMasterResponses, page, pageSize, totalCount);

            return new ServiceResponse<PagedResponse<BOMMasterResponseDTO>>(true, "Lấy dữ liệu thành công", pagedResponse);
        }

        public async Task<ServiceResponse<BOMMasterResponseDTO>> GetBOMMasterByCode(string bomCode, string bomVersion)
        {
            if(string.IsNullOrEmpty(bomCode) || string.IsNullOrEmpty(bomVersion))
            {
                return new ServiceResponse<BOMMasterResponseDTO>(false, "Mã BOM và phiên bản không được để trống");
            }
            var bomMaster = await _bomMasterRepository.GetBOMMasterByCode(bomCode, bomVersion);
            if(bomMaster == null)
            {
                return new ServiceResponse<BOMMasterResponseDTO>(false, "Không tìm thấy BOM Master với mã và phiên bản đã cho");
            }
            var versions = await _bomMasterRepository.GetListVersionByBomCode(bomCode);
            var bomMasterResponse = new BOMMasterResponseDTO
            {
                BOMCode = bomMaster.Bomcode,
                ProductCode = bomMaster.ProductCode,
                ProductName = bomMaster.ProductCodeNavigation!.ProductName!,
                BOMVersionResponses = versions.Select(v => new BOMVersionResponseDTO
                {
                    BOMVersion = v.Bomversion,
                    IsActive = v.IsActive
                }).ToList()
            };
            return new ServiceResponse<BOMMasterResponseDTO>(true, "Lấy dữ liệu BOM Master thành công", bomMasterResponse);
        }

        public async Task<ServiceResponse<int>> GetTotalBOMMasterCount()
        {
            try
            {
                var totalCount = await _bomMasterRepository.GetTotalBOMMasterCount();
                return new ServiceResponse<int>(true, "Lấy tổng số BOM Master thành công", totalCount);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, $"Lỗi khi lấy tổng số BOM Master: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<BOMMasterResponseDTO>>> SearchBOMMaster(string textToSearch, int page, int pageSize)
        {
            if(string.IsNullOrEmpty(textToSearch) || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<BOMMasterResponseDTO>>(false, "Dữ liệu tìm kiếm không hợp lệ");
            }
            var bomMasters = await  _bomMasterRepository.SearchBOMMaster(textToSearch, page, pageSize);
            var totalCount = await _bomMasterRepository.GetTotalSearchBOMMasterCount(textToSearch);
            if (bomMasters == null || !bomMasters.Any())
            {
                return new ServiceResponse<PagedResponse<BOMMasterResponseDTO>>(false, "Không tìm thấy BOM Master phù hợp với từ khóa tìm kiếm");
            }
            var bomMasterResponses = new List<BOMMasterResponseDTO>();
            foreach (var bom in bomMasters)
            {
                var versions = await _bomMasterRepository.GetListVersionByBomCode(bom.Bomcode);
                var response = new BOMMasterResponseDTO
                {
                    BOMCode = bom.Bomcode,
                    ProductCode = bom.ProductCode,
                    ProductName = bom.ProductCodeNavigation!.ProductName!,
                    BOMVersionResponses = versions.Select(v => new BOMVersionResponseDTO
                    {
                        BOMVersion = v.Bomversion,
                        IsActive = v.IsActive
                    }).ToList()
                };
                bomMasterResponses.Add(response);
            }
            var pagedResponse = new PagedResponse<BOMMasterResponseDTO>(bomMasterResponses, page, pageSize, totalCount);
            return new ServiceResponse<PagedResponse<BOMMasterResponseDTO>>(true, "Tìm kiếm BOM Master thành công", pagedResponse);
        }

        public async Task<ServiceResponse<bool>> UpdateBOMMaster(BOMMasterRequestDTO bomMasterRequestDTO)
        {
            if(bomMasterRequestDTO ==null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu BOM Master không hợp lệ");
            }    
            var existingBomMaster = await _bomMasterRepository.GetBOMMasterByCode(bomMasterRequestDTO.BOMCode, bomMasterRequestDTO.BOMVersion);
            if(existingBomMaster == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy BOM Master với mã và phiên bản đã cho");
            }
            using(var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    existingBomMaster.ProductCode = bomMasterRequestDTO.ProductCode;
                    existingBomMaster.Bomversion = bomMasterRequestDTO.BOMVersion;
                    existingBomMaster.IsActive = bomMasterRequestDTO.IsActive; // Giả sử bạn muốn đặt IsActive là false khi cập nhật
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Cập nhật BOM Master thành công");
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
