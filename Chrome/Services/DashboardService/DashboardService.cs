using Chrome.DTO;
using Chrome.DTO.DashboardDTO;
using Chrome.DTO.InventoryDTO;
using Chrome.DTO.ManufacturingOrderDTO;
using Chrome.Repositories.InventoryRepository;
using Chrome.Repositories.ManufacturingOrderRepository;
using Chrome.Repositories.MovementRepository;
using Chrome.Repositories.StockInRepository;
using Chrome.Repositories.StockOutRepository;
using Chrome.Repositories.TransferRepository;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Services.DashboardService
{
    public class DashboardService: IDashboardService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IManufacturingOrderRepository _manufacturingOrderRepository;
        private readonly IStockInRepository _stockInRepository;
        private readonly IStockOutRepository _stockOutRepository;
        private readonly ITransferRepository _transferRepository;
        private readonly IMovementRepository _movementRepository;

        public DashboardService(
            IInventoryRepository inventoryRepository,
            IManufacturingOrderRepository manufacturingOrderRepository,
            IStockInRepository stockInRepository,
            IStockOutRepository stockOutRepository,
            ITransferRepository transferRepository,
            IMovementRepository movementRepository)
        {
            _inventoryRepository = inventoryRepository;
            _manufacturingOrderRepository = manufacturingOrderRepository;
            _stockInRepository = stockInRepository;
            _stockOutRepository = stockOutRepository;
            _transferRepository = transferRepository;
            _movementRepository = movementRepository;
        }
        public async Task<ServiceResponse<DashboardResponseDTO>> GetDashboardInformation(DashboardRequestDTO dashboardRequest)
        {
            if (dashboardRequest == null)
                return new ServiceResponse<DashboardResponseDTO>(false, "Dữ liệu nhận vào không hợp lệ");

            var inventorySummary = _inventoryRepository.GetInventories(dashboardRequest.warehouseCodes);
            var manufacturingOrders = _manufacturingOrderRepository.GetAllManufacturingOrder(dashboardRequest.warehouseCodes);
            var stockIns =  _stockInRepository.GetAllStockInAsync(dashboardRequest.warehouseCodes);
            var stockOuts =  _stockOutRepository.GetAllStockOutAsync(dashboardRequest.warehouseCodes);
            var transfers =  _transferRepository.GetAllTransfersAsync(dashboardRequest.warehouseCodes);
            var movements =  _movementRepository.GetAllMovementAsync(dashboardRequest.warehouseCodes);

            var statusOrderCodeDTOs = new List<StatusOrderCodeDTO>();

            // STOCK IN
            statusOrderCodeDTOs.Add(new StatusOrderCodeDTO
            {
                OrderTypeCode = "StockIn",
                CountStatusStart = stockIns.Count(x => x.StatusId == 1),
                CountStatusInProgress = stockIns.Count(x => x.StatusId == 2),
                CountStatusCompleted = stockIns.Count(x => x.StatusId == 3)
            });

            // STOCK OUT
            statusOrderCodeDTOs.Add(new StatusOrderCodeDTO
            {
                OrderTypeCode = "StockOut",
                CountStatusStart = stockOuts.Count(x => x.StatusId == 1),
                CountStatusInProgress = stockOuts.Count(x => x.StatusId == 2),
                CountStatusCompleted = stockOuts.Count(x => x.StatusId == 3)
            });

            // MANUFACTURING ORDER
            statusOrderCodeDTOs.Add(new StatusOrderCodeDTO
            {
                OrderTypeCode = "ManufacturingOrder",
                CountStatusStart = manufacturingOrders.Count(x => x.StatusId == 1),
                CountStatusInProgress = manufacturingOrders.Count(x => x.StatusId == 2),
                CountStatusCompleted = manufacturingOrders.Count(x => x.StatusId == 3)
            });

            // TRANSFER
            statusOrderCodeDTOs.Add(new StatusOrderCodeDTO
            {
                OrderTypeCode = "Transfer",
                CountStatusStart = transfers.Count(x => x.StatusId == 1),
                CountStatusInProgress = transfers.Count(x => x.StatusId == 2),
                CountStatusCompleted = transfers.Count(x => x.StatusId == 3)
            });

            // MOVEMENT
            statusOrderCodeDTOs.Add(new StatusOrderCodeDTO
            {
                OrderTypeCode = "Movement",
                CountStatusStart = movements.Count(x => x.StatusId == 1),
                CountStatusInProgress = movements.Count(x => x.StatusId == 2),
                CountStatusCompleted = movements.Count(x => x.StatusId == 3)
            });


            var upcomingDeadlines = new List<UpcomingDeadlineDTO>();
            var today = DateTime.Today;
            var deadlineRange = today.AddDays(3);

            // STOCK IN
            upcomingDeadlines.AddRange(
                stockIns
                .Where(x => x.OrderDeadline.HasValue &&
                            x.OrderDeadline.Value.Date >= today &&
                            x.OrderDeadline.Value.Date <= deadlineRange)
                .Select(x => new UpcomingDeadlineDTO
                {
                    OrderCode = x.StockInCode,
                    ProductCodes = x.StockInDetails
                                     .Select(d => d.ProductCode!)
                                     .Distinct()
                                     .ToList(),
                    Deadline = x.OrderDeadline!.Value.ToString("yyyy-MM-dd"),
                    StatusName = x.OrderDeadline.Value.Date < today ? "Trễ hạn" : "Còn hạn"
                }));

            // STOCK OUT
            upcomingDeadlines.AddRange(
                stockOuts
                .Where(x => x.StockOutDate.HasValue &&
                            x.StockOutDate.Value.Date >= today &&
                            x.StockOutDate.Value.Date <= deadlineRange)
                .Select(x => new UpcomingDeadlineDTO
                {
                    OrderCode = x.StockOutCode,
                    ProductCodes = x.StockOutDetails
                                     .Select(d => d.ProductCode!)
                                     .Distinct()
                                     .ToList(),
                    Deadline = x.StockOutDate!.Value.ToString("yyyy-MM-dd"),
                    StatusName = x.StockOutDate.Value.Date < today ? "Trễ hạn" : "Còn hạn"
                }));

            // MANUFACTURING ORDER
            upcomingDeadlines.AddRange(
                manufacturingOrders
                .Where(x => x.Deadline.HasValue &&
                            x.Deadline.Value.Date >= today &&
                            x.Deadline.Value.Date <= deadlineRange)
                .Select(x => new UpcomingDeadlineDTO
                {
                    OrderCode = x.ManufacturingOrderCode,
                    ProductCodes = new List<string> { x.ProductCode! },
                    Deadline = x.Deadline!.Value.ToString("yyyy-MM-dd"),
                    StatusName = x.Deadline.Value.Date < today ? "Trễ hạn" : "Còn hạn"
                }));

            // TRANSFER
            upcomingDeadlines.AddRange(
                transfers
                .Where(x => x.TransferDate.HasValue &&
                            x.TransferDate.Value.Date >= today &&
                            x.TransferDate.Value.Date <= deadlineRange)
                .Select(x => new UpcomingDeadlineDTO
                {
                    OrderCode = x.TransferCode,
                    ProductCodes = x.TransferDetails
                                     .Select(d => d.ProductCode!)
                                     .Distinct()
                                     .ToList(),
                    Deadline = x.TransferDate!.Value.ToString("yyyy-MM-dd"),
                    StatusName = x.TransferDate.Value.Date < today ? "Trễ hạn" : "Còn hạn"
                }));

            // MOVEMENT
            upcomingDeadlines.AddRange(
                movements
                .Where(x => x.MovementDate.HasValue &&
                            x.MovementDate.Value.Date >= today &&
                            x.MovementDate.Value.Date <= deadlineRange)
                .Select(x => new UpcomingDeadlineDTO
                {
                    OrderCode = x.MovementCode,
                    ProductCodes = x.MovementDetails
                                     .Select(d => d.ProductCode!)
                                     .Distinct()
                                     .ToList(),
                    Deadline = x.MovementDate!.Value.ToString("yyyy-MM-dd"),
                    StatusName = x.MovementDate.Value.Date < today ? "Trễ hạn" : "Còn hạn"
                }));
            var inventoryResponse = await inventorySummary
                    .GroupBy(x => x.ProductCode)
                .Select(g => new InventorySummaryDTO
                {
                    ProductCode = g.Key,
                    ProductName = g.First().ProductCodeNavigation.ProductName!,
                    CategoryId = g.First().ProductCodeNavigation.CategoryId!,
                    CategoryName = g.First().ProductCodeNavigation.Category!.CategoryName!,
                    Quantity = g.Sum(x => x.Quantity),
                    BaseQuantity = g.Sum(x => x.Quantity / (x.ProductCodeNavigation.BaseQuantity)),
                    UOM = g.First().ProductCodeNavigation.Uom!,
                    BaseUOM = g.First().ProductCodeNavigation.BaseUom!
                }).ToListAsync();
            var dashBoardResponse = new DashboardResponseDTO
            {
                // Tổng hợp tồn kho
                inventorySummaryDTOs = inventoryResponse,

                // Tiến độ sản xuất
                progressManufacturingOrderDTOs = await manufacturingOrders
                 
                 .OrderByDescending(x=>x.ScheduleDate)
                 .Take(5)
                 .Select(x => new ProgressManufacturingOrderDTO
                {
                    ManufacturingOrderCode = x.ManufacturingOrderCode,
                    ProductCode = x.ProductCode!,
                    ProductName = x.ProductCodeNavigation!.ProductName!,
                    Quantity = x.Quantity,
                    QuantityProduced = x.QuantityProduced,
                    StatusName = x.Status!.StatusName!,
                    WarehouseCode = x.WarehouseCode!,
                    Progress = x.QuantityProduced.HasValue && x.Quantity > 0
                        ? (double)x.QuantityProduced / x.Quantity * 100
                        : 0,
                }).ToListAsync(),

                // Số lượng chưa hoàn thành
                QuantityToCompleteStockIn = await stockIns.Where(x => x.StatusId == 2 && x.StockInDetails.All(x => x.Demand == x.Quantity)).CountAsync(),
                QuantityToCompleteStockOut = await stockOuts.Where(x => x.StatusId == 2 && x.StockOutDetails.All(x => x.Demand == x.Quantity)).CountAsync(),
                QuantityToCompleteManufacturingOrder = await manufacturingOrders.Where(x => x.StatusId == 2 && x.Quantity == x.QuantityProduced).CountAsync(),

                // Các deadline sắp tới
                upcomingDeadlines = upcomingDeadlines.ToList(),
                statusOrderCodeDTOs = statusOrderCodeDTOs.ToList(),



            };

            return new ServiceResponse<DashboardResponseDTO>(true, "Lấy dữ liệu thành công", dashBoardResponse);
        }

        public async Task<ServiceResponse<DashboardStockInOutSummaryDTO>> GetStockInOutSummaryAsync(DashboardRequestDTO dashboardRequest)
        {
            if (dashboardRequest == null)
                return new ServiceResponse<DashboardStockInOutSummaryDTO>(false, "Dữ liệu nhận vào không hợp lệ");
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek); // Chủ nhật
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var last7Days = today.AddDays(-6);

            var stockIns = await _stockInRepository.GetAllStockInAsync(dashboardRequest.warehouseCodes).ToListAsync();
            var stockOuts = await _stockOutRepository.GetAllStockOutAsync(dashboardRequest.warehouseCodes).ToListAsync();

            var result = new DashboardStockInOutSummaryDTO
            {
                StockInToday = stockIns.Count(x => x.OrderDeadline == today),
                StockInThisWeek = stockIns.Count(x => x.OrderDeadline >= startOfWeek),
                StockInThisMonth = stockIns.Count(x => x.OrderDeadline >= startOfMonth),

                StockOutToday = stockOuts.Count(x => x.StockOutDate == today),
                StockOutThisWeek = stockOuts.Count(x => x.StockOutDate >= startOfWeek),
                StockOutThisMonth = stockOuts.Count(x => x.StockOutDate >= startOfMonth),

                DailyStockInOuts = Enumerable.Range(0, 7)
                    .Select(i =>
                    {
                        var date = last7Days.AddDays(i).Date;
                        return new DailyStockInOutDTO
                        {
                            Date = date.ToString("dd/MM/yyyy"),
                            StockInCount = stockIns.Count(x => x.OrderDeadline == date),
                            StockOutCount = stockOuts.Count(x => x.StockOutDate == date)
                        };
                    })
                    .ToList()
            };

            return new ServiceResponse<DashboardStockInOutSummaryDTO>(true, "Thành công", result);
        }
    }
}
