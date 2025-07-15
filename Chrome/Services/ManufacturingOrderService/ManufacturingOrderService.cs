using Chrome.DTO;
using Chrome.DTO.AccountManagementDTO;
using Chrome.DTO.BOMMasterDTO;
using Chrome.DTO.ManufacturingOrderDTO;
using Chrome.DTO.OrderTypeDTO;
using Chrome.DTO.PickListDTO;
using Chrome.DTO.ProductMasterDTO;
using Chrome.DTO.PutAwayDTO;
using Chrome.DTO.ReservationDTO;
using Chrome.DTO.StatusMasterDTO;
using Chrome.DTO.WarehouseMasterDTO;
using Chrome.Models;
using Chrome.Repositories.AccountRepository;
using Chrome.Repositories.BOMComponentRepository;
using Chrome.Repositories.BOMMasterRepository;
using Chrome.Repositories.InventoryRepository;
using Chrome.Repositories.ManufacturingOrderDetailRepository;
using Chrome.Repositories.ManufacturingOrderRepository;
using Chrome.Repositories.OrderTypeRepository;
using Chrome.Repositories.ProductMasterRepository;
using Chrome.Repositories.PutAwayRulesRepository;
using Chrome.Repositories.StatusMasterRepository;
using Chrome.Repositories.WarehouseMasterRepository;
using Chrome.Services.PickListService;
using Chrome.Services.PutAwayService;
using Chrome.Services.ReservationService;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;

namespace Chrome.Services.ManufacturingOrderService
{
    public class ManufacturingOrderService : IManufacturingOrderService
    {
        private readonly IManufacturingOrderRepository _manufacturingOrderRepository;
        private readonly IManufacturingOrderDetailRepository _manufacturingOrderDetailRepository;
        private readonly IOrderTypeRepository _orderTypeRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IStatusMasterRepository _statusMasterRepository;
        private readonly IWarehouseMasterRepository _warehouseMasterRepository;
        private readonly IBOMMasterRepository _bomMasterRepository;
        private readonly IBOMComponentRepository _bomComponentRepository;
        private readonly IPutAwayRulesRepository _putAwayRulesRepository;
        private readonly IPutAwayService _putAwayService;
        private readonly IProductMasterRepository _productMasterRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IReservationService _reservationService;
        private readonly IPickListService _pickListService;
        private readonly ChromeContext _context;

        public ManufacturingOrderService(
            IManufacturingOrderRepository manufacturingOrderRepository,
            IManufacturingOrderDetailRepository manufacturingOrderDetailRepository,
            IOrderTypeRepository orderTypeRepository,
            IAccountRepository accountRepository,
            IStatusMasterRepository statusMasterRepository,
            IWarehouseMasterRepository warehouseMasterRepository,
            IBOMMasterRepository bomMasterRepository,
            IBOMComponentRepository bomComponentRepository,
            IPutAwayRulesRepository putAwayRulesRepository,
            IPutAwayService putAwayService,
            IProductMasterRepository productMasterRepository,
            IInventoryRepository inventoryRepository,
            IReservationService reservationService,
            IPickListService pickListService,
            ChromeContext context)
        {
            _manufacturingOrderRepository = manufacturingOrderRepository;
            _manufacturingOrderDetailRepository = manufacturingOrderDetailRepository;
            _orderTypeRepository = orderTypeRepository;
            _accountRepository = accountRepository;
            _statusMasterRepository = statusMasterRepository;
            _warehouseMasterRepository = warehouseMasterRepository;
            _bomMasterRepository = bomMasterRepository;
            _bomComponentRepository = bomComponentRepository;
            _putAwayRulesRepository = putAwayRulesRepository;
            _putAwayService = putAwayService;
            _productMasterRepository = productMasterRepository;
            _inventoryRepository = inventoryRepository;
            _reservationService = reservationService;
            _pickListService = pickListService;
            _context = context;
        }
        public async Task<ServiceResponse<List<ProductShortageDTO>>> CheckInventoryShortageForManufacturingOrderAsync(ManufacturingOrderRequestDTO manufacturingOrder)
        {
            try
            {
                // Lấy phiên bản BOM hoạt động
                var lstBomVersion = await _bomMasterRepository.GetListVersionByBomCode(manufacturingOrder.Bomcode);
                var bomVersionActived = lstBomVersion.FirstOrDefault(x => x.IsActive == true);
                if (bomVersionActived == null)
                    return new ServiceResponse<List<ProductShortageDTO>>(false, $"Không tìm thấy phiên bản BOM hoạt động cho mã {manufacturingOrder.Bomcode}.");

                // Lấy chi tiết BOM
                var bomComponents = await _bomComponentRepository.GetAllBOMComponent(manufacturingOrder.Bomcode, bomVersionActived.Bomversion);
                if (bomComponents == null || !bomComponents.Any())
                    return new ServiceResponse<List<ProductShortageDTO>>(false, $"Không tìm thấy chi tiết BOM cho mã {manufacturingOrder.Bomcode} và phiên bản {bomVersionActived.Bomversion}.");

                var shortageList = new List<ProductShortageDTO>();

                foreach (var component in bomComponents)
                {
                    var requiredQuantity = (component.ConsumpQuantity * manufacturingOrder.Quantity ) * (1+component.ScrapRate);
                    if (requiredQuantity <= 0) continue;

                    // Tồn kho theo FIFO ReceiveDate
                    var inventories = await _inventoryRepository.GetInventoryByProductCodeAsync(component.ComponentCode, manufacturingOrder.WarehouseCode!)
                        .OrderBy(i => i.ReceiveDate)
                        .ToListAsync();

                    double remainingQuantity = (double)requiredQuantity!;
                    
                    foreach (var inventory in inventories)
                    {
                        var product = await _productMasterRepository.GetProductMasterWithProductCode(inventory.ProductCode);
                        var reservedQuantity = await _context.ReservationDetails
                            .Include(rd => rd.ReservationCodeNavigation)
                            
                            .Where(rd => rd.ProductCode == inventory.ProductCode &&
                                         rd.LotNo == inventory.Lotno &&
                                         rd.ReservationCodeNavigation!.StatusId != 3)
                            .SumAsync(rd => (double?)rd.QuantityReserved)*product.BaseQuantity ?? 0;

                        var availableQuantity = (inventory.Quantity ?? 0) - reservedQuantity;

                        if (availableQuantity > 0)
                        {
                            remainingQuantity -= availableQuantity;
                            if (remainingQuantity <= 0) break;
                        }
                    }

                    if (remainingQuantity > 0)
                    {
                        shortageList.Add(new ProductShortageDTO
                        {
                            ProductCode = component.ComponentCode,
                            ProductName = (await _context.ProductMasters
                                .Where(p => p.ProductCode == component.ComponentCode)
                                .Select(p => p.ProductName)
                                .FirstOrDefaultAsync()) ?? component.ComponentCode,
                            RequiredQuantity = (double)requiredQuantity,
                            ShortageQuantity = remainingQuantity,
                            WarehouseCode = manufacturingOrder.WarehouseCode
                        });
                    }
                }

                if (!shortageList.Any())
                {
                    return new ServiceResponse<List<ProductShortageDTO>>(true, "Đủ tồn kho cho tất cả thành phần", shortageList);
                }

                return new ServiceResponse<List<ProductShortageDTO>>(true, "Danh sách thành phần thiếu tồn kho", shortageList);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<ProductShortageDTO>>(false, $"Lỗi kiểm tra tồn kho: {ex.Message}");
            }
        }
        public async Task<ServiceResponse<bool>> CheckInventory (ManufacturingOrderRequestDTO manufacturingOrder)
        {
            // 7. Kiểm tra tồn kho bằng hàm kiểm tra tồn kho tổng hợp đã viết sẵn
            var shortageCheck = await CheckInventoryShortageForManufacturingOrderAsync(manufacturingOrder);
            if (!shortageCheck.Success)
                return new ServiceResponse<bool>(false, $"Lỗi kiểm tra tồn kho: {shortageCheck.Message}");

            // Nếu thiếu tồn kho → thông báo lỗi, không tạo tiếp
            if (shortageCheck.Data != null && shortageCheck.Data.Any())
            {
                var shortageList = string.Join("\n ", shortageCheck.Data.Select(x => $"{x.ProductCode} (thiếu {x.ShortageQuantity})"));
                return new ServiceResponse<bool>(false, $"Không đủ tồn kho cho các thành phần: \n {shortageList}");
            }
            return new ServiceResponse<bool>(true, $"Đủ tồn kho để sản xuất cho mã {manufacturingOrder.ProductCode} với số lượng:{manufacturingOrder.Quantity}");
        }
        public async Task<ServiceResponse<bool>> CheckQuantityWithBase(ManufacturingOrderRequestDTO manufacturingOrder)
        {
            // 4. Lấy phiên bản BOM đang hoạt động
            var lstBomVersion = await _bomMasterRepository.GetListVersionByBomCode(manufacturingOrder.Bomcode);
            var bomVersionActived = lstBomVersion.FirstOrDefault(x => x.IsActive == true);
            if (bomVersionActived == null)
                return new ServiceResponse<bool>(false, $"Không tìm thấy phiên bản BOM hoạt động cho mã {manufacturingOrder.Bomcode}.");

            // 5. Lấy danh sách chi tiết BOM
            var bomComponents = await _bomComponentRepository.GetAllBOMComponent(manufacturingOrder.Bomcode, bomVersionActived.Bomversion);
            if (bomComponents == null || !bomComponents.Any())
                return new ServiceResponse<bool>(false, $"Không tìm thấy chi tiết BOM cho mã {manufacturingOrder.Bomcode} và phiên bản {bomVersionActived.Bomversion}.");

            // 6. Tạo danh sách ManufacturingOrderDetail từ BOM
            var manufacturingDetails = new List<ManufacturingOrderDetail>();
            foreach (var bomComponent in bomComponents)
            {
                // Lấy thông tin chi tiết từng component (bao gồm ScrapRate)
                var componentDetail = await _bomComponentRepository.GetBomComponent(bomComponent.Bomcode, bomComponent.ComponentCode, bomComponent.BomVersion);

                manufacturingDetails.Add(new ManufacturingOrderDetail
                {
                    ManufacturingOrderCode = manufacturingOrder.ManufacturingOrderCode,
                    ComponentCode = bomComponent.ComponentCode,
                    ToConsumeQuantity = (bomComponent.ConsumpQuantity * manufacturingOrder.Quantity) * (1 + bomComponent.ScrapRate),
                    ConsumedQuantity = 0,
                    ScraptRate = componentDetail?.ScrapRate ?? 0
                });
            }
            // 1. Lấy BaseQuantity dictionary
            var productBaseQtyDict = _context.ProductMasters
                .Where(p => p.BaseQuantity != null)
                .ToDictionary(p => p.ProductCode, p => p.BaseQuantity!.Value);

            // 2. Lấy thông tin số lượng
            int currentMOQty = (int)manufacturingOrder.Quantity!;
            int suggestedMOQty = currentMOQty;
            string productCode = manufacturingOrder.ProductCode;

            // 3. Lấy baseQty của sản phẩm chính
            productBaseQtyDict.TryGetValue(productCode, out double mainProductBaseQty);
            if (mainProductBaseQty == 0) mainProductBaseQty = 1;

            // 4. Hàm kiểm tra hợp lệ số lượng
            bool IsValidMOQty(int moQty)
            {
                // Kiểm tra sản phẩm chính
                if (moQty % mainProductBaseQty != 0)
                    return false;

                // Kiểm tra component
                return manufacturingDetails.All(x =>
                {
                    productBaseQtyDict.TryGetValue(x.ComponentCode, out double baseQty);
                    if (baseQty == 0) baseQty = 1;

                    var bomQty = bomComponents.FirstOrDefault(b => b.ComponentCode == x.ComponentCode)?.ConsumpQuantity ?? 1;
                    var totalConsume = (bomQty * moQty) * (1 + x.ScraptRate);

                    return (totalConsume % baseQty) == 0;
                });
            }

            // 5. Tìm số lượng gần nhất chia hết
            while (!IsValidMOQty(suggestedMOQty))
            {
                suggestedMOQty++;
            }

            // 6. Nếu số lượng không hợp lệ → sinh thông báo chi tiết
            if (suggestedMOQty != currentMOQty)
            {
                var invalidComponents = manufacturingDetails
                    .Select(x =>
                    {
                        productBaseQtyDict.TryGetValue(x.ComponentCode, out double baseQty);
                        if (baseQty == 0) baseQty = 1;

                        var bomQty = bomComponents.FirstOrDefault(b => b.ComponentCode == x.ComponentCode)?.ConsumpQuantity ?? 1;
                        var totalConsume = (bomQty * currentMOQty) * (1 + x.ScraptRate);
                        var convertedQty = totalConsume / baseQty;

                        return new
                        {
                            x.ComponentCode,
                            ToConsumeQty = Math.Round((double)x.ToConsumeQuantity!, 3),
                            BaseQty = baseQty,
                            ConvertedQty = Math.Round((double)convertedQty!, 3),
                            IsInvalid = (totalConsume % baseQty) != 0
                        };
                    })
                    .Where(x => x.IsInvalid)
                    .ToList();

                // Kiểm tra sản phẩm chính
                if (currentMOQty % mainProductBaseQty != 0)
                {
                    invalidComponents.Add(new
                    {
                        ComponentCode = productCode,
                        ToConsumeQty = (double)currentMOQty,
                        BaseQty = mainProductBaseQty,
                        ConvertedQty = Math.Round(currentMOQty / mainProductBaseQty, 3),
                        IsInvalid = true
                    });
                }

                // 7. Tách lỗi sản phẩm và nguyên vật liệu
                var productInvalid = invalidComponents
                    .Where(x => x.ComponentCode == productCode)
                    .Select(x => $"{x.ComponentCode} (sản phẩm chính): Số lượng sản xuất {x.ToConsumeQty} không chia hết cho BaseQty ({x.BaseQty}) → {x.ConvertedQty}");

                var componentInvalid = invalidComponents
                    .Where(x => x.ComponentCode != productCode)
                    .Select(x => $"{x.ComponentCode}: Tiêu thụ {x.ToConsumeQty} (chia cho base {x.BaseQty}) = {x.ConvertedQty} → không chia hết cho BaseQty");

                // 8. Xây thông báo
                var messageBuilder = new StringBuilder();
                messageBuilder.AppendLine($"⚠️ Số lượng sản xuất hiện tại là {currentMOQty} khiến một số thành phần không chia hết theo BaseQuantity:");

                if (productInvalid.Any())
                {
                    messageBuilder.AppendLine("\n🟥 Lỗi ở sản phẩm chính:");
                    foreach (var msg in productInvalid)
                        messageBuilder.AppendLine(" - " + msg);
                }

                if (componentInvalid.Any())
                {
                    messageBuilder.AppendLine("\n🟧 Lỗi ở nguyên vật liệu (component):");
                    foreach (var msg in componentInvalid)
                        messageBuilder.AppendLine(" - " + msg);
                }

                messageBuilder.AppendLine($"\n✅ Gợi ý: Tăng lên {suggestedMOQty} để toàn bộ chia hết theo BaseQuantity.");
                messageBuilder.AppendLine($"\nBạn có muốn tiếp tục tạo lệnh nếu bị lẻ số lượng so với BaseQuantity hay không?");

                // 9. Trả lỗi
                return new ServiceResponse<bool>(false, messageBuilder.ToString());
            }

            return new ServiceResponse<bool>(true, "Số lượng đã phù hợp để sản xuất");

        }

        public async Task<ServiceResponse<bool>> AddManufacturingOrderAsync(ManufacturingOrderRequestDTO manufacturingOrder)
        {
            // 1. Kiểm tra dữ liệu đầu vào
            if (manufacturingOrder == null ||
                string.IsNullOrEmpty(manufacturingOrder.ManufacturingOrderCode) ||
                string.IsNullOrEmpty(manufacturingOrder.ProductCode) ||
                string.IsNullOrEmpty(manufacturingOrder.WarehouseCode) ||
                string.IsNullOrEmpty(manufacturingOrder.Bomcode) ||
                string.IsNullOrEmpty(manufacturingOrder.OrderTypeCode) ||
                string.IsNullOrEmpty(manufacturingOrder.Responsible) ||
                manufacturingOrder.Quantity <= 0)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            // 2. Kiểm tra định dạng ngày tháng
            string[] formats = { "M/d/yyyy h:mm:ss tt", "MM/dd/yyyy hh:mm:ss tt", "dd/MM/yyyy" };

            if (!DateTime.TryParseExact(manufacturingOrder.ScheduleDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedScheduleDate))
                return new ServiceResponse<bool>(false, "Ngày bắt đầu sản xuất không đúng định dạng.");

            if (!DateTime.TryParseExact(manufacturingOrder.Deadline, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDeadline))
                return new ServiceResponse<bool>(false, "Ngày hết hạn sản xuất không đúng định dạng.");

            // 3. Kiểm tra ngày hợp lệ
            if (parsedScheduleDate > parsedDeadline)
                return new ServiceResponse<bool>(false, "Ngày bắt đầu sản xuất không thể lớn hơn ngày hết hạn.");

            // 4. Lấy phiên bản BOM đang hoạt động
            var lstBomVersion = await _bomMasterRepository.GetListVersionByBomCode(manufacturingOrder.Bomcode);
            var bomVersionActived = lstBomVersion.FirstOrDefault(x => x.IsActive == true);
            if (bomVersionActived == null)
                return new ServiceResponse<bool>(false, $"Không tìm thấy phiên bản BOM hoạt động cho mã {manufacturingOrder.Bomcode}.");

            // 5. Lấy danh sách chi tiết BOM
            var bomComponents = await _bomComponentRepository.GetAllBOMComponent(manufacturingOrder.Bomcode, bomVersionActived.Bomversion);
            if (bomComponents == null || !bomComponents.Any())
                return new ServiceResponse<bool>(false, $"Không tìm thấy chi tiết BOM cho mã {manufacturingOrder.Bomcode} và phiên bản {bomVersionActived.Bomversion}.");

            // 6. Tạo danh sách ManufacturingOrderDetail từ BOM
            var manufacturingDetails = new List<ManufacturingOrderDetail>();
            foreach (var bomComponent in bomComponents)
            {
                // Lấy thông tin chi tiết từng component (bao gồm ScrapRate)
                var componentDetail = await _bomComponentRepository.GetBomComponent(bomComponent.Bomcode, bomComponent.ComponentCode, bomComponent.BomVersion);

                manufacturingDetails.Add(new ManufacturingOrderDetail
                {
                    ManufacturingOrderCode = manufacturingOrder.ManufacturingOrderCode,
                    ComponentCode = bomComponent.ComponentCode,
                    ToConsumeQuantity = (bomComponent.ConsumpQuantity * manufacturingOrder.Quantity)  * (1+bomComponent.ScrapRate),
                    ConsumedQuantity = 0,
                    ScraptRate = componentDetail?.ScrapRate ?? 0
                });
            }

            // 8. Tạo bản ghi ManufacturingOrder
            var manufacturing = new ManufacturingOrder
            {
                ManufacturingOrderCode = manufacturingOrder.ManufacturingOrderCode,
                OrderTypeCode = manufacturingOrder.OrderTypeCode,
                ProductCode = manufacturingOrder.ProductCode,
                Bomcode = manufacturingOrder.Bomcode,
                BomVersion = bomVersionActived.Bomversion,
                ScheduleDate = parsedScheduleDate,
                Deadline = parsedDeadline,
                Responsible = manufacturingOrder.Responsible,
                Quantity = manufacturingOrder.Quantity,
                QuantityProduced = 0,
                Lotno = $"MO-{manufacturingOrder.ManufacturingOrderCode}-{manufacturingOrder.ProductCode}",
                StatusId = 1,
                WarehouseCode = manufacturingOrder.WarehouseCode
            };

            // 9. Thêm vào database sử dụng transaction
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Thêm lệnh sản xuất
                    await _manufacturingOrderRepository.AddAsync(manufacturing, saveChanges: false);
                    // Thêm chi tiết lệnh sản xuất
                    await _context.ManufacturingOrderDetails.AddRangeAsync(manufacturingDetails);
                    // Lưu thay đổi và commit transaction
                    await _context.SaveChangesAsync();
                    // Tạo Reservation
                    var reservationCode = $"RES_{manufacturing.ManufacturingOrderCode}";
                    var reservationRequest = new ReservationRequestDTO
                    {
                        ReservationCode = reservationCode,
                        OrderTypeCode = manufacturing.OrderTypeCode,
                        OrderId = manufacturing.ManufacturingOrderCode,
                        ReservationDate = DateTime.Now.ToString("dd/MM/yyyy"),
                        StatusId = 1,
                        WarehouseCode = manufacturing.WarehouseCode
                    };
                    var reservationResponse = await _reservationService.AddOrUpdateReservation(reservationRequest, transaction);
                    if (!reservationResponse.Success)
                    {
                        await transaction.RollbackAsync();
                        return new ServiceResponse<bool>(false, $"Lỗi khi tạo reservation: {reservationResponse.Message}");
                    }

                    // Tạo Picklist
                    var pickListCode = $"PICK_{manufacturing.ManufacturingOrderCode}";
                    var pickListRequest = new PickListRequestDTO
                    {
                        PickNo = pickListCode,
                        ReservationCode = reservationCode,
                        WarehouseCode = manufacturing.WarehouseCode,
                        Responsible = manufacturing.Responsible,
                        PickDate = DateTime.Now.ToString("dd/MM/yyyy"),
                        StatusId = 1
                    };
                    var pickListResponse = await _pickListService.AddPickList(pickListRequest, transaction);
                    if (!pickListResponse.Success)
                    {
                        await transaction.RollbackAsync();
                        return new ServiceResponse<bool>(false, $"Lỗi khi tạo picklist: {pickListResponse.Message}");
                    }

                    // Lưu thay đổi và commit transaction
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new ServiceResponse<bool>(true, "Thêm lệnh sản xuất và chi tiết thành công");
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
                            return new ServiceResponse<bool>(false, "Dữ liệu tham chiếu không đúng (ví dụ: mã sản phẩm, kho, hoặc thành phần không tồn tại)");
                        }
                    }
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi thêm lệnh sản xuất: {ex.Message}");
                }
            }
        }


        public async Task<ServiceResponse<PagedResponse<ManufacturingOrderResponseDTO>>> GetAllManufacturingOrdersAsync(string[] warehouseCodes, int page, int pageSize)
        {
            if (warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<ManufacturingOrderResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            try
            {
                var query = _manufacturingOrderRepository.GetAllManufacturingOrder(warehouseCodes);
                var result = await query
                    .Select(x => new ManufacturingOrderResponseDTO
                    {
                        ManufacturingOrderCode = x.ManufacturingOrderCode,
                        ProductCode = x.ProductCode!,
                        ProductName = x.ProductCodeNavigation!.ProductName!,
                        WarehouseCode = x.WarehouseCode,
                        WarehouseName = x.WarehouseCodeNavigation!.WarehouseName!,
                        Bomcode = x.Bomcode!,
                        BomVersion = x.BomVersion!,
                        OrderTypeCode = x.OrderTypeCode,
                        OrderTypeName = x.OrderTypeCodeNavigation!.OrderTypeName,
                        Responsible = x.Responsible,
                        FullNameResponsible = x.ResponsibleNavigation!.FullName,
                        StatusId = x.StatusId,
                        StatusName = x.Status!.StatusName!,
                        Lotno = x.Lotno,
                        ScheduleDate = x.ScheduleDate.HasValue ? x.ScheduleDate.Value.ToString("dd/MM/yyyy") : null,
                        Deadline = x.Deadline.HasValue ? x.Deadline.Value.ToString("dd/MM/yyyy") : null,
                        Quantity = x.Quantity,
                        QuantityProduced = x.QuantityProduced
                    })
                    .OrderBy(x => x.StatusId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var totalItems = await query.CountAsync();
                var pagedResponse = new PagedResponse<ManufacturingOrderResponseDTO>(result, page, pageSize, totalItems);

                return new ServiceResponse<PagedResponse<ManufacturingOrderResponseDTO>>(true, "Lấy danh sách lệnh sản xuất thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<ManufacturingOrderResponseDTO>>(false, $"Lỗi khi lấy danh sách lệnh sản xuất: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<ManufacturingOrderResponseDTO>>> GetAllManufacturingOrdersWithStatusAsync(string[] warehouseCodes, int statusId, int page, int pageSize)
        {
            if (warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<ManufacturingOrderResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            try
            {
                var query = _manufacturingOrderRepository.GetAllManufacturingOrderWithStatus(warehouseCodes, statusId);
                var result = await query
                    .Select(x => new ManufacturingOrderResponseDTO
                    {
                        ManufacturingOrderCode = x.ManufacturingOrderCode,
                        ProductCode = x.ProductCode!,
                        ProductName = x.ProductCodeNavigation!.ProductName!,
                        WarehouseCode = x.WarehouseCode,
                        WarehouseName = x.WarehouseCodeNavigation!.WarehouseName!,
                        Bomcode = x.Bomcode!,
                        BomVersion = x.BomVersion!,
                        OrderTypeCode = x.OrderTypeCode,
                        OrderTypeName = x.OrderTypeCodeNavigation!.OrderTypeName,
                        Responsible = x.Responsible,
                        FullNameResponsible = x.ResponsibleNavigation!.FullName,
                        StatusId = x.StatusId,
                        StatusName = x.Status!.StatusName!,
                        Lotno = x.Lotno,
                        ScheduleDate = x.ScheduleDate.HasValue ? x.ScheduleDate.Value.ToString("dd/MM/yyyy") : null,
                        Deadline = x.Deadline.HasValue ? x.Deadline.Value.ToString("dd/MM/yyyy") : null,
                        Quantity = x.Quantity,
                        QuantityProduced = x.QuantityProduced
                    })
                    .OrderBy(x => x.ManufacturingOrderCode)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var totalItems = await query.CountAsync();
                var pagedResponse = new PagedResponse<ManufacturingOrderResponseDTO>(result, page, pageSize, totalItems);

                return new ServiceResponse<PagedResponse<ManufacturingOrderResponseDTO>>(true, "Lấy danh sách lệnh sản xuất theo trạng thái thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<ManufacturingOrderResponseDTO>>(false, $"Lỗi khi lấy danh sách lệnh sản xuất: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<ManufacturingOrderResponseDTO>> GetManufacturingOrderByCodeAsync(string manufacturingCode)
        {
            if (string.IsNullOrEmpty(manufacturingCode))
            {
                return new ServiceResponse<ManufacturingOrderResponseDTO>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            try
            {
                var manufacturingOrder = await _manufacturingOrderRepository.GetManufacturingWithCode(manufacturingCode);
                if (manufacturingOrder == null)
                {
                    return new ServiceResponse<ManufacturingOrderResponseDTO>(false, "Không tìm thấy lệnh sản xuất");
                }

                var response = new ManufacturingOrderResponseDTO
                {
                    ManufacturingOrderCode = manufacturingOrder.ManufacturingOrderCode,
                    ProductCode = manufacturingOrder.ProductCode!,
                    ProductName = manufacturingOrder.ProductCodeNavigation!.ProductName!,
                    WarehouseCode = manufacturingOrder.WarehouseCode,
                    WarehouseName = manufacturingOrder.WarehouseCodeNavigation!.WarehouseName!,
                    Bomcode = manufacturingOrder.Bomcode!,
                    BomVersion = manufacturingOrder.BomVersion!,
                    OrderTypeCode = manufacturingOrder.OrderTypeCode,
                    OrderTypeName = manufacturingOrder.OrderTypeCodeNavigation!.OrderTypeName,
                    Responsible = manufacturingOrder.Responsible,
                    FullNameResponsible = manufacturingOrder.ResponsibleNavigation!.FullName,
                    StatusId = manufacturingOrder.StatusId,
                    StatusName = manufacturingOrder.Status!.StatusName!,
                    Lotno = manufacturingOrder.Lotno,
                    ScheduleDate = manufacturingOrder.ScheduleDate.HasValue ? manufacturingOrder.ScheduleDate.Value.ToString("dd/MM/yyyy") : null,
                    Deadline = manufacturingOrder.Deadline.HasValue ? manufacturingOrder.Deadline.Value.ToString("dd/MM/yyyy") : null,
                    Quantity = manufacturingOrder.Quantity,
                    QuantityProduced = manufacturingOrder.QuantityProduced
                };

                return new ServiceResponse<ManufacturingOrderResponseDTO>(true, "Lấy lệnh sản xuất thành công", response);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ManufacturingOrderResponseDTO>(false, $"Lỗi khi lấy lệnh sản xuất: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<ManufacturingOrderResponseDTO>>> SearchManufacturingOrdersAsync(string[] warehouseCodes, string textToSearch, int page, int pageSize)
        {
            if (warehouseCodes.Length == 0 || page < 1 || pageSize < 1 || string.IsNullOrEmpty(textToSearch))
            {
                return new ServiceResponse<PagedResponse<ManufacturingOrderResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            try
            {
                var query = _manufacturingOrderRepository.SearchManufacturingAsync(warehouseCodes, textToSearch);
                var result = await query
                    .Select(x => new ManufacturingOrderResponseDTO
                    {
                        ManufacturingOrderCode = x.ManufacturingOrderCode,
                        ProductCode = x.ProductCode!,
                        ProductName = x.ProductCodeNavigation!.ProductName!,
                        WarehouseCode = x.WarehouseCode,
                        WarehouseName = x.WarehouseCodeNavigation!.WarehouseName!,
                        Bomcode = x.Bomcode!,
                        BomVersion = x.BomVersion!,
                        OrderTypeCode = x.OrderTypeCode,
                        OrderTypeName = x.OrderTypeCodeNavigation!.OrderTypeName,
                        Responsible = x.Responsible,
                        FullNameResponsible = x.ResponsibleNavigation!.FullName,
                        StatusId = x.StatusId,
                        StatusName = x.Status!.StatusName!,
                        Lotno = x.Lotno,
                        ScheduleDate = x.ScheduleDate.HasValue ? x.ScheduleDate.Value.ToString("dd/MM/yyyy") : null,
                        Deadline = x.Deadline.HasValue ? x.Deadline.Value.ToString("dd/MM/yyyy") : null,
                        Quantity = x.Quantity,
                        QuantityProduced = x.QuantityProduced
                    })
                    .OrderBy(x => x.StatusId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var totalItems = await query.CountAsync();
                var pagedResponse = new PagedResponse<ManufacturingOrderResponseDTO>(result, page, pageSize, totalItems);

                return new ServiceResponse<PagedResponse<ManufacturingOrderResponseDTO>>(true, "Tìm kiếm lệnh sản xuất thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<ManufacturingOrderResponseDTO>>(false, $"Lỗi khi tìm kiếm lệnh sản xuất: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> UpdateManufacturingOrderAsync(ManufacturingOrderRequestDTO manufacturingOrder)
        {
            if (manufacturingOrder == null || string.IsNullOrEmpty(manufacturingOrder.ManufacturingOrderCode))
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            var existingOrder = await _manufacturingOrderRepository.GetManufacturingWithCode(
                manufacturingOrder.ManufacturingOrderCode);

            if (existingOrder == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy lệnh sản xuất");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    existingOrder.QuantityProduced = manufacturingOrder.QuantityProduced;
                    if (existingOrder.QuantityProduced > 0 && existingOrder.QuantityProduced <= existingOrder.Quantity)
                    {
                        existingOrder.StatusId = 2;
                    }
                    var manufacturingDetails = await _context.ManufacturingOrderDetails.Where(x => x.ManufacturingOrderCode == existingOrder.ManufacturingOrderCode).ToListAsync();
                    foreach (var detail in manufacturingDetails)
                    {
                        var percent = (float)manufacturingOrder.QuantityProduced! / manufacturingOrder.Quantity;

                        detail.ConsumedQuantity = Math.Round((double)(percent * detail.ToConsumeQuantity)!, 3);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Cập nhật lệnh sản xuất thành công");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật lệnh sản xuất: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteManufacturingOrderAsync(string manufacturingCode)
        {
            if (string.IsNullOrEmpty(manufacturingCode))
            {
                return new ServiceResponse<bool>(false, "Mã lệnh sản xuất không được để trống");
            }

            var manufacturingOrder = await _manufacturingOrderRepository.GetManufacturingWithCode(manufacturingCode);
            if (manufacturingOrder == null)
            {
                return new ServiceResponse<bool>(false, "Lệnh sản xuất không tồn tại");
            }

            if (manufacturingOrder.StatusId >= 2)
            {
                return new ServiceResponse<bool>(false, "Lệnh sản xuất đang thực hiện không thể xóa");
            }

            var manufacturingOrderDetail = await _manufacturingOrderDetailRepository.GetManufacturingOrderDetail(manufacturingCode).ToListAsync();
            if (manufacturingOrderDetail == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy chi tiết lệnh sản xuất");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (var item in manufacturingOrderDetail)
                    {
                        Expression<Func<ManufacturingOrderDetail, bool>> expression = x => x.ManufacturingOrderCode == item.ManufacturingOrderCode && x.ComponentCode == item.ComponentCode;
                        await _manufacturingOrderDetailRepository.DeleteFirstByConditionAsync(expression, saveChanges: false);
                    }

                    await _manufacturingOrderRepository.DeleteAsync(manufacturingCode, saveChanges: false);
                    await _pickListService.DeletePickList($"PICK_{manufacturingCode}",transaction);
                    await _reservationService.DeleteReservationAsync($"RES_{manufacturingCode}",transaction);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa lệnh sản xuất thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("foreign key"))
                        {
                            return new ServiceResponse<bool>(false, "Không thể xóa lệnh sản xuất vì có dữ liệu tham chiếu.");
                        }
                    }
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xóa lệnh sản xuất: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<List<OrderTypeResponseDTO>>> GetListOrderTypeAsync(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                return new ServiceResponse<List<OrderTypeResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            try
            {
                var lstOrderType = await _orderTypeRepository.GetOrderTypeByCode(prefix);
                var lstOrderTypeResponse = lstOrderType.Select(x => new OrderTypeResponseDTO
                {
                    OrderTypeCode = x.OrderTypeCode,
                    OrderTypeName = x.OrderTypeName
                }).ToList();

                return new ServiceResponse<List<OrderTypeResponseDTO>>(true, "Lấy danh sách loại lệnh sản xuất thành công", lstOrderTypeResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<OrderTypeResponseDTO>>(false, $"Lỗi khi lấy danh sách loại lệnh: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<List<AccountManagementResponseDTO>>> GetListResponsibleAsync(string warehouseCode)
        {
            try
            {
                var lstResponsible = await _accountRepository.GetAllAccount(1, int.MaxValue);
                var lstResponsibleResponse = lstResponsible
                    .Where(x => !x.GroupId!.StartsWith("ADMIN") && !x.GroupId.StartsWith("QLKHO") && !x.GroupId.StartsWith("QLSX") && x.Group!.GroupFunctions.Select(x => x.ApplicableLocation).FirstOrDefault() == warehouseCode)
                    .Select(x => new AccountManagementResponseDTO
                    {
                        UserName = x.UserName,
                        FullName = x.FullName!,
                        GroupID = x.GroupId!,
                        GroupName = x.Group!.GroupId,
                        Password = x.Password!
                    }).ToList();

                return new ServiceResponse<List<AccountManagementResponseDTO>>(true, "Lấy danh sách nhân viên chịu trách nhiệm thành công", lstResponsibleResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<AccountManagementResponseDTO>>(false, $"Lỗi khi lấy danh sách nhân viên: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<List<StatusMasterResponseDTO>>> GetListStatusMasterAsync()
        {
            try
            {
                var lstStatus = await _statusMasterRepository.GetAllStatuses();
                var lstStatusResponse = lstStatus.Select(x => new StatusMasterResponseDTO
                {
                    StatusId = x.StatusId,
                    StatusName = x.StatusName
                }).ToList();

                return new ServiceResponse<List<StatusMasterResponseDTO>>(true, "Lấy danh sách trạng thái thành công", lstStatusResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StatusMasterResponseDTO>>(false, $"Lỗi khi lấy danh sách trạng thái: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<List<WarehouseMasterResponseDTO>>> GetListWarehousePermissionAsync(string[] warehouseCodes)
        {
            if (warehouseCodes.Length == 0)
            {
                return new ServiceResponse<List<WarehouseMasterResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            try
            {
                var lstWarehouse = await _warehouseMasterRepository.GetWarehouseMasters(1, int.MaxValue);
                var lstWarehouseResponse = lstWarehouse
                    .Where(x => warehouseCodes.Contains(x.WarehouseCode))
                    .Select(x => new WarehouseMasterResponseDTO
                    {
                        WarehouseCode = x.WarehouseCode,
                        WarehouseName = x.WarehouseName,
                        WarehouseAddress = x.WarehouseAddress,
                        WarehouseDescription = x.WarehouseDescription
                    }).ToList();

                return new ServiceResponse<List<WarehouseMasterResponseDTO>>(true, "Lấy danh sách kho dựa theo quyền thành công", lstWarehouseResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<WarehouseMasterResponseDTO>>(false, $"Lỗi khi lấy danh sách kho: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<BOMMasterResponseDTO>> GetListBomMasterAsync(string productCode)
        {
            try
            {
                var bom = await _bomMasterRepository.GetBOMMasterByProductCode(productCode);
                if (bom == null)
                {
                    return new ServiceResponse<BOMMasterResponseDTO>(false, "Không tìm thấy dữ liệu BOM Master");
                }
                var lstVersion = await _bomMasterRepository.GetListVersionByBomCode(bom.Bomcode);
                var response = new BOMMasterResponseDTO
                {
                    BOMCode = bom.Bomcode,
                    ProductCode = bom.ProductCode,
                    ProductName = bom.ProductCodeNavigation!.ProductName!,
                    BOMVersionResponses = lstVersion.Select(v => new BOMVersionResponseDTO
                    {
                        BOMVersion = v.Bomversion,
                        IsActive = v.IsActive
                    }).ToList()


                };
                return new ServiceResponse<BOMMasterResponseDTO>(true, "Lấy dữ liệu bom thành công", response);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<BOMMasterResponseDTO>(false, $"Không lấy được dữ liệu {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> ConfirmManufacturingOrder(string manufacturingCode)
        {
            if (string.IsNullOrEmpty(manufacturingCode))
            {
                return new ServiceResponse<bool>(false, "Mã lệnh sản xuất không được để trống");
            }

            var manufacturing = await _manufacturingOrderRepository.GetManufacturingWithCode(manufacturingCode);
            if (manufacturing == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy lệnh sản xuất");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Validate quantity
                    if (manufacturing.QuantityProduced <= 0)
                    {
                        return new ServiceResponse<bool>(false, "Số lượng sản xuất không thể nhỏ hơn hoặc bằng 0");
                    }

                    // Preload PutAwayRules
                    var putAwayRulesPaged = await _putAwayRulesRepository.GetAllPutAwayRules(1, int.MaxValue);
                    var putAwayRulesList = putAwayRulesPaged
                        .Where(x => x.ProductCode == manufacturing.ProductCode && x.LocationCodeNavigation!.WarehouseCode == manufacturing.WarehouseCode)
                        .ToList();

                    // Load ProductMasters, LocationMasters, and StorageProducts
                    var productMasters = (await _productMasterRepository.GetAllProduct(1, int.MaxValue))
                        .GroupBy(p => p.ProductCode!)
                        .ToDictionary(g => g.Key, g => g.First());
                    var locationMasters = await _context.LocationMasters
                        .Where(l => l.WarehouseCode == manufacturing.WarehouseCode)
                        .ToDictionaryAsync(l => l.LocationCode!, l => l);
                    var storageProducts = await _context.StorageProducts
                        .ToDictionaryAsync(sp => sp.StorageProductId!, sp => sp);

                    // Tính tổng số lượng tồn kho hiện tại tại các vị trí
                    var inventoryQuantities = await _inventoryRepository.GetInventoryByProductCodeAsync(manufacturing.ProductCode!, manufacturing.WarehouseCode!)
                        .GroupBy(i => i.LocationCode)
                        .Select(g => new { LocationCode = g.Key, TotalQuantity = g.Sum(i => i.Quantity ?? 0) / g.First().ProductCodeNavigation.BaseQuantity })
                        .ToListAsync();

                    string? selectedLocationCode = null;
                    double quantityToPut = (double)manufacturing.QuantityProduced!;

                    // Tìm vị trí phù hợp từ PutAwayRules
                    if (putAwayRulesList.Any())
                    {
                        var sortedRules = putAwayRulesList
                            .Where(r => locationMasters.ContainsKey(r.LocationCode!) && storageProducts.ContainsKey(locationMasters[r.LocationCode!].StorageProductId!))
                            .OrderBy(r => inventoryQuantities.FirstOrDefault(q => q.LocationCode == r.LocationCode)?.TotalQuantity ?? 0);

                        foreach (var rule in sortedRules)
                        {
                            var locationCode = rule.LocationCode!;
                            var storageProductId = locationMasters[locationCode].StorageProductId!;
                            var storageProduct = storageProducts[storageProductId];
                            var currentQuantity = inventoryQuantities.FirstOrDefault(q => q.LocationCode == locationCode)?.TotalQuantity ?? 0;
                            var maxQuantity = storageProduct.MaxQuantity ?? int.MaxValue;
                            var availableSpace = maxQuantity - currentQuantity;

                            if (availableSpace >= quantityToPut)
                            {
                                selectedLocationCode = locationCode;
                                break;
                            }
                        }
                    }

                    // Nếu không tìm được vị trí thực, thử vị trí ảo
                    if (selectedLocationCode == null)
                    {
                        var locationCode = $"{manufacturing.WarehouseCode}/VIRTUAL_LOC/{manufacturing.ProductCode}";
                        var storageProductId = $"SP_{manufacturing.ProductCode}";

                        if (!storageProducts.TryGetValue(storageProductId, out var storageProduct))
                        {
                            var baseQuantity = productMasters.ContainsKey(manufacturing.ProductCode!) ? (productMasters[manufacturing.ProductCode!].BaseQuantity ?? 1) : 1;
                            var maxQuantityInBaseUOM = manufacturing.QuantityProduced * baseQuantity;

                            storageProduct = new StorageProduct
                            {
                                StorageProductId = storageProductId,
                                StorageProductName = $"Định mức ảo cho {manufacturing.ProductCode}",
                                ProductCode = manufacturing.ProductCode,
                                MaxQuantity = maxQuantityInBaseUOM
                            };
                            _context.StorageProducts.Add(storageProduct);
                            storageProducts.Add(storageProductId, storageProduct);
                        }
                        else
                        {
                            var baseQuantity = productMasters.ContainsKey(manufacturing.ProductCode!) ? (productMasters[manufacturing.ProductCode!].BaseQuantity ?? 1) : 1;
                            storageProduct.MaxQuantity += manufacturing.QuantityProduced * baseQuantity;
                            _context.StorageProducts.Update(storageProduct);
                        }

                        // Tạo vị trí ảo nếu chưa tồn tại
                        if (!locationMasters.ContainsKey(locationCode))
                        {
                            var newLocation = new LocationMaster
                            {
                                LocationCode = locationCode,
                                LocationName = $"Vùng ảo cho {manufacturing.ProductCode}",
                                WarehouseCode = manufacturing.WarehouseCode,
                                StorageProductId = storageProductId
                            };
                            _context.LocationMasters.Add(newLocation);
                            locationMasters.Add(locationCode, newLocation);
                            await _context.SaveChangesAsync();
                        }

                        // Kiểm tra định mức của vị trí ảo
                        var currentLocationQuantity = inventoryQuantities.FirstOrDefault(q => q.LocationCode == locationCode)?.TotalQuantity ?? 0;
                        var availableSpace = storageProduct.MaxQuantity - currentLocationQuantity;

                        if (availableSpace >= quantityToPut)
                        {
                            selectedLocationCode = locationCode;
                        }
                    }

                    // Nếu không tìm được vị trí nào đủ định mức, trả về lỗi
                    if (selectedLocationCode == null)
                    {
                        await transaction.RollbackAsync();
                        return new ServiceResponse<bool>(false, $"Không tìm được vị trí có đủ định mức để cất {quantityToPut} đơn vị sản phẩm {manufacturing.ProductCode}");
                    }

                    // Create PutAway
                    var putAwayCode = $"PUT_{manufacturingCode}_{manufacturing.ProductCode}";
                    var putAwayRequest = new PutAwayRequestDTO
                    {
                        PutAwayCode = putAwayCode,
                        OrderTypeCode = manufacturing.OrderTypeCode,
                        LocationCode = selectedLocationCode,
                        Responsible = manufacturing.Responsible,
                        StatusId = 1,
                        PutAwayDate = DateTime.Now.ToString("dd/MM/yyyy"),
                        PutAwayDescription = $"Cất hàng cho lệnh sản xuất {manufacturingCode}",
                    };

                    var putAwayResponse = await _putAwayService.AddPutAway(putAwayRequest, transaction);
                    if (!putAwayResponse.Success)
                    {
                        await transaction.RollbackAsync();
                        return new ServiceResponse<bool>(false, $"Lỗi khi tạo putaway: {putAwayResponse.Message}");
                    }
                    var product = _context.ProductMasters.FirstOrDefault(x => x.ProductCode == manufacturing.ProductCode);
                    // Create or update PutAwayDetail
                    var putAwayDetail = new PutAwayDetail
                    {
                        PutAwayCode = putAwayCode,
                        ProductCode = manufacturing.ProductCode!,
                        LotNo = manufacturing.Lotno!, // Using manufacturing.LotNo as per original logic
                        Demand = manufacturing.QuantityProduced / product!.BaseQuantity,
                        Quantity = 0,
                    };

                    var existingPutAwayDetail = await _context.PutAwayDetails
                        .FirstOrDefaultAsync(x => x.PutAwayCode == putAwayCode && x.ProductCode == manufacturing.ProductCode);
                    if (existingPutAwayDetail == null)
                    {
                        await _context.PutAwayDetails.AddAsync(putAwayDetail);
                    }
                    else
                    {
                        existingPutAwayDetail.Demand = manufacturing.QuantityProduced;
                        existingPutAwayDetail.LotNo = manufacturing.Lotno!;
                        _context.PutAwayDetails.Update(existingPutAwayDetail);
                    }

                    // Update manufacturing order status to "Completed" (assuming statusId 3 is Completed)
                    manufacturing.StatusId = 3;
                    manufacturing.Quantity = manufacturing.QuantityProduced;

                    var manufacturingDetail = await _context.ManufacturingOrderDetails
                        .Where(x => x.ManufacturingOrderCode == manufacturingCode)
                        .ToListAsync();
                    foreach (var detail in manufacturingDetail)
                    {
                        detail.ToConsumeQuantity = detail.ConsumedQuantity;
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new ServiceResponse<bool>(true, "Xác nhận lệnh sản xuất và tạo putaway thành công");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xác nhận lệnh sản xuất: {ex.Message}");
                }
            }
        }


        public async Task<ServiceResponse<bool>> CreateBackOrder(string manufacturingCode, string ScheduleDateBackOrder, string DeadLineBackOrder)
        {
            if (string.IsNullOrEmpty(manufacturingCode))
            {
                return new ServiceResponse<bool>(false, "Mã lệnh sản xuất không được để trống");
            }

            var manufacturing = await _manufacturingOrderRepository.GetManufacturingWithCode(manufacturingCode);
            if (manufacturing == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy lệnh sản xuất");
            }

            if (manufacturing.QuantityProduced >= manufacturing.Quantity)
            {
                return new ServiceResponse<bool>(false, "Số lượng sản xuất đã đủ hoặc vượt yêu cầu, không cần tạo back order");
            }
            string[] formats = {
                "M/d/yyyy h:mm:ss tt",
                "MM/dd/yyyy hh:mm:ss tt",
                "dd/MM/yyyy","dd/MM/yyyy hh:mm:ss tt"
            };
            if (!DateTime.TryParseExact(ScheduleDateBackOrder, formats, new CultureInfo("vi-VN"), DateTimeStyles.None, out DateTime parsedDate))
            {
                return new ServiceResponse<bool>(false, "Ngày bắt đầu sản xuất không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy hoặc M/d/yyyy h:mm:ss tt.");
            }
            if (!DateTime.TryParseExact(DeadLineBackOrder, formats, new CultureInfo("vi-VN"), DateTimeStyles.None, out DateTime parsedDeadLineDate))
            {
                return new ServiceResponse<bool>(false, "Ngày hạn chót sản xuất không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy hoặc M/d/yyyy h:mm:ss tt.");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Calculate remaining quantity for back order
                    var remainingQuantity = manufacturing.Quantity - manufacturing.QuantityProduced;

                    // Create new ManufacturingOrder for back order
                    var backOrderCode = $"BackOrder-{manufacturing.ManufacturingOrderCode}-{manufacturing.ProductCode}";
                    var backOrder = new ManufacturingOrder
                    {
                        ManufacturingOrderCode = backOrderCode,
                        OrderTypeCode = manufacturing.OrderTypeCode,
                        ProductCode = manufacturing.ProductCode,
                        Bomcode = manufacturing.Bomcode,
                        BomVersion = manufacturing.BomVersion,
                        ScheduleDate = parsedDate,
                        Deadline = parsedDeadLineDate,
                        Responsible = manufacturing.Responsible,
                        Quantity = remainingQuantity,
                        QuantityProduced = 0,
                        Lotno = $"MO-{manufacturing.ManufacturingOrderCode}-{manufacturing.ProductCode}",
                        StatusId = 1, // New status
                        WarehouseCode = manufacturing.WarehouseCode,
                    };

                    // Get BOM components for the back order
                    var bomComponents = await _bomComponentRepository.GetAllBOMComponent(manufacturing.Bomcode!, manufacturing.BomVersion!);
                    if (bomComponents == null || !bomComponents.Any())
                    {
                        return new ServiceResponse<bool>(false, $"Không tìm thấy chi tiết BOM cho mã {manufacturing.Bomcode} và phiên bản {manufacturing.BomVersion}.");
                    }

                    // Create ManufacturingOrderDetails for back order
                    var backOrderDetails = bomComponents.Select(bomComponent => new ManufacturingOrderDetail
                    {
                        ManufacturingOrderCode = backOrderCode,
                        ComponentCode = bomComponent.ComponentCode,
                        ToConsumeQuantity = bomComponent.ConsumpQuantity * remainingQuantity,
                        ConsumedQuantity = 0,
                        ScraptRate = bomComponent.ScrapRate ?? 0
                    }).ToList();

                    // Save back order and details
                    await _manufacturingOrderRepository.AddAsync(backOrder, saveChanges: false);
                    await _context.ManufacturingOrderDetails.AddRangeAsync(backOrderDetails);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new ServiceResponse<bool>(true, "Tạo back order thành công");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi tạo back order: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> CheckAndUpdateBackOrderStatus(string manufacturingCode)
        {
            if (string.IsNullOrEmpty(manufacturingCode))
            {
                return new ServiceResponse<bool>(false, "Mã lệnh sản xuất không được để trống");
            }

            var manufacturing = await _manufacturingOrderRepository.GetManufacturingWithCode(manufacturingCode);
            if (manufacturing == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy lệnh sản xuất");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Find related back orders (based on naming convention or a relationship)
                    var backOrders = await _manufacturingOrderRepository
                        .GetAllManufacturingOrder(new[] { manufacturing.WarehouseCode }!)
                        .Where(m => m.ManufacturingOrderCode.StartsWith($"BackOrder-{manufacturingCode}"))
                        .ToListAsync();

                    // Check if all back orders are completed (assuming statusId 3 is Completed)
                    bool allBackOrdersCompleted = backOrders.All(bo => bo.StatusId == 3);

                    if (allBackOrdersCompleted)
                    {
                        // Update original manufacturing order to completed status
                        manufacturing.StatusId = 3; // Completed
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return new ServiceResponse<bool>(true, "Cập nhật trạng thái lệnh sản xuất thành công - tất cả back order đã hoàn thành");
                    }

                    // If not all back orders are completed, check total produced quantity
                    var totalProduced = manufacturing.QuantityProduced + backOrders.Sum(bo => bo.QuantityProduced);
                    if (totalProduced >= manufacturing.Quantity)
                    {
                        manufacturing.StatusId = 3; // Completed
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return new ServiceResponse<bool>(true, "Cập nhật trạng thái lệnh sản xuất thành công - đủ số lượng yêu cầu");
                    }

                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Chưa hoàn thành tất cả back order hoặc chưa đủ số lượng yêu cầu");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi kiểm tra và cập nhật trạng thái back order: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<List<ProductMasterResponseDTO>>> GetListProductMasterIsFGAndSFG()
        {
            try
            {
                var lstProduct = await _productMasterRepository.GetProductMasterWithCategoryID(new string[] { "FG", "SFG" });
                var productResponse = lstProduct.Select(product => new ProductMasterResponseDTO
                {
                    ProductCode = product.ProductCode,
                    ProductName = product.ProductName,
                    ProductDescription = product.ProductDescription,
                    ProductImage = product.ProductImage,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category?.CategoryName ?? "Không có danh mục",
                    BaseQuantity = product.BaseQuantity,
                    Uom = product.Uom,
                    BaseUom = product.BaseUom,
                    Valuation = (float?)product.Valuation,
                    TotalOnHand = (float)(product.Inventories.Where(t => t.ProductCode == product.ProductCode).Sum(i => i.Quantity) ?? 0.00),
                }).ToList();
                return new ServiceResponse<List<ProductMasterResponseDTO>>(true, "Lấy danh sách sản phẩm thành công", productResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<ProductMasterResponseDTO>>(false, $"Lỗi : {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<ManufacturingOrderResponseDTO>>> GetAllManufacturingOrdersAsyncWithResponsible(string[] warehouseCodes, string responsible, int page, int pageSize)
        {
            if (warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<ManufacturingOrderResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            try
            {
                var query = _manufacturingOrderRepository.GetAllManufacturingOrder(warehouseCodes);
                var result = await query
                    .Where(x => x.Responsible == responsible)
                    .Select(x => new ManufacturingOrderResponseDTO
                    {
                        ManufacturingOrderCode = x.ManufacturingOrderCode,
                        ProductCode = x.ProductCode!,
                        ProductName = x.ProductCodeNavigation!.ProductName!,
                        WarehouseCode = x.WarehouseCode,
                        WarehouseName = x.WarehouseCodeNavigation!.WarehouseName!,
                        Bomcode = x.Bomcode!,
                        BomVersion = x.BomVersion!,
                        OrderTypeCode = x.OrderTypeCode,
                        OrderTypeName = x.OrderTypeCodeNavigation!.OrderTypeName,
                        Responsible = x.Responsible,
                        FullNameResponsible = x.ResponsibleNavigation!.FullName,
                        StatusId = x.StatusId,
                        StatusName = x.Status!.StatusName!,
                        Lotno = x.Lotno,
                        ScheduleDate = x.ScheduleDate.HasValue ? x.ScheduleDate.Value.ToString("dd/MM/yyyy") : null,
                        Deadline = x.Deadline.HasValue ? x.Deadline.Value.ToString("dd/MM/yyyy") : null,
                        Quantity = x.Quantity,
                        QuantityProduced = x.QuantityProduced
                    })
                    .OrderBy(x => x.StatusId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var totalItems = await query.Where(x => x.Responsible == responsible).CountAsync();
                var pagedResponse = new PagedResponse<ManufacturingOrderResponseDTO>(result, page, pageSize, totalItems);

                return new ServiceResponse<PagedResponse<ManufacturingOrderResponseDTO>>(true, "Lấy danh sách lệnh sản xuất thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<ManufacturingOrderResponseDTO>>(false, $"Lỗi khi lấy danh sách lệnh sản xuất: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PagedResponse<ManufacturingOrderResponseDTO>>> SearchManufacturingOrdersAsyncWithResponsible(string[] warehouseCodes, string responsible, string textToSearch, int page, int pageSize)
        {
            if (warehouseCodes.Length == 0 || page < 1 || pageSize < 1 || string.IsNullOrEmpty(textToSearch))
            {
                return new ServiceResponse<PagedResponse<ManufacturingOrderResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            try
            {
                var query = _manufacturingOrderRepository.SearchManufacturingAsync(warehouseCodes, textToSearch);
                var result = await query
                    .Where(x => x.Responsible == responsible)
                    .Select(x => new ManufacturingOrderResponseDTO
                    {
                        ManufacturingOrderCode = x.ManufacturingOrderCode,
                        ProductCode = x.ProductCode!,
                        ProductName = x.ProductCodeNavigation!.ProductName!,
                        WarehouseCode = x.WarehouseCode,
                        WarehouseName = x.WarehouseCodeNavigation!.WarehouseName!,
                        Bomcode = x.Bomcode!,
                        BomVersion = x.BomVersion!,
                        OrderTypeCode = x.OrderTypeCode,
                        OrderTypeName = x.OrderTypeCodeNavigation!.OrderTypeName,
                        Responsible = x.Responsible,
                        FullNameResponsible = x.ResponsibleNavigation!.FullName,
                        StatusId = x.StatusId,
                        StatusName = x.Status!.StatusName!,
                        Lotno = x.Lotno,
                        ScheduleDate = x.ScheduleDate.HasValue ? x.ScheduleDate.Value.ToString("dd/MM/yyyy") : null,
                        Deadline = x.Deadline.HasValue ? x.Deadline.Value.ToString("dd/MM/yyyy") : null,
                        Quantity = x.Quantity,
                        QuantityProduced = x.QuantityProduced
                    })
                    .OrderBy(x => x.StatusId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var totalItems = await query.Where(x => x.Responsible == responsible).CountAsync();
                var pagedResponse = new PagedResponse<ManufacturingOrderResponseDTO>(result, page, pageSize, totalItems);

                return new ServiceResponse<PagedResponse<ManufacturingOrderResponseDTO>>(true, "Tìm kiếm lệnh sản xuất thành công", pagedResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResponse<ManufacturingOrderResponseDTO>>(false, $"Lỗi khi tìm kiếm lệnh sản xuất: {ex.Message}");
            }
        }
    }
}