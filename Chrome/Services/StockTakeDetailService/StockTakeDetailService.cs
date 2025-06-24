using Chrome.DTO;
using Chrome.DTO.StockTakeDetailDTO;
using Chrome.Models;
using Chrome.Repositories.StockTakeDetailRepository;
using Chrome.Repositories.StockTakeRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Chrome.Services.StockTakeDetailService
{
    public class StockTakeDetailService : IStockTakeDetailService
    {
        private readonly IStockTakeDetailRepository _StockTakeDetailRepository;
        private readonly IStockTakeRepository _StockTakeRepository;
        private readonly ChromeContext _context;

        public StockTakeDetailService(
            IStockTakeDetailRepository StockTakeDetailRepository,
            IStockTakeRepository StockTakeRepository,
            ChromeContext context)
        {
            _StockTakeDetailRepository = StockTakeDetailRepository ?? throw new ArgumentNullException(nameof(StockTakeDetailRepository));
            _StockTakeRepository = StockTakeRepository ?? throw new ArgumentNullException(nameof(StockTakeRepository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ServiceResponse<PagedResponse<StockTakeDetailResponseDTO>>> GetAllStockTakeDetailsAsync(string[] warehouseCodes, int page = 1, int pageSize = 10)
        {
            try
            {
                if (warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
                {
                    return new ServiceResponse<PagedResponse<StockTakeDetailResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
                }

                var query = _StockTakeDetailRepository.GetAllStockTakeDetailsAsync(warehouseCodes);
                var totalItems = await query.CountAsync();
                var StockTakeDetails = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(sd => new StockTakeDetailResponseDTO
                    {
                        StocktakeCode = sd.StocktakeCode,
                        ProductCode = sd.ProductCode,
                        ProductName = sd.ProductCodeNavigation!.ProductName!,
                        Lotno = sd.Lotno,
                        LocationCode = sd.LocationCode,
                        Quantity = sd.Quantity,
                        CountedQuantity = sd.CountedQuantity
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<StockTakeDetailResponseDTO>(StockTakeDetails, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<StockTakeDetailResponseDTO>>(true, "Lấy danh sách chi tiết kiểm kho thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<StockTakeDetailResponseDTO>>(false, $"Lỗi khi lấy danh sách chi tiết kiểm kho: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<StockTakeDetailResponseDTO>>> GetStockTakeDetailsByStockTakeCodeAsync(string StockTakeCode, int page = 1, int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(StockTakeCode))
                {
                    return new ServiceResponse<PagedResponse<StockTakeDetailResponseDTO>>(false, "Mã kiểm kho không được để trống");
                }

                if (page < 1 || pageSize < 1)
                {
                    return new ServiceResponse<PagedResponse<StockTakeDetailResponseDTO>>(false, "Dữ liệu phân trang không hợp lệ");
                }

                var query = _StockTakeDetailRepository.GetStockTakeDetailsByStockTakeCodeAsync(StockTakeCode);
                var totalItems = await query.CountAsync();
                var StockTakeDetails = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(sd => new StockTakeDetailResponseDTO
                    {
                        StocktakeCode = sd.StocktakeCode,
                        ProductCode = sd.ProductCode,
                        ProductName = sd.ProductCodeNavigation!.ProductName!,
                        Lotno = sd.Lotno,
                        LocationCode = sd.LocationCode,
                        Quantity = sd.Quantity,
                        CountedQuantity = sd.CountedQuantity
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<  StockTakeDetailResponseDTO>(StockTakeDetails, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<   StockTakeDetailResponseDTO>>(true, "Lấy chi tiết kiểm kho theo mã thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<StockTakeDetailResponseDTO>>(false, $"Lỗi khi lấy chi tiết kiểm kho theo mã: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<StockTakeDetailResponseDTO>>> SearchStockTakeDetailsAsync(string[] warehouseCodes, string StockTakeCode, string textToSearch, int page = 1, int pageSize = 10)
        {
            try
            {
                if (warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
                {
                    return new ServiceResponse<PagedResponse<StockTakeDetailResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
                }

                var query = _StockTakeDetailRepository.SearchStockTakeDetailsAsync(warehouseCodes, StockTakeCode, textToSearch);
                var totalItems = await query.CountAsync();
                var StockTakeDetails = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(sd => new StockTakeDetailResponseDTO
                    {
                        StocktakeCode = sd.StocktakeCode,
                        ProductCode = sd.ProductCode,
                        ProductName = sd.ProductCodeNavigation!.ProductName!,
                        Lotno = sd.Lotno,
                        LocationCode = sd.LocationCode,
                        Quantity = sd.Quantity,
                        CountedQuantity = sd.CountedQuantity
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<StockTakeDetailResponseDTO>(StockTakeDetails, page, pageSize, totalItems);
                return new ServiceResponse<PagedResponse<StockTakeDetailResponseDTO>>(true, "Tìm kiếm chi tiết kiểm kho thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<StockTakeDetailResponseDTO>>(false, $"Lỗi khi tìm kiếm chi tiết kiểm kho: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> UpdateStockTakeDetail(StockTakeDetailRequestDTO StockTakeDetail)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (StockTakeDetail == null ||
                    string.IsNullOrEmpty(StockTakeDetail.StocktakeCode) ||
                    string.IsNullOrEmpty(StockTakeDetail.ProductCode) ||
                    string.IsNullOrEmpty(StockTakeDetail.Lotno) ||
                    string.IsNullOrEmpty(StockTakeDetail.LocationCode))
                {
                    return new ServiceResponse<bool>(false, "Dữ liệu đầu vào không hợp lệ. StockTakeCode, ProductCode, Lotno và LocationCode là bắt buộc.");
                }

                var existingDetail = await _context.StocktakeDetails
                    .FirstOrDefaultAsync(sd =>
                        sd.StocktakeCode == StockTakeDetail.StocktakeCode &&
                        sd.ProductCode == StockTakeDetail.ProductCode &&
                        sd.Lotno == StockTakeDetail.Lotno &&
                        sd.LocationCode == StockTakeDetail.LocationCode);

                if (existingDetail == null)
                {
                    return new ServiceResponse<bool>(false, "Chi tiết kiểm kho không tồn tại.");
                }

                // Update allowed fields
                existingDetail.Quantity = StockTakeDetail.Quantity ?? existingDetail.Quantity;
                existingDetail.CountedQuantity = StockTakeDetail.CountedQuantity ?? existingDetail.CountedQuantity;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResponse<bool>(true, "Cập nhật chi tiết kiểm kho thành công");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi database khi cập nhật chi tiết kiểm kho: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật chi tiết kiểm kho: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> DeleteStockTakeDetail(string StockTakeCode, string productCode, string lotNo, string locationCode)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrEmpty(StockTakeCode) || string.IsNullOrEmpty(productCode) ||
                    string.IsNullOrEmpty(lotNo) || string.IsNullOrEmpty(locationCode))
                {
                    return new ServiceResponse<bool>(false, "StockTakeCode, ProductCode, Lotno và LocationCode không được để trống.");
                }

                var StockTakeDetail = await _context.StocktakeDetails
                    .FirstOrDefaultAsync(sd =>
                        sd.StocktakeCode == StockTakeCode &&
                        sd.ProductCode == productCode &&
                        sd.Lotno == lotNo &&
                        sd.LocationCode == locationCode);

                if (StockTakeDetail == null)
                {
                    return new ServiceResponse<bool>(false, "Chi tiết kiểm kho không tồn tại.");
                }

                Expression<Func<StocktakeDetail, bool>> predicate = sd =>
                    sd.StocktakeCode == StockTakeCode &&
                    sd.ProductCode == productCode &&
                    sd.Lotno == lotNo &&
                    sd.LocationCode == locationCode;

                await _StockTakeDetailRepository.DeleteFirstByConditionAsync(predicate);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResponse<bool>(true, "Xóa chi tiết kiểm kho thành công");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi database khi xóa chi tiết kiểm kho: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResponse<bool>(false, $"Lỗi khi xóa chi tiết kiểm kho: {ex.Message}");
            }
        }
    }
}
