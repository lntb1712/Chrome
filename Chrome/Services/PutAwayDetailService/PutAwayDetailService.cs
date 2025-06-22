using Chrome.DTO;
using Chrome.DTO.PutAwayDetailDTO;
using Chrome.Models;
using Chrome.Repositories.PutAwayDetailRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Chrome.Services.PutAwayDetailService
{
    public class PutAwayDetailService : IPutAwayDetailService
    {
        private readonly IPutAwayDetailRepository _putAwayDetailRepository;
        private readonly ChromeContext _context;

        public PutAwayDetailService(IPutAwayDetailRepository putAwayDetailRepository, ChromeContext context)
        {
            _putAwayDetailRepository = putAwayDetailRepository ?? throw new ArgumentNullException(nameof(putAwayDetailRepository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ServiceResponse<PagedResponse<PutAwayDetailResponseDTO>>> GetAllPutAwayDetailsAsync(string[] warehouseCodes, int page = 1, int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _putAwayDetailRepository.GetAllPutAwayDetailsAsync(warehouseCodes);
                var totalItems = await query.CountAsync();
                var putAwayDetails = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(pd => new PutAwayDetailResponseDTO
                    {
                        PutAwayCode = pd.PutAwayCode,
                        ProductCode = pd.ProductCode,
                        ProductName = pd.ProductCodeNavigation!.ProductName!,
                        LotNo = pd.LotNo,
                        Demand = pd.Demand,
                        Quantity = pd.Quantity
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<PutAwayDetailResponseDTO>(putAwayDetails, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<PutAwayDetailResponseDTO>>(true, "Lấy danh sách chi tiết putaway thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<PutAwayDetailResponseDTO>>(false, $"Lỗi khi lấy danh sách chi tiết putaway: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<PutAwayDetailResponseDTO>>> GetPutAwayDetailsByPutawayCodeAsync(string putawayNo, int page = 1, int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(putawayNo))
                {
                    return new ServiceResponse<PagedResponse<PutAwayDetailResponseDTO>>(false, "Mã putaway không được để trống");
                }

                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _putAwayDetailRepository.GetPutAwayDetailsByPutawayNoAsync(putawayNo);
                var totalItems = await query.CountAsync();
                var putAwayDetails = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(pd => new PutAwayDetailResponseDTO
                    {
                        PutAwayCode = pd.PutAwayCode,
                        ProductCode = pd.ProductCode,
                        ProductName = pd.ProductCodeNavigation!.ProductName!,
                        LotNo = pd.LotNo,
                        Demand = pd.Demand,
                        Quantity = pd.Quantity
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<PutAwayDetailResponseDTO>(putAwayDetails, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<PutAwayDetailResponseDTO>>(true, "Lấy chi tiết putaway theo mã thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<PutAwayDetailResponseDTO>>(false, $"Lỗi khi lấy chi tiết putaway theo mã: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<PutAwayDetailResponseDTO>>> SearchPutAwayDetailsAsync(string[] warehouseCodes, string putawayNo, string textToSearch, int page = 1, int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _putAwayDetailRepository.SearchPutAwayDetailsAsync(warehouseCodes, putawayNo, textToSearch);
                var totalItems = await query.CountAsync();
                var putAwayDetails = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(pd => new PutAwayDetailResponseDTO
                    {
                        PutAwayCode = pd.PutAwayCode,
                        ProductCode = pd.ProductCode,
                        ProductName = pd.ProductCodeNavigation!.ProductName!,
                        LotNo = pd.LotNo,
                        Demand = pd.Demand,
                        Quantity = pd.Quantity
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<PutAwayDetailResponseDTO>(putAwayDetails, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<PutAwayDetailResponseDTO>>(true, "Tìm kiếm chi tiết putaway thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<PutAwayDetailResponseDTO>>(false, $"Lỗi khi tìm kiếm chi tiết putaway: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> UpdatePutAwayDetail(PutAwayDetailRequestDTO putAwayDetail)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (putAwayDetail == null || string.IsNullOrEmpty(putAwayDetail.PutAwayCode) || string.IsNullOrEmpty(putAwayDetail.ProductCode))
                {
                    return new ServiceResponse<bool>(false, "Dữ liệu đầu vào không hợp lệ. PutAwayCode và ProductCode là bắt buộc.");
                }

                var existingDetail = await _context.PutAwayDetails
                    .FirstOrDefaultAsync(pd => pd.PutAwayCode == putAwayDetail.PutAwayCode && pd.ProductCode == putAwayDetail.ProductCode);

                if (existingDetail == null)
                {
                    return new ServiceResponse<bool>(false, "Chi tiết putaway không tồn tại.");
                }

                existingDetail.Demand = putAwayDetail.Demand ?? existingDetail.Demand;
                existingDetail.Quantity = putAwayDetail.Quantity ?? existingDetail.Quantity;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResponse<bool>(true, "Cập nhật chi tiết putaway thành công");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi database khi cập nhật chi tiết putaway: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật chi tiết putaway: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> DeletePutAwayDetail(string putawayNo, string productCode)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrEmpty(putawayNo) || string.IsNullOrEmpty(productCode))
                {
                    return new ServiceResponse<bool>(false, "PutAwayCode và ProductCode không được để trống.");
                }

                var putAwayDetail = await _context.PutAwayDetails
                    .FirstOrDefaultAsync(pd => pd.PutAwayCode == putawayNo && pd.ProductCode == productCode);

                if (putAwayDetail == null)
                {
                    return new ServiceResponse<bool>(false, "Chi tiết putaway không tồn tại.");
                }

                _context.PutAwayDetails.Remove(putAwayDetail);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResponse<bool>(true, "Xóa chi tiết putaway thành công");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi database khi xóa chi tiết putaway: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi khi xóa chi tiết putaway: {ex.Message}");
            }
        }
    }
}