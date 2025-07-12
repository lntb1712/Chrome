using Chrome.DTO;
using Chrome.DTO.ProductMasterDTO;
using Chrome.DTO.StockOutDetailDTO;
using Chrome.Models;
using Chrome.Repositories.InventoryRepository;
using Chrome.Repositories.ManufacturingOrderRepository;
using Chrome.Repositories.ProductMasterRepository;
using Chrome.Repositories.ReservationRepository;
using Chrome.Repositories.StockOutDetailRepository;
using Chrome.Repositories.StockOutRepository;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq.Expressions;

namespace Chrome.Services.StockOutDetailService
{
    public class StockOutDetailService : IStockOutDetailService
    {
        private readonly IStockOutDetailRepository _stockOutDetailRepository;
        private readonly IStockOutRepository _stockOutRepository;
        private readonly IProductMasterRepository _productMasterRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IManufacturingOrderRepository _manufacturingOrderRepository;
        private readonly ChromeContext _context;

        public StockOutDetailService(
            IStockOutDetailRepository stockOutDetailRepository,
            IStockOutRepository stockOutRepository,
            IProductMasterRepository productMasterRepository,
            IInventoryRepository inventoryRepository,
            IReservationRepository reservationRepository,
            IManufacturingOrderRepository manufacturingOrderRepository,
            ChromeContext context)
        {
            _stockOutDetailRepository = stockOutDetailRepository;
            _stockOutRepository = stockOutRepository;
            _productMasterRepository = productMasterRepository;
            _inventoryRepository = inventoryRepository;
            _reservationRepository = reservationRepository;
            _manufacturingOrderRepository = manufacturingOrderRepository;
            _context = context;
        }

        public async Task<ServiceResponse<bool>> AddStockOutDetail(StockOutDetailRequestDTO stockOutDetail)
        {
            if (stockOutDetail == null) return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            if (string.IsNullOrEmpty(stockOutDetail.StockOutCode) || string.IsNullOrEmpty(stockOutDetail.ProductCode))
                return new ServiceResponse<bool>(false, "Mã xuất kho và mã sản phẩm không được để trống");

            var stockOut = await _stockOutRepository.GetStockOutWithCode(stockOutDetail.StockOutCode);
            if (stockOut == null)
                return new ServiceResponse<bool>(false, "Phiếu xuất không tồn tại");

            var stockOutDetailRequest = new StockOutDetail
            {
                StockOutCode = stockOutDetail.StockOutCode,
                ProductCode = stockOutDetail.ProductCode,
                Demand = stockOutDetail.Demand,
                Quantity = stockOutDetail.Quantity
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _stockOutDetailRepository.AddAsync(stockOutDetailRequest, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Thêm sản phẩm để xuất thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("unique") || error.Contains("duplicate") || error.Contains("primary key"))
                            return new ServiceResponse<bool>(false, "Dữ liệu đã tồn tại");
                        if (error.Contains("foreign key"))
                            return new ServiceResponse<bool>(false, "Dữ liệu tham chiếu không đúng");
                    }
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi thêm sản phẩm xuất kho: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> CheckAndUpdateBackOrderStatus(string stockOutCode)
        {
            if (string.IsNullOrEmpty(stockOutCode))
                return new ServiceResponse<bool>(false, "Mã xuất kho không được để trống");

            string decodedStockOutCode = Uri.UnescapeDataString(stockOutCode);

            var stockOutHeader = await _context.StockOuts.FirstOrDefaultAsync(x => x.StockOutCode == decodedStockOutCode);
            if (stockOutHeader == null)
                return new ServiceResponse<bool>(false, "Phiếu xuất kho không tồn tại");

            var allStockOutDetails = await _context.StockOutDetails
                .Where(x => x.StockOutCode == decodedStockOutCode)
                .ToListAsync();
            if (allStockOutDetails.Count == 0)
                return new ServiceResponse<bool>(false, "Không có sản phẩm trong phiếu xuất");

            bool allCompleted = allStockOutDetails.All(x => x.Quantity >= x.Demand);

            if (allCompleted)
            {
                stockOutHeader.StatusId = 3; // Hoàn tất
                _context.StockOuts.Update(stockOutHeader);
                await _context.SaveChangesAsync();
                return new ServiceResponse<bool>(true, "Phiếu xuất kho đã được hoàn tất");
            }
            return new ServiceResponse<bool>(true, "Chưa đủ số lượng để hoàn tất phiếu xuất kho");
        }

        public async Task<ServiceResponse<bool>> ConfirmStockOut(string stockOutCode)
        {
            if (string.IsNullOrEmpty(stockOutCode))
                return new ServiceResponse<bool>(false, "Mã xuất kho không được để trống");

            string decodedStockOutCode = Uri.UnescapeDataString(stockOutCode);

            var lstStockOutDetails = await _stockOutDetailRepository.GetAllStockOutDetails(decodedStockOutCode).ToListAsync();
            if (lstStockOutDetails.Count == 0)
                return new ServiceResponse<bool>(false, "Không có sản phẩm để xác nhận xuất kho");

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var stockOutHeader = await _context.StockOuts
                        .FirstOrDefaultAsync(x => x.StockOutCode == decodedStockOutCode);
                    if (stockOutHeader == null)
                        return new ServiceResponse<bool>(false, "Phiếu xuất kho không tồn tại");

                    stockOutHeader.StatusId = 3; // Đang xử lý hoặc trạng thái xác nhận
                    _context.StockOuts.Update(stockOutHeader);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xác nhận xuất kho thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xác nhận xuất kho: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> CreateBackOrder(string stockOutCode, string backOrderDescription, string dateBackOrder)
        {
            if (string.IsNullOrEmpty(stockOutCode))
                return new ServiceResponse<bool>(false, "Mã xuất kho không được để trống");

            string decodedStockOutCode = Uri.UnescapeDataString(stockOutCode);

            var lstStockOutDetails = await _stockOutDetailRepository.GetAllStockOutDetails(decodedStockOutCode).ToListAsync();
            if (lstStockOutDetails.Count == 0)
                return new ServiceResponse<bool>(false, "Không có sản phẩm trong phiếu xuất");

            var itemsToBackOrder = lstStockOutDetails.Where(item => item.Quantity < item.Demand).ToList();
            if (itemsToBackOrder.Count == 0)
                return new ServiceResponse<bool>(false, "Không có sản phẩm cần tạo backorder");

            if (string.IsNullOrEmpty(dateBackOrder)) return new ServiceResponse<bool>(false, "Mã xuất kho không được để trống");
            string[] formats = {
                "M/d/yyyy h:mm:ss tt",
                "MM/dd/yyyy hh:mm:ss tt",
                "dd/MM/yyyy","dd/MM/yyyy hh:mm:ss tt"
            };
            if (!DateTime.TryParseExact(dateBackOrder, formats, new CultureInfo("vi-VN"), DateTimeStyles.None, out DateTime parsedDate))
            {
                return new ServiceResponse<bool>(false, "Ngày xuất kho không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy hoặc M/d/yyyy h:mm:ss tt.");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var stockOutHeader = itemsToBackOrder.First().StockOutCodeNavigation;

                    var backOrderCode = "BackOrder/" + decodedStockOutCode;
                    var newStockOut = new StockOut
                    {
                        StockOutCode = backOrderCode,
                        OrderTypeCode = stockOutHeader.OrderTypeCode,
                        WarehouseCode = stockOutHeader.WarehouseCode,
                        CustomerCode = stockOutHeader.CustomerCode,
                        Responsible = stockOutHeader.Responsible,
                        StockOutDate = parsedDate,
                        StatusId = 1,
                        StockOutDescription = backOrderDescription
                    };

                    await _stockOutRepository.AddAsync(newStockOut, saveChanges: false);

                    foreach (var item in itemsToBackOrder)
                    {
                        var quantityDifference = item.Demand - item.Quantity;

                        var backOrderDetail = new StockOutDetail
                        {
                            StockOutCode = backOrderCode,
                            ProductCode = item.ProductCode,
                            Demand = quantityDifference,
                            Quantity = 0
                        };

                        item.Demand = item.Quantity;

                        await _stockOutDetailRepository.AddAsync(backOrderDetail, saveChanges: false);
                        await _stockOutDetailRepository.UpdateAsync(item, saveChanges: false);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new ServiceResponse<bool>(true, "Đã tạo lệnh xuất kho trở lại (backorder)");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("unique") || error.Contains("duplicate") || error.Contains("primary key"))
                            return new ServiceResponse<bool>(false, "Dữ liệu đã tồn tại");
                        if (error.Contains("foreign key"))
                            return new ServiceResponse<bool>(false, "Dữ liệu tham chiếu không đúng");
                    }
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi tạo backorder: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteStockOutDetail(string stockOutCode, string productCode)
        {
            if (string.IsNullOrEmpty(stockOutCode) || string.IsNullOrEmpty(productCode))
                return new ServiceResponse<bool>(false, "Mã xuất kho và mã sản phẩm không được để trống");

            string decodedStockOutCode = Uri.UnescapeDataString(stockOutCode);

            var existingStockOutDetail = await _stockOutDetailRepository.GetStockOutDetailWithCode(decodedStockOutCode, productCode);
            if (existingStockOutDetail == null)
                return new ServiceResponse<bool>(false, "Sản phẩm để xuất kho không tồn tại");

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    Expression<Func<StockOutDetail, bool>> predicate = x => x.StockOutCode == decodedStockOutCode && x.ProductCode == productCode;
                    await _stockOutDetailRepository.DeleteFirstByConditionAsync(predicate);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa sản phẩm xuất kho thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("foreign key"))
                            return new ServiceResponse<bool>(false, "Không thể xóa sản phẩm xuất kho vì có dữ liệu tham chiếu.");
                    }
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xóa sản phẩm xuất kho: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<PagedResponse<StockOutDetailResponseDTO>>> GetAllStockOutDetails(string stockOutCode, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(stockOutCode))
                return new ServiceResponse<PagedResponse<StockOutDetailResponseDTO>>(false, "Mã phiếu xuất kho không được để trống");

            string decodedStockOutCode = Uri.UnescapeDataString(stockOutCode);

            var query = _stockOutDetailRepository.GetAllStockOutDetails(decodedStockOutCode);
            var lstStockOutDetails = await query
                                    .Select(x => new StockOutDetailResponseDTO
                                    {
                                        StockOutCode = x.StockOutCode,
                                        ProductCode = x.ProductCode,
                                        ProductName = x.ProductCodeNavigation.ProductName!,
                                        Demand = x.Demand,
                                        Quantity = x.Quantity,
                                    })
                                    .OrderBy(x => x.ProductCode)
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();
            var totalItems = await query.CountAsync();
            var pagedResponse = new PagedResponse<StockOutDetailResponseDTO>(lstStockOutDetails, page, pageSize, totalItems);
            return new ServiceResponse<PagedResponse<StockOutDetailResponseDTO>>(true, "Lấy danh sách xuất kho thành công", pagedResponse);
        }

        public async Task<ServiceResponse<ForecastStockOutDetailDTO>> GetForecastStockOutDetail(string stockOutCode, string productCode)
        {
            if (string.IsNullOrEmpty(stockOutCode) || string.IsNullOrEmpty(productCode))
            {
                return new ServiceResponse<ForecastStockOutDetailDTO>(false, "Mã phiếu xuất và sản phẩm chọn không được để trống");
            }

            try
            {
                // Lấy thông tin phiếu xuất
                var stockOut = await _stockOutRepository.GetStockOutWithCode(stockOutCode);
                if (stockOut == null)
                {
                    return new ServiceResponse<ForecastStockOutDetailDTO>(false, "Không tìm thấy phiếu xuất kho");
                }

                // Lấy tồn kho hiện tại của sản phẩm tại kho đó
                var inventoryList = await _inventoryRepository.GetInventoryByProductCodeAsync(productCode, stockOut.WarehouseCode!).ToListAsync();
                var quantityOnHand = inventoryList.Sum(x => x.Quantity);

                // Lấy tổng số lượng reservation đang giữ cho sản phẩm đó tại kho
                var reservations = await _reservationRepository.GetAllReservationsAsync(new string[] { stockOut.WarehouseCode! }).Where(x => x.StatusId != 3 && !x.ReservationCode.StartsWith("MV")).ToListAsync();
                var quantityReserved = reservations
                    .SelectMany(r => r.ReservationDetails)
                    .Where(d => d.ProductCode == productCode)
                    .Sum(d => d.QuantityReserved);

                // Lấy tổng số lượng inbound (từ manufacturing order có deadline <= ngày xuất hàng)
                var manufacturingOrders = await _manufacturingOrderRepository.GetAllManufacturingOrder(new string[] { stockOut.WarehouseCode! }).ToListAsync();
                var quantityInBound = manufacturingOrders
                    .Where(x => x.ProductCode == productCode && x.Deadline <= stockOut.StockOutDate)
                    .Sum(x => x.Quantity);

                // Tính khả dụng (ATP)
                var availableQty = quantityOnHand - quantityReserved + quantityInBound;

                // Build kết quả trả về
                var result = new ForecastStockOutDetailDTO
                {
                    StockOutCode = stockOutCode,
                    ProductCode = productCode,
                    QuantityOnHand = quantityOnHand,
                    QuantityToOutBound = quantityReserved,
                    QuantityToInBound = quantityInBound,
                    AvailableQty = availableQty
                };

                return new ServiceResponse<ForecastStockOutDetailDTO>(true, "Lấy thông tin tồn kho thành công", result);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ForecastStockOutDetailDTO>(false, ex.Message);
            }
        }



        public async Task<ServiceResponse<List<ProductMasterResponseDTO>>> GetListProductToSO()
        {
            var lstProduct = await _productMasterRepository.GetAllProduct(1, int.MaxValue);
            var lstProductForSO = lstProduct.Where(p => p.CategoryId == "FG")
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

            return new ServiceResponse<List<ProductMasterResponseDTO>>(true, "Lấy danh sách sản phẩm thành công", lstProductForSO);
        }

        public async Task<ServiceResponse<bool>> UpdateStockOutDetail(StockOutDetailRequestDTO stockOutDetail)
        {
            if (stockOutDetail == null)
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");

            string decodedStockOutCode = Uri.UnescapeDataString(stockOutDetail.StockOutCode);

            var existingStockOutDetail = await _stockOutDetailRepository.GetStockOutDetailWithCode(decodedStockOutCode, stockOutDetail.ProductCode);
            if (existingStockOutDetail == null)
                return new ServiceResponse<bool>(false, "Không tìm thấy sản phẩm để xuất kho");

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    existingStockOutDetail.Quantity = stockOutDetail.Quantity;
                    existingStockOutDetail.Demand = stockOutDetail.Demand;
                    await _stockOutDetailRepository.UpdateAsync(existingStockOutDetail, saveChanges: false);

                    var allStockOutDetails = await _context.StockOutDetails
                        .Where(x => x.StockOutCode == decodedStockOutCode)
                        .ToListAsync();
                    bool allCompleted = allStockOutDetails.All(x => x.Quantity >= x.Demand);

                    var stockOutHeader = await _context.StockOuts
                        .FirstOrDefaultAsync(x => x.StockOutCode == decodedStockOutCode);
                    if (stockOutHeader != null)
                    {
                        if (allCompleted)
                            stockOutHeader.StatusId = 3; // Hoàn tất
                        else
                            stockOutHeader.StatusId = 2; // Đang xử lý
                        _context.StockOuts.Update(stockOutHeader);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, allCompleted ?
                        "Cập nhật số lượng sản phẩm và hoàn tất phiếu xuất kho thành công" :
                        "Cập nhật số lượng sản phẩm và đặt trạng thái đang xử lý thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("unique") || error.Contains("duplicate") || error.Contains("primary key"))
                            return new ServiceResponse<bool>(false, "Dữ liệu đã tồn tại");
                        if (error.Contains("foreign key"))
                            return new ServiceResponse<bool>(false, "Dữ liệu tham chiếu không đúng");
                    }
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật số lượng sản phẩm xuất kho: {ex.Message}");
                }
            }
        }
    }
}