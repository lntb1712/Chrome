using Chrome.DTO;
using Chrome.DTO.AccountManagementDTO;
using Chrome.DTO.BOMComponentDTO;
using Chrome.DTO.BOMMasterDTO;
using Chrome.DTO.ManufacturingOrderDTO;
using Chrome.DTO.OrderTypeDTO;
using Chrome.DTO.ProductMasterDTO;
using Chrome.DTO.PutAwayDTO;
using Chrome.DTO.StatusMasterDTO;
using Chrome.DTO.WarehouseMasterDTO;
using Chrome.Models;
using Chrome.Repositories.AccountRepository;
using Chrome.Repositories.BOMComponentRepository;
using Chrome.Repositories.BOMMasterRepository;
using Chrome.Repositories.InventoryRepository;
using Chrome.Repositories.ManufacturingOrderRepository;
using Chrome.Repositories.OrderTypeRepository;
using Chrome.Repositories.ProductMasterRepository;
using Chrome.Repositories.PutAwayRulesRepository;
using Chrome.Repositories.StatusMasterRepository;
using Chrome.Repositories.WarehouseMasterRepository;
using Chrome.Services.ManufacturingOrderService;
using Chrome.Services.PutAwayService;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Chrome.Services.ManufacturingOrderService
{
    public class ManufacturingOrderService : IManufacturingOrderService
    {
        private readonly IManufacturingOrderRepository _manufacturingOrderRepository;
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
        private readonly ChromeContext _context;

        public ManufacturingOrderService(
            IManufacturingOrderRepository manufacturingOrderRepository,
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
            ChromeContext context)
        {
            _manufacturingOrderRepository = manufacturingOrderRepository;
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
            _context = context;
        }

        public async Task<ServiceResponse<bool>> AddManufacturingOrderAsync(ManufacturingOrderRequestDTO manufacturingOrder)
        {
            // Kiểm tra dữ liệu đầu vào
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

            // Kiểm tra định dạng ngày tháng
            string[] formats = {
                "M/d/yyyy h:mm:ss tt",
                "MM/dd/yyyy hh:mm:ss tt",
                "dd/MM/yyyy"
            };

            if (!DateTime.TryParseExact(manufacturingOrder.ScheduleDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedScheduleDate))
            {
                return new ServiceResponse<bool>(false, "Ngày bắt đầu sản xuất không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy hoặc M/d/yyyy h:mm:ss tt.");
            }

            if (!DateTime.TryParseExact(manufacturingOrder.Deadline, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDeadline))
            {
                return new ServiceResponse<bool>(false, "Ngày hết hạn sản xuất không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy hoặc M/d/yyyy h:mm:ss tt.");
            }

            // Kiểm tra ngày hợp lệ
            if (parsedScheduleDate > parsedDeadline)
            {
                return new ServiceResponse<bool>(false, "Ngày bắt đầu sản xuất không thể lớn hơn ngày hết hạn.");
            }

            // Lấy phiên bản BOM hoạt động
            var lstBomVersion = await _bomMasterRepository.GetListVersionByBomCode(manufacturingOrder.Bomcode);
            var bomVersionActived = lstBomVersion.FirstOrDefault(x => x.IsActive == true);
            if (bomVersionActived == null)
            {
                return new ServiceResponse<bool>(false, $"Không tìm thấy phiên bản BOM hoạt động cho mã BOM {manufacturingOrder.Bomcode}.");
            }

            // Lấy danh sách chi tiết BOM (bao gồm cả BOM con)
            var bomComponents = await _bomComponentRepository.GetAllBOMComponent(manufacturingOrder.Bomcode, bomVersionActived.Bomversion);
            if (bomComponents == null || !bomComponents.Any())
            {
                return new ServiceResponse<bool>(false, $"Không tìm thấy chi tiết BOM cho mã {manufacturingOrder.Bomcode} và phiên bản {bomVersionActived.Bomversion}.");
            }

            // Tạo entity ManufacturingOrder
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

            // Tạo danh sách ManufacturingOrderDetail
            var manufacturingDetails = new List<ManufacturingOrderDetail>();
            foreach (var bomComponent in bomComponents)
            {
                // Lấy ScrapRate từ BomComponent (nếu cần)
                var componentDetail = await _bomComponentRepository.GetBomComponent(
                    bomComponent.Bomcode, bomComponent.ComponentCode, bomComponent.BomVersion);

                manufacturingDetails.Add(new ManufacturingOrderDetail
                {
                    ManufacturingOrderCode = manufacturingOrder.ManufacturingOrderCode,
                    ComponentCode = bomComponent.ComponentCode,
                    ToConsumeQuantity = bomComponent.ConsumpQuantity * manufacturingOrder.Quantity, // Nhân với số lượng lệnh sản xuất
                    ConsumedQuantity = 0, // Ban đầu chưa tiêu thụ
                    ScraptRate = componentDetail?.ScrapRate ?? 0 // Lấy ScrapRate từ BomComponent hoặc mặc định là 0

                });
                
            }

            // Sử dụng transaction
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Thêm ManufacturingOrder
                    await _manufacturingOrderRepository.AddAsync(manufacturing, saveChanges: false);
                    await _context.ManufacturingOrderDetails.AddRangeAsync(manufacturingDetails);
                    

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
                    .OrderBy(x => x.ManufacturingOrderCode)
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
                    .OrderBy(x => x.ManufacturingOrderCode)
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
                    if(existingOrder.QuantityProduced>0 && existingOrder.QuantityProduced<existingOrder.Quantity)
                    {
                        existingOrder.StatusId = 2;
                    }
                    var manufacturingDetails = await _context.ManufacturingOrderDetails.Where(x => x.ManufacturingOrderCode == existingOrder.ManufacturingOrderCode).ToListAsync();
                    foreach(var detail in manufacturingDetails )
                    {
                        var percent = (float)manufacturingOrder.QuantityProduced! / manufacturingOrder.Quantity;

                        detail.ConsumedQuantity=Math.Round((double)( percent*detail.ToConsumeQuantity)!,3);
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

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _manufacturingOrderRepository.DeleteAsync(manufacturingOrder, saveChanges: false);
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

        public async Task<ServiceResponse<List<AccountManagementResponseDTO>>> GetListResponsibleAsync()
        {
            try
            {
                var lstResponsible = await _accountRepository.GetAllAccount(1, int.MaxValue);
                var lstResponsibleResponse = lstResponsible
                    .Where(x => !x.GroupId!.StartsWith("ADMIN") && !x.GroupId.StartsWith("QLKHO"))
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
                    // Preload PutAwayRules
                    var putAwayRulesPaged = await _putAwayRulesRepository.GetAllPutAwayRules(1, int.MaxValue);
                    var putAwayRulesList = putAwayRulesPaged
                        .Where(x => x.ProductCode == manufacturing.ProductCode && x.LocationCodeNavigation!.WarehouseCode == manufacturing.WarehouseCode)
                        .ToList();

                    // Load ProductMasters and LocationMasters
                    var productMasters = (await _productMasterRepository.GetAllProduct(1, int.MaxValue))
                        .GroupBy(p => p.ProductCode!)
                        .ToDictionary(g => g.Key, g => g.First());
                    var locationMasters = await _context.LocationMasters
                        .Where(l => l.WarehouseCode == manufacturing.WarehouseCode)
                        .ToDictionaryAsync(l => l.LocationCode!, l => l);
                    var storageProducts = await _context.StorageProducts
                        .ToDictionaryAsync(sp => sp.StorageProductId!, sp => sp);

                    string locationCode;
                    if (putAwayRulesList.Any())
                    {
                        var inventoryQuantities = await _inventoryRepository.GetInventoryByProductCodeAsync(manufacturing.ProductCode!, manufacturing.WarehouseCode!)
                            .GroupBy(i => i.LocationCode)
                            .Select(g => new { LocationCode = g.Key, TotalQuantity = g.Sum(i => i.Quantity ?? 0) })
                            .ToListAsync();

                        var rule = putAwayRulesList
                            .OrderBy(r => inventoryQuantities.FirstOrDefault(q => q.LocationCode == r.LocationCode)?.TotalQuantity ?? int.MaxValue)
                            .First();
                        locationCode = rule.LocationCode!;
                    }
                    else
                    {
                        locationCode = $"{manufacturing.WarehouseCode}/VIRTUAL_LOC/{manufacturing.ProductCode}";
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
                        }
                        await _context.SaveChangesAsync();
                    }

                    // Create PutAway
                    var putAwayCode = $"PUT_{manufacturingCode}_{manufacturing.ProductCode}";
                    var putAwayRequest = new PutAwayRequestDTO
                    {
                        PutAwayCode = putAwayCode,
                        OrderTypeCode = manufacturing.OrderTypeCode,
                        LocationCode = locationCode,
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
                    var putAwayDetail = new PutAwayDetail
                    {
                        PutAwayCode = putAwayCode,
                        ProductCode = manufacturing.ProductCode!,
                        LotNo = manufacturing.Lotno,
                        Demand = manufacturing.QuantityProduced,
                        Quantity = 0,
                    };
                    await _context.PutAwayDetails.AddAsync(putAwayDetail);

                    // Update manufacturing order status to "Completed" (assuming statusId 3 is Completed)
                    manufacturing.StatusId = 3;
                    manufacturing.Quantity = manufacturing.QuantityProduced;

                    var manufacturingDetail = await _context.ManufacturingOrderDetails.Where(x => x.ManufacturingOrderCode == manufacturingCode).ToListAsync();
                    foreach(var detail in manufacturingDetail)
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

        public async Task<ServiceResponse<bool>> CreateBackOrder(string manufacturingCode)
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
                        ScheduleDate = DateTime.Now,
                        Deadline = manufacturing.Deadline,
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

    }
}