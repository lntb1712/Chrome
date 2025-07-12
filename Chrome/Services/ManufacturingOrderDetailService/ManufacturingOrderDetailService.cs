using Chrome.DTO;
using Chrome.DTO.ManufacturingOrderDetailDTO;
using Chrome.Models;
using Chrome.Repositories.InventoryRepository;
using Chrome.Repositories.ManufacturingOrderDetailRepository;
using Chrome.Repositories.ManufacturingOrderRepository;
using Chrome.Repositories.ProductMasterRepository;
using Chrome.Repositories.ReservationRepository;
using Chrome.Repositories.StockInRepository;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Services.ManufacturingOrderDetailService
{
    public class ManufacturingOrderDetailService : IManufacturingOrderDetailService
    {
        private readonly IManufacturingOrderDetailRepository _manufacturingOrderDetailRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IStockInRepository _stockInRepository;
        private readonly IManufacturingOrderRepository _manufacturingOrderRepository;
        private readonly IProductMasterRepository _productMasterRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ChromeContext _context;

        public ManufacturingOrderDetailService(
            IManufacturingOrderDetailRepository manufacturingOrderDetailRepository,
            IReservationRepository reservationRepository,
            IStockInRepository stockInRepository,
            IManufacturingOrderRepository manufacturingOrderRepository,
            IProductMasterRepository productMasterRepository,
            IInventoryRepository inventoryRepository,
            ChromeContext context)
        {
            _manufacturingOrderDetailRepository = manufacturingOrderDetailRepository;
            _reservationRepository = reservationRepository;
            _stockInRepository = stockInRepository;
            _productMasterRepository = productMasterRepository;
            _inventoryRepository = inventoryRepository;
            _manufacturingOrderRepository = manufacturingOrderRepository;
            _context = context;
        }

        public async Task<ServiceResponse<ForecastManufacturingOrderDetailDTO>> GetForecastManufacturingOrderDetail(string manufacturingOrderCode, string productCode)
        {

            if (string.IsNullOrEmpty(manufacturingOrderCode) || string.IsNullOrEmpty(productCode))
            {
                return new ServiceResponse<ForecastManufacturingOrderDetailDTO>(false, "Mã lệnh sản xuất và sản phẩm chọn không được để trống");
            }

            try
            {
                // Lấy thông tin phiếu xuất
                var manufacturingOrder = await _manufacturingOrderRepository.GetManufacturingWithCode(manufacturingOrderCode);
                if (manufacturingOrder == null)
                {
                    return new ServiceResponse<ForecastManufacturingOrderDetailDTO>(false, "Không tìm thấy lệnh sản xuất");
                }

                // Lấy tồn kho hiện tại của sản phẩm tại kho đó
                var inventoryList = await _inventoryRepository.GetInventoryByProductCodeAsync(productCode, manufacturingOrder.WarehouseCode!).ToListAsync();
                var quantityOnHand = inventoryList.Sum(x => x.Quantity)/inventoryList.First().ProductCodeNavigation.BaseQuantity;

                // Lấy tổng số lượng reservation đang giữ cho sản phẩm đó tại kho
                var reservations = await _reservationRepository.GetAllReservationsAsync(new string[] { manufacturingOrder.WarehouseCode! }).Where(x => x.StatusId != 3 && !x.ReservationCode.StartsWith("MV")).ToListAsync();
                var quantityReserved = reservations
                    .SelectMany(r => r.ReservationDetails)
                    .Where(d => d.ProductCode == productCode)
                    .Sum(d => d.QuantityReserved);

                // Lấy tổng số lượng inbound (từ manufacturing order có deadline <= ngày xuất hàng)
                var componentInformation = await _productMasterRepository.GetProductMasterWithProductCode(productCode);
                if (string.IsNullOrEmpty(componentInformation.CategoryId))
                {
                    return new ServiceResponse<ForecastManufacturingOrderDetailDTO>(false, "Sản phẩm chưa khai báo loại CategoryId");
                }

                var manufacturingOrders = await _manufacturingOrderRepository.GetAllManufacturingOrder(new string[] { manufacturingOrder.WarehouseCode! }).ToListAsync();
                var stockIn = await _stockInRepository.GetAllStockInAsync(new string[] { manufacturingOrder.WarehouseCode! }).Where(x => x.OrderDeadline <= manufacturingOrder.ScheduleDate && x.StatusId!=3).ToListAsync();
                var quantityInBound = 0.0;
                if (componentInformation.CategoryId!.Equals("SFG"))
                {
                    quantityInBound = (double)manufacturingOrders
                        .Where(x => x.ProductCode == productCode && x.Deadline <= manufacturingOrder.ScheduleDate)
                        .Sum(x => x.Quantity)!;
                }
                else if (componentInformation.CategoryId.Equals("MAT"))
                {
                    quantityInBound = (double)stockIn
                        .SelectMany(x => x.StockInDetails)
                        .Where(x => x.ProductCode == productCode)
                        .Sum(x => x.Demand)!;
                }

                // Tính khả dụng (ATP)
                var availableQty = quantityOnHand - quantityReserved + quantityInBound;

                // Build kết quả trả về
                var result = new ForecastManufacturingOrderDetailDTO
                {
                    ManufacturingOrderCode = manufacturingOrderCode,
                    ComponentCode = productCode,
                    QuantityOnHand = quantityOnHand,
                    QuantityToOutBound = quantityReserved,
                    QuantityToInBound = quantityInBound,
                    AvailableQty = availableQty
                };

                return new ServiceResponse<ForecastManufacturingOrderDetailDTO>(true, "Lấy thông tin tồn kho thành công", result);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ForecastManufacturingOrderDetailDTO>(false, ex.Message);
            }
        }

        public async Task<ServiceResponse<PagedResponse<ManufacturingOrderDetailResponseDTO>>> GetManufacturingOrderDetail(string manufacturingOrderCode)
        {
            if (string.IsNullOrEmpty(manufacturingOrderCode))
            {
                return new ServiceResponse<PagedResponse<ManufacturingOrderDetailResponseDTO>>(false, "Mã lệnh sản xuất không được để trống");
            }

            try
            {
                var query = _manufacturingOrderDetailRepository.GetManufacturingOrderDetail(manufacturingOrderCode);
                var result = await query
                    .Select(x => new ManufacturingOrderDetailResponseDTO
                    {
                        ManufacturingOrderCode = x.ManufacturingOrderCode,
                        ComponentCode = x.ComponentCode,
                        ComponentName = x.ComponentCodeNavigation!.ProductName!,
                        ToConsumeQuantity = x.ToConsumeQuantity,
                        ConsumedQuantity = x.ConsumedQuantity,
                        ScraptRate = x.ScraptRate
                    })
                    .OrderBy(x => x.ComponentCode)
                    .ToListAsync();

                var totalItems = await query.CountAsync();
                var pagedResponse = new PagedResponse<ManufacturingOrderDetailResponseDTO>(result, 1, totalItems, totalItems);

                if (!result.Any())
                {
                    return new ServiceResponse<PagedResponse<ManufacturingOrderDetailResponseDTO>>(false, $"Không tìm thấy chi tiết lệnh sản xuất cho mã {manufacturingOrderCode}");
                }

                return new ServiceResponse<PagedResponse<ManufacturingOrderDetailResponseDTO>>(true, "Lấy danh sách chi tiết lệnh sản xuất thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<ManufacturingOrderDetailResponseDTO>>(false, $"Lỗi khi lấy danh sách chi tiết lệnh sản xuất: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<ManufacturingOrderDetailResponseDTO>> GetManufacturingOrderDetail(string manufacturingOrderCode, string productCode)
        {
            if (string.IsNullOrEmpty(manufacturingOrderCode) || string.IsNullOrEmpty(productCode))
            {
                return new ServiceResponse<ManufacturingOrderDetailResponseDTO>(false, "Mã lệnh sản xuất hoặc mã sản phẩm không được để trống");
            }

            try
            {
                var detail = await _manufacturingOrderDetailRepository.GetManufacturingOrderDetailWithCode(manufacturingOrderCode, productCode);
                if (detail == null)
                {
                    return new ServiceResponse<ManufacturingOrderDetailResponseDTO>(false, $"Không tìm thấy chi tiết lệnh sản xuất cho mã {manufacturingOrderCode} và mã sản phẩm {productCode}");
                }

                var response = new ManufacturingOrderDetailResponseDTO
                {
                    ManufacturingOrderCode = detail.ManufacturingOrderCode,
                    ComponentCode = detail.ComponentCode,
                    ComponentName = detail.ComponentCodeNavigation!.ProductName!,
                    ToConsumeQuantity = detail.ToConsumeQuantity,
                    ConsumedQuantity = detail.ConsumedQuantity,
                    ScraptRate = detail.ScraptRate
                };

                return new ServiceResponse<ManufacturingOrderDetailResponseDTO>(true, "Lấy chi tiết lệnh sản xuất thành công", response);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ManufacturingOrderDetailResponseDTO>(false, $"Lỗi khi lấy chi tiết lệnh sản xuất: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> UpdateListManufacturingOrderDetail(List<ManufacturingOrderDetailRequestDTO> detailRequestDTOs)
        {
            if (detailRequestDTOs == null || !detailRequestDTOs.Any())
            {
                return new ServiceResponse<bool>(false, "Danh sách chi tiết lệnh sản xuất không được để trống");
            }
            try
            {

                foreach (var detailRequest in detailRequestDTOs)
                {

                    if (string.IsNullOrEmpty(detailRequest.ComponentCode))
                    {
                        return new ServiceResponse<bool>(false, "không tìm thấy mã thành phần");
                    }


                    var existingDetail = await _manufacturingOrderDetailRepository.GetManufacturingOrderDetailWithCode(
                         detailRequest.ManufacturingOrderCode, detailRequest.ComponentCode);

                    if (existingDetail == null)
                    {
                        return new ServiceResponse<bool>(false, "Chi tiết lệnh sản xuất không tồn tại cho mã lệnh sản xuất và mã sản phẩm đã cung cấp");
                    }
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            existingDetail.ToConsumeQuantity = detailRequest.ToConsumeQuantity;
                            existingDetail.ConsumedQuantity = detailRequest.ConsumedQuantity;
                            existingDetail.ScraptRate = detailRequest.ScraptRate;
                            await _context.SaveChangesAsync();
                            // Commit the transaction
                            await transaction.CommitAsync();
                            
                        }
                        catch(DbUpdateException dbEx)
                        {
                            // Rollback the transaction in case of error
                            await transaction.RollbackAsync();
                            return new ServiceResponse<bool>(false, $"Lỗi cập nhật chi tiết lệnh sản xuất: {dbEx.Message}");
                        }
                        catch (Exception ex)
                        {
                            // Rollback the transaction in case of error
                            await transaction.RollbackAsync();
                            return new ServiceResponse<bool>(false, $"Lỗi: {ex.Message}");
                        }
                    }
                }
                return new ServiceResponse<bool>(true, "Cập nhật chi tiết lệnh sản xuất thành công");
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, $"Lỗi: {ex.Message}");
            }
        }
    }
}