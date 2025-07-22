
using Chrome.DTO;
using Chrome.DTO.DashboardDTO;
using Chrome.DTO.InventoryDTO;
using Chrome.DTO.ManufacturingOrderDTO;
using Chrome.Repositories.InventoryRepository;
using Chrome.Repositories.ManufacturingOrderRepository;
using Chrome.Repositories.MovementRepository;
using Chrome.Repositories.PurchaseOrderRepository;
using Chrome.Repositories.StockInRepository;
using Chrome.Repositories.StockOutRepository;
using Chrome.Repositories.StockTakeRepository;
using Chrome.Repositories.TransferRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chrome.Services.DashboardService
{
    public class DashboardService : IDashboardService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IManufacturingOrderRepository _manufacturingOrderRepository;
        private readonly IStockInRepository _stockInRepository;
        private readonly IStockOutRepository _stockOutRepository;
        private readonly ITransferRepository _transferRepository;
        private readonly IMovementRepository _movementRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IStockTakeRepository _stocktakeRepository;

        public DashboardService(
            IInventoryRepository inventoryRepository,
            IManufacturingOrderRepository manufacturingOrderRepository,
            IStockInRepository stockInRepository,
            IStockOutRepository stockOutRepository,
            ITransferRepository transferRepository,
            IMovementRepository movementRepository,
            IPurchaseOrderRepository purchaseOrderRepository,
            IStockTakeRepository stocktakeRepository)
        {
            _inventoryRepository = inventoryRepository;
            _manufacturingOrderRepository = manufacturingOrderRepository;
            _stockInRepository = stockInRepository;
            _stockOutRepository = stockOutRepository;
            _transferRepository = transferRepository;
            _movementRepository = movementRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
            _stocktakeRepository = stocktakeRepository;
        }

        public async Task<ServiceResponse<DashboardResponseDTO>> GetDashboardInformation(DashboardRequestDTO dashboardRequest)
        {
            if (dashboardRequest == null)
                return new ServiceResponse<DashboardResponseDTO>(false, "Dữ liệu nhận vào không hợp lệ");

            // Áp dụng bộ lọc ngày, tháng, năm
            var inventorySummary = _inventoryRepository.GetInventories(dashboardRequest.warehouseCodes);
            var manufacturingOrders = FilterByDate(_manufacturingOrderRepository.GetAllManufacturingOrder(dashboardRequest.warehouseCodes), dashboardRequest, "ScheduleDate");
            var stockIns = FilterByDate(_stockInRepository.GetAllStockInAsync(dashboardRequest.warehouseCodes), dashboardRequest, "OrderDeadline");
            var stockOuts = FilterByDate(_stockOutRepository.GetAllStockOutAsync(dashboardRequest.warehouseCodes), dashboardRequest, "StockOutDate");
            var transfers = FilterByDate(_transferRepository.GetAllTransfersAsync(dashboardRequest.warehouseCodes), dashboardRequest, "TransferDate");
            var movements = FilterByDate(_movementRepository.GetAllMovementAsync(dashboardRequest.warehouseCodes), dashboardRequest, "MovementDate");
            var purchaseOrders = FilterByDate(_purchaseOrderRepository.GetAllPurchaseOrdersAsync(dashboardRequest.warehouseCodes), dashboardRequest, "OrderDate");
            var stocktakes = FilterByDate(_stocktakeRepository.GetAllStockTakesAsync(dashboardRequest.warehouseCodes), dashboardRequest, "StocktakeDate");

            var statusOrderCodeDTOs = new List<StatusOrderCodeDTO>();

            // STOCK IN
            statusOrderCodeDTOs.Add(new StatusOrderCodeDTO
            {
                OrderTypeCode = "Phiếu nhập",
                CountStatusStart = await stockIns.CountAsync(x => x.StatusId == 1),
                CountStatusInProgress = await stockIns.CountAsync(x => x.StatusId == 2),
                CountStatusCompleted = await stockIns.CountAsync(x => x.StatusId == 3)
            });

            // STOCK OUT
            statusOrderCodeDTOs.Add(new StatusOrderCodeDTO
            {
                OrderTypeCode = "Phiếu xuất",
                CountStatusStart = await stockOuts.CountAsync(x => x.StatusId == 1),
                CountStatusInProgress = await stockOuts.CountAsync(x => x.StatusId == 2),
                CountStatusCompleted = await stockOuts.CountAsync(x => x.StatusId == 3)
            });

            // MANUFACTURING ORDER
            statusOrderCodeDTOs.Add(new StatusOrderCodeDTO
            {
                OrderTypeCode = "Sản xuất",
                CountStatusStart = await manufacturingOrders.CountAsync(x => x.StatusId == 1),
                CountStatusInProgress = await manufacturingOrders.CountAsync(x => x.StatusId == 2),
                CountStatusCompleted = await manufacturingOrders.CountAsync(x => x.StatusId == 3)
            });

            // TRANSFER
            statusOrderCodeDTOs.Add(new StatusOrderCodeDTO
            {
                OrderTypeCode = "Chuyển kho",
                CountStatusStart = await transfers.CountAsync(x => x.StatusId == 1),
                CountStatusInProgress = await transfers.CountAsync(x => x.StatusId == 2),
                CountStatusCompleted = await transfers.CountAsync(x => x.StatusId == 3)
            });

            // MOVEMENT
            statusOrderCodeDTOs.Add(new StatusOrderCodeDTO
            {
                OrderTypeCode = "Chuyển kệ",
                CountStatusStart = await movements.CountAsync(x => x.StatusId == 1),
                CountStatusInProgress = await movements.CountAsync(x => x.StatusId == 2),
                CountStatusCompleted = await movements.CountAsync(x => x.StatusId == 3)
            });

            // PURCHASE ORDER
            statusOrderCodeDTOs.Add(new StatusOrderCodeDTO
            {
                OrderTypeCode = "Đơn đặt hàng",
                CountStatusStart = await purchaseOrders.CountAsync(x => x.StatusId == 1),
                CountStatusInProgress = await purchaseOrders.CountAsync(x => x.StatusId == 2),
                CountStatusCompleted = await purchaseOrders.CountAsync(x => x.StatusId == 3)
            });

            // STOCKTAKE
            statusOrderCodeDTOs.Add(new StatusOrderCodeDTO
            {
                OrderTypeCode = "Kiểm kho",
                CountStatusStart = await stocktakes.CountAsync(x => x.StatusId == 1),
                CountStatusInProgress = await stocktakes.CountAsync(x => x.StatusId == 2),
                CountStatusCompleted = await stocktakes.CountAsync(x => x.StatusId == 3)
            });

            var upcomingDeadlines = new List<UpcomingDeadlineDTO>();
            var today = DateTime.Today;
            var deadlineRange = today.AddDays(3);

            // STOCK IN
            upcomingDeadlines.AddRange(
                await stockIns
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
                }).ToListAsync());

            // STOCK OUT
            upcomingDeadlines.AddRange(
                await stockOuts
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
                }).ToListAsync());

            // MANUFACTURING ORDER
            upcomingDeadlines.AddRange(
                await manufacturingOrders
                .Where(x => x.ScheduleDate.HasValue &&
                            x.ScheduleDate.Value.Date >= today &&
                            x.ScheduleDate.Value.Date <= deadlineRange)
                .Select(x => new UpcomingDeadlineDTO
                {
                    OrderCode = x.ManufacturingOrderCode,
                    ProductCodes = new List<string> { x.ProductCode! },
                    Deadline = x.Deadline!.Value.ToString("yyyy-MM-dd"),
                    StatusName = x.Deadline.Value.Date < today ? "Trễ hạn" : "Còn hạn"
                }).ToListAsync());

            // TRANSFER
            upcomingDeadlines.AddRange(
                await transfers
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
                }).ToListAsync());

            // MOVEMENT
            upcomingDeadlines.AddRange(
                await movements
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
                }).ToListAsync());

            // PURCHASE ORDER
            upcomingDeadlines.AddRange(
                await purchaseOrders
                .Where(x => x.OrderDate.HasValue &&
                            x.OrderDate.Value.Date >= today &&
                            x.OrderDate.Value.Date <= deadlineRange)
                .Select(x => new UpcomingDeadlineDTO
                {
                    OrderCode = x.PurchaseOrderCode,
                    ProductCodes = x.PurchaseOrderDetails
                                     .Select(d => d.ProductCode!)
                                     .Distinct()
                                     .ToList(),
                    Deadline = x.OrderDate!.Value.ToString("yyyy-MM-dd"),
                    StatusName = x.OrderDate.Value.Date < today ? "Trễ hạn" : "Còn hạn"
                }).ToListAsync());

            // STOCKTAKE
            upcomingDeadlines.AddRange(
                await stocktakes
                .Where(x => x.StocktakeDate.HasValue &&
                            x.StocktakeDate.Value.Date >= today &&
                            x.StocktakeDate.Value.Date <= deadlineRange)
                .Select(x => new UpcomingDeadlineDTO
                {
                    OrderCode = x.StocktakeCode,
                    ProductCodes = x.StocktakeDetails
                                     .Select(d => d.ProductCode!)
                                     .Distinct()
                                     .ToList(),
                    Deadline = x.StocktakeDate!.Value.ToString("yyyy-MM-dd"),
                    StatusName = x.StocktakeDate.Value.Date < today ? "Trễ hạn" : "Còn hạn"
                }).ToListAsync());

            var inventoryResponse = await inventorySummary
                .GroupBy(x => x.ProductCode)
                .Select(g => new InventorySummaryDTO
                {
                    ProductCode = g.Key,
                    ProductName = g.First().ProductCodeNavigation!.ProductName ?? "Unknown",
                    CategoryId = g.First().ProductCodeNavigation!.CategoryId??"",
                    CategoryName = g.First().ProductCodeNavigation!.Category!.CategoryName ?? "Unknown",
                    Quantity = g.Sum(x => x.Quantity * (x.ProductCodeNavigation!.BaseQuantity ?? 1)),
                    BaseQuantity = g.Sum(x => x.Quantity),
                    UOM = g.First().ProductCodeNavigation!.Uom ?? "Unknown",
                    BaseUOM = g.First().ProductCodeNavigation!.BaseUom ?? "Unknown"
                }).ToListAsync();

            var dashBoardResponse = new DashboardResponseDTO
            {
                InventorySummaryDTOs = inventoryResponse,
                ProgressManufacturingOrderDTOs = await manufacturingOrders
                    .OrderByDescending(x => x.ScheduleDate)
                    .Take(5)
                    .Select(x => new ProgressManufacturingOrderDTO
                    {
                        ManufacturingOrderCode = x.ManufacturingOrderCode,
                        ProductCode = x.ProductCode!,
                        ProductName = x.ProductCodeNavigation!.ProductName ?? "Unknown",
                        Quantity = x.Quantity,
                        QuantityProduced = x.QuantityProduced,
                        StatusName = x.Status!.StatusName ?? "Unknown",
                        WarehouseCode = x.WarehouseCode!,
                        Progress = x.QuantityProduced.HasValue && x.Quantity > 0
                            ? (double)x.QuantityProduced / x.Quantity * 100
                            : 0
                    }).ToListAsync(),
                QuantityToCompleteStockIn = await stockIns.Where(x => x.StatusId == 2 && x.StockInDetails.All(x => x.Demand == x.Quantity)).CountAsync(),
                QuantityToCompleteStockOut = await stockOuts.Where(x => x.StatusId == 2 && x.StockOutDetails.All(x => x.Demand == x.Quantity)).CountAsync(),
                QuantityToCompleteManufacturingOrder = await manufacturingOrders.Where(x => x.StatusId == 2 && x.Quantity == x.QuantityProduced).CountAsync(),
                QuantityToCompletePurchaseOrder = await purchaseOrders.Where(x => x.StatusId == 2 && x.PurchaseOrderDetails.All(x => x.Quantity == x.QuantityReceived)).CountAsync(),
                QuantityToCompleteStocktake = await stocktakes.Where(x => x.StatusId == 2).CountAsync(),
                UpcomingDeadlines = upcomingDeadlines,
                StatusOrderCodeDTOs = statusOrderCodeDTOs
            };

            return new ServiceResponse<DashboardResponseDTO>(true, "Lấy dữ liệu thành công", dashBoardResponse);
        }

        public async Task<ServiceResponse<DashboardStockInOutSummaryDTO>> GetStockInOutSummaryAsync(DashboardRequestDTO dashboardRequest)
        {
            if (dashboardRequest == null)
                return new ServiceResponse<DashboardStockInOutSummaryDTO>(false, "Dữ liệu nhận vào không hợp lệ");

            var today = DateTime.Today;
            var startOfYear = new DateTime(today.Year, 1, 1);
            var startOfQuarter = new DateTime(today.Year, ((today.Month - 1) / 3 * 3) + 1, 1);
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            var stockIns = FilterByDate(_stockInRepository.GetAllStockInAsync(dashboardRequest.warehouseCodes), dashboardRequest, "OrderDeadline");
            var stockOuts = FilterByDate(_stockOutRepository.GetAllStockOutAsync(dashboardRequest.warehouseCodes), dashboardRequest, "StockOutDate");
            var purchaseOrders = FilterByDate(_purchaseOrderRepository.GetAllPurchaseOrdersAsync(dashboardRequest.warehouseCodes), dashboardRequest, "OrderDate");
            var stocktakes = FilterByDate(_stocktakeRepository.GetAllStockTakesAsync(dashboardRequest.warehouseCodes), dashboardRequest, "StocktakeDate");

            var result = new DashboardStockInOutSummaryDTO
            {
                StockInThisMonth = await stockIns.CountAsync(x => x.OrderDeadline >= startOfMonth),
                StockInThisQuarter = await stockIns.CountAsync(x => x.OrderDeadline >= startOfQuarter),
                StockInThisYear = await stockIns.CountAsync(x => x.OrderDeadline >= startOfYear),
                StockOutThisMonth = await stockOuts.CountAsync(x => x.StockOutDate >= startOfMonth),
                StockOutThisQuarter = await stockOuts.CountAsync(x => x.StockOutDate >= startOfQuarter),
                StockOutThisYear = await stockOuts.CountAsync(x => x.StockOutDate >= startOfYear),
                PurchaseOrderThisMonth = await purchaseOrders.CountAsync(x => x.OrderDate >= startOfMonth),
                PurchaseOrderThisQuarter = await purchaseOrders.CountAsync(x => x.OrderDate >= startOfQuarter),
                PurchaseOrderThisYear = await purchaseOrders.CountAsync(x => x.OrderDate >= startOfYear),
                StocktakeThisMonth = await stocktakes.CountAsync(x => x.StocktakeDate >= startOfMonth),
                StocktakeThisQuarter = await stocktakes.CountAsync(x => x.StocktakeDate >= startOfQuarter),
                StocktakeThisYear = await stocktakes.CountAsync(x => x.StocktakeDate >= startOfYear),
                MonthlyStockInOuts = await Task.Run(async () =>
                {
                    var monthlyStockInOuts = new List<MonthlyStockInOutDTO>();
                    for (int month = 1; month <= 12; month++)
                    {
                        var startOfMonth = new DateTime(today.Year, month, 1);
                        monthlyStockInOuts.Add(new MonthlyStockInOutDTO
                        {
                            Month = startOfMonth.ToString("MM/yyyy"),
                            StockInCount = await stockIns.CountAsync(x => x.OrderDeadline != null && x.OrderDeadline.Value.Month == month && x.OrderDeadline.Value.Year == today.Year),
                            StockOutCount = await stockOuts.CountAsync(x => x.StockOutDate != null && x.StockOutDate.Value.Month == month && x.StockOutDate.Value.Year == today.Year),
                            PurchaseOrderCount = await purchaseOrders.CountAsync(x => x.OrderDate != null && x.OrderDate.Value.Month == month && x.OrderDate.Value.Year == today.Year),
                            StocktakeCount = await stocktakes.CountAsync(x => x.StocktakeDate != null && x.StocktakeDate.Value.Month == month && x.StocktakeDate.Value.Year == today.Year)
                        });
                    }
                    return monthlyStockInOuts;
                })
            };

            return new ServiceResponse<DashboardStockInOutSummaryDTO>(true, "Thành công", result);
        }

        public async Task<ServiceResponse<HandyDashboardDTO>> GetHandyDashboardAsync(HandyDashboardRequestDTO dashboardRequest)
        {
            if (dashboardRequest == null || dashboardRequest.warehouseCodes == null || !dashboardRequest.warehouseCodes.Any())
                return new ServiceResponse<HandyDashboardDTO>(false, "Dữ liệu đầu vào không hợp lệ");

            var today = DateTime.Today;
            var deadlineRange = today.AddDays(3);

            var stockIns = FilterByDateHandy(_stockInRepository.GetAllStockInAsync(dashboardRequest.warehouseCodes), dashboardRequest, "OrderDeadline");
            var stockOuts = FilterByDateHandy(_stockOutRepository.GetAllStockOutAsync(dashboardRequest.warehouseCodes), dashboardRequest, "StockOutDate");
            var manufacturingOrders = FilterByDateHandy(_manufacturingOrderRepository.GetAllManufacturingOrder(dashboardRequest.warehouseCodes), dashboardRequest, "ScheduleDate");
            var transfers = FilterByDateHandy(_transferRepository.GetAllTransfersAsync(dashboardRequest.warehouseCodes), dashboardRequest, "TransferDate");
            var movements = FilterByDateHandy(_movementRepository.GetAllMovementAsync(dashboardRequest.warehouseCodes), dashboardRequest, "MovementDate");

            var userCode = dashboardRequest.userName;

            var alerts = new List<string>();

            alerts.AddRange(await stockIns
                .Where(x => x.OrderDeadline.HasValue &&
                            x.OrderDeadline.Value.Date < today &&
                            x.StatusId != 3 &&
                            x.Responsible == userCode)
                .Select(x => $"Đơn nhập kho {x.StockInCode} t\nrễ hạn ({x.OrderDeadline!.Value:dd-MM-yyyy})")
                .ToListAsync());

            alerts.AddRange(await stockOuts
                .Where(x => x.StockOutDate.HasValue &&
                            x.StockOutDate.Value.Date < today &&
                            x.StatusId != 3 &&
                            x.Responsible == userCode)
                .Select(x => $"Đơn xuất kho {x.StockOutCode} \ntrễ hạn ({x.StockOutDate!.Value:dd-MM-yyyy})")
                .ToListAsync());

            alerts.AddRange(await transfers
                .Where(x => x.TransferDate.HasValue &&
                            x.TransferDate.Value.Date < today &&
                            x.StatusId != 3 &&
                            (x.FromResponsible == userCode || x.ToResponsible == userCode))
                .Select(x => $"Phiếu chuyển kho {x.TransferCode} \ntrễ hạn ({x.TransferDate!.Value:dd-MM-yyyy})")
                .ToListAsync());

            alerts.AddRange(await movements
                .Where(x => x.MovementDate.HasValue &&
                            x.MovementDate.Value.Date < today &&
                            x.StatusId != 3 &&
                            x.Responsible == userCode)
                .Select(x => $"Phiếu chuyển kệ {x.MovementCode} \ntrễ hạn ({x.MovementDate!.Value:dd-MM-yyyy})")
                .ToListAsync());

            alerts.AddRange(await manufacturingOrders
                .Where(x => x.ScheduleDate.HasValue &&
                            x.ScheduleDate.Value.Date < today &&
                            x.StatusId != 3 &&
                            x.Responsible == userCode)
                .Select(x => $"Lệnh sản xuất {x.ManufacturingOrderCode} \ntrễ hạn ({x.Deadline!.Value:dd-MM-yyyy})")
                .ToListAsync());

            var todoTasks = new List<HandyTaskDTO>();

            // NHẬP KHO
            todoTasks.AddRange(await stockIns
                .Where(x => x.OrderDeadline.HasValue &&
                            x.OrderDeadline.Value.Date <= today &&
                            x.OrderDeadline.Value.Date <= deadlineRange &&
                            x.StatusId != 3 &&
                            x.Responsible == userCode)
                .Select(x => new HandyTaskDTO
                {
                    OrderCode = x.StockInCode,
                    OrderType = "Nhập kho",
                    Deadline = x.OrderDeadline!.Value.ToString("yyyy-MM-dd"),
                    Status = x.Status!.StatusName!,
                    ProductCodes = x.StockInDetails.Select(d => d.ProductCode!).Distinct().ToList()
                }).ToListAsync());

            // XUẤT KHO
            todoTasks.AddRange(await stockOuts
                .Where(x => x.StockOutDate.HasValue &&
                            x.StockOutDate.Value.Date <= today &&
                            x.StockOutDate.Value.Date <= deadlineRange &&
                            x.StatusId != 3 &&
                            x.Responsible == userCode)
                .Select(x => new HandyTaskDTO
                {
                    OrderCode = x.StockOutCode,
                    OrderType = "Xuất kho",
                    Deadline = x.StockOutDate!.Value.ToString("yyyy-MM-dd"),
                    Status = x.Status!.StatusName!,
                    ProductCodes = x.StockOutDetails.Select(d => d.ProductCode!).Distinct().ToList()
                }).ToListAsync());

            // CHUYỂN KHO
            todoTasks.AddRange(await transfers
                .Where(x => x.TransferDate.HasValue &&
                            x.TransferDate.Value.Date <= today &&
                            x.TransferDate.Value.Date <= deadlineRange &&
                            x.StatusId != 3 &&(
                            x.ToResponsible == userCode || x.FromResponsible ==userCode))
                .Select(x => new HandyTaskDTO
                {
                    OrderCode = x.TransferCode,
                    OrderType = "Chuyển kho",
                    Deadline = x.TransferDate!.Value.ToString("yyyy-MM-dd"),
                    Status = x.Status!.StatusName!,
                    ProductCodes = x.TransferDetails.Select(d => d.ProductCode!).Distinct().ToList()
                }).ToListAsync());

            // CHUYỂN KỆ
            todoTasks.AddRange(await movements
                .Where(x => x.MovementDate.HasValue &&
                            x.MovementDate.Value.Date <= today &&
                            x.MovementDate.Value.Date <= deadlineRange &&
                            x.StatusId != 3 &&
                            x.Responsible == userCode)
                .Select(x => new HandyTaskDTO
                {
                    OrderCode = x.MovementCode,
                    OrderType = "Chuyển kệ",
                    Deadline = x.MovementDate!.Value.ToString("yyyy-MM-dd"),
                    Status = x.Status!.StatusName!,
                    ProductCodes = x.MovementDetails.Select(d => d.ProductCode!).Distinct().ToList()
                }).ToListAsync());

            // SẢN XUẤT
            todoTasks.AddRange(await manufacturingOrders
                .Where(x => x.ScheduleDate.HasValue &&
                            x.ScheduleDate.Value.Date <= today &&
                            x.ScheduleDate.Value.Date <= deadlineRange &&
                            x.StatusId != 3 &&
                            x.Responsible == userCode)
                .Select(x => new HandyTaskDTO
                {
                    OrderCode = x.ManufacturingOrderCode,
                    OrderType = "Sản xuất",
                    Deadline = x.Deadline!.Value.ToString("yyyy-MM-dd"),
                    Status = x.Status!.StatusName!,
                    ProductCodes = new List<string> { x.ProductCode! }
                }).ToListAsync());

            // SUMMARY
            var summary = new Dictionary<string, int>
            {
                ["Nhập kho"] = await stockIns.CountAsync(x =>
                    x.OrderDeadline.HasValue &&
                    (
                        (x.OrderDeadline.Value.Date < today && x.StatusId != 3) || // Trễ hạn & chưa hoàn thành
                        x.OrderDeadline.Value.Date == today                         // Hôm nay
                    )
                    && x.Responsible == userCode
                ),

                ["Xuất kho"] = await stockOuts.CountAsync(x =>
                    x.StockOutDate.HasValue &&
                    (
                        (x.StockOutDate.Value.Date < today && x.StatusId != 3) ||
                        x.StockOutDate.Value.Date == today
                    )
                    && x.Responsible == userCode
                ),

                ["Chuyển kho"] = await transfers.CountAsync(x =>
                    x.TransferDate.HasValue &&
                    (
                        (x.TransferDate.Value.Date < today && x.StatusId != 3) ||
                        x.TransferDate.Value.Date == today
                    )
                    && (x.FromResponsible == userCode || x.ToResponsible == userCode)
                ),

                ["Chuyển kệ"] = await movements.CountAsync(x =>
                    x.MovementDate.HasValue &&
                    (
                        (x.MovementDate.Value.Date < today && x.StatusId != 3) ||
                        x.MovementDate.Value.Date == today
                    )
                    && x.Responsible == userCode
                ),

                ["Sản xuất"] = await manufacturingOrders.CountAsync(x =>
                    x.ScheduleDate.HasValue &&
                    (
                        (x.ScheduleDate.Value.Date < today && x.StatusId != 3) ||
                        x.ScheduleDate.Value.Date == today
                    )
                    && x.Responsible == userCode
                ),
            };


            var result = new HandyDashboardDTO
            {
                Alerts = alerts,
                TodoTasks = todoTasks.OrderBy(x => x.Deadline).ToList(),
                SummaryToday = summary
            };

            return new ServiceResponse<HandyDashboardDTO>(true, "Lấy dashboard thành công", result);
        }


        private IQueryable<T> FilterByDate<T>(IQueryable<T> query, DashboardRequestDTO request, string dateProperty) where T : class
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            // If no filters are provided, return the original query
            if (!request.Year.HasValue && !request.Month.HasValue && !request.Quarter.HasValue)
                return query;

            // Year filter: Filter by the entire year
            if (request.Year.HasValue && !request.Month.HasValue && !request.Quarter.HasValue)
            {
                query = query.Where(x =>
                    EF.Property<DateTime?>(x, dateProperty) != null &&
                    EF.Property<DateTime?>(x, dateProperty)!.Value.Year == request.Year.Value);
            }
            // Quarter filter: Filter by the specified quarter within the year (if provided)
            else if (request.Quarter.HasValue && !request.Month.HasValue)
            {
                var (startMonth, endMonth) = request.Quarter.Value switch
                {
                    1 => (1, 3),   // Q1: January - March
                    2 => (4, 6),   // Q2: April - June
                    3 => (7, 9),   // Q3: July - September
                    4 => (10, 12), // Q4: October - December
                    _ => throw new ArgumentException("Dữ liệu quý không hợp lệ, 1,2,3")
                };

                query = query.Where(x =>
                    EF.Property<DateTime?>(x, dateProperty) != null &&
                    EF.Property<DateTime?>(x, dateProperty)!.Value.Month >= startMonth &&
                    EF.Property<DateTime?>(x, dateProperty)!.Value.Month <= endMonth &&
                    (!request.Year.HasValue || EF.Property<DateTime?>(x, dateProperty)!.Value.Year == request.Year.Value));
            }
            // Month filter: Filter by the entire month within the year (if provided)
            else if (request.Month.HasValue)
            {
                query = query.Where(x =>
                    EF.Property<DateTime?>(x, dateProperty) != null &&
                    EF.Property<DateTime?>(x, dateProperty)!.Value.Month == request.Month.Value &&
                    (!request.Year.HasValue || EF.Property<DateTime?>(x, dateProperty)!.Value.Year == request.Year.Value));
            }

            return query;
        }
        private IQueryable<T> FilterByDateHandy<T>(IQueryable<T> query, HandyDashboardRequestDTO request, string dateProperty) where T : class
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            // If no filters are provided, return the original query
            if (!request.Year.HasValue && !request.Month.HasValue && !request.Quarter.HasValue)
                return query;

            // Year filter: Filter by the entire year
            if (request.Year.HasValue && !request.Month.HasValue && !request.Quarter.HasValue)
            {
                query = query.Where(x =>
                    EF.Property<DateTime?>(x, dateProperty) != null &&
                    EF.Property<DateTime?>(x, dateProperty)!.Value.Year == request.Year.Value);
            }
            // Quarter filter: Filter by the specified quarter within the year (if provided)
            else if (request.Quarter.HasValue && !request.Month.HasValue)
            {
                var (startMonth, endMonth) = request.Quarter.Value switch
                {
                    1 => (1, 3),   // Q1: January - March
                    2 => (4, 6),   // Q2: April - June
                    3 => (7, 9),   // Q3: July - September
                    4 => (10, 12), // Q4: October - December
                    _ => throw new ArgumentException("Dữ liệu quý không hợp lệ, 1,2,3")
                };

                query = query.Where(x =>
                    EF.Property<DateTime?>(x, dateProperty) != null &&
                    EF.Property<DateTime?>(x, dateProperty)!.Value.Month >= startMonth &&
                    EF.Property<DateTime?>(x, dateProperty)!.Value.Month <= endMonth &&
                    (!request.Year.HasValue || EF.Property<DateTime?>(x, dateProperty)!.Value.Year == request.Year.Value));
            }
            // Month filter: Filter by the entire month within the year (if provided)
            else if (request.Month.HasValue)
            {
                query = query.Where(x =>
                    EF.Property<DateTime?>(x, dateProperty) != null &&
                    EF.Property<DateTime?>(x, dateProperty)!.Value.Month == request.Month.Value &&
                    (!request.Year.HasValue || EF.Property<DateTime?>(x, dateProperty)!.Value.Year == request.Year.Value));
            }

            return query;
        }
    }

    
}