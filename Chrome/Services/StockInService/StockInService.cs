using Azure;
using Chrome.DTO;
using Chrome.DTO.AccountManagementDTO;
using Chrome.DTO.OrderTypeDTO;
using Chrome.DTO.PurchaseOrderDetailDTO;
using Chrome.DTO.PurchaseOrderDTO;
using Chrome.DTO.StatusMasterDTO;
using Chrome.DTO.StockInDetailDTO;
using Chrome.DTO.StockInDTO;
using Chrome.DTO.SupplierMasterDTO;
using Chrome.DTO.WarehouseMasterDTO;
using Chrome.Models;
using Chrome.Repositories.AccountRepository;
using Chrome.Repositories.OrderTypeRepository;
using Chrome.Repositories.PurchaseOrderDetailRepository;
using Chrome.Repositories.PurchaseOrderRepository;
using Chrome.Repositories.StatusMasterRepository;
using Chrome.Repositories.StockInRepository;
using Chrome.Repositories.SupplierMasterRepository;
using Chrome.Repositories.WarehouseMasterRepository;
using Chrome.Services.StockInDetailService;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;

namespace Chrome.Services.StockInService
{
    public class StockInService : IStockInService
    {
        private readonly IStockInRepository _stockInRepository;
        private readonly IOrderTypeRepository _orderTypeRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IStatusMasterRepository _statusMasterRepository;
        private readonly IWarehouseMasterRepository _warehouseMasterRepository;
        private readonly IPurchaseOrderDetailRepository _purchaseOrderDetailRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IStockInDetailService _stockInDetailService;
        private readonly ChromeContext _context;

        public StockInService(IStockInRepository stockInRepository,
            IOrderTypeRepository orderTypeRepository,
            IAccountRepository accountRepository,
            IStatusMasterRepository statusMasterRepository,
            IWarehouseMasterRepository warehouseMasterRepository,
            IPurchaseOrderDetailRepository purchaseOrderDetailRepository,
            IPurchaseOrderRepository purchaseOrderRepository,
            IStockInDetailService stockInDetailService,
            ChromeContext context)
        {
            _stockInRepository = stockInRepository;
            _orderTypeRepository = orderTypeRepository;
            _accountRepository = accountRepository;
            _statusMasterRepository = statusMasterRepository;
            _warehouseMasterRepository = warehouseMasterRepository;
            _purchaseOrderDetailRepository = purchaseOrderDetailRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
            _stockInDetailService = stockInDetailService;
            _context = context;
        }

        public async Task<ServiceResponse<bool>> AddStockIn(StockInRequestDTO stockIn)
        {
            if (stockIn == null) return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            if (string.IsNullOrEmpty(stockIn.StockInCode)) return new ServiceResponse<bool>(false, "Mã nhập kho không được để trống");
            string[] formats = {
                "M/d/yyyy h:mm:ss tt",
                "MM/dd/yyyy hh:mm:ss tt",
                "dd/MM/yyyy"
            };
            if (!DateTime.TryParseExact(stockIn.OrderDeadline, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return new ServiceResponse<bool>(false, "Ngày nhập kho không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy hoặc M/d/yyyy h:mm:ss tt.");
            }
            var stockInRequest = new StockIn
            {
                StockInCode = stockIn.StockInCode,
                OrderTypeCode = stockIn.OrderTypeCode,
                WarehouseCode = stockIn.WarehouseCode,
                PurchaseOrderCode = stockIn.PurchaseOrderCode,
                Responsible = stockIn.Responsible,
                StatusId = 1,
                OrderDeadline = parsedDate,
                StockInDescription = stockIn.StockInDescription,
            };
            var lstPOdetails = new List<PurchaseOrderDetailResponseDTO>();
            if(!string.IsNullOrEmpty(stockInRequest.PurchaseOrderCode))
            {
                var purchaseOrderDetail = _purchaseOrderDetailRepository.GetAllPurchaseOrderDetailsAsync(stockInRequest.PurchaseOrderCode);
                if(purchaseOrderDetail==null)
                {
                    return new ServiceResponse<bool>(false, "Không tìm thấy chi tiết đặt hàng");
                }
                lstPOdetails = await purchaseOrderDetail
                .Select(x => new PurchaseOrderDetailResponseDTO
                {
                    PurchaseOrderCode = x.PurchaseOrderCode,
                    ProductCode = x.ProductCode,
                    ProductName = x.ProductCodeNavigation.ProductName!,
                    Quantity = x.Quantity
                }).ToListAsync();
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. Thêm phiếu nhập kho (StockIn)
                    await _stockInRepository.AddAsync(stockInRequest, saveChanges: false);
                    await _context.SaveChangesAsync();
                    // 2. Thêm chi tiết nhập kho theo từng sản phẩm của PO
                    foreach (var item in lstPOdetails)
                    {
                        var stockInDetail = new StockInDetailRequestDTO
                        {
                            StockInCode = stockIn.StockInCode,
                            ProductCode = item.ProductCode,
                            Demand = item.Quantity,
                            Quantity = 0 // Chưa nhận hàng
                        };

                        await _stockInDetailService.AddStockInDetail(stockInDetail, transaction);
                    }

                    // 3. Cập nhật trạng thái đơn đặt hàng
                    var purchaseOrderHeader = await _purchaseOrderRepository.GetPurchaseOrderWithCode(stockIn.PurchaseOrderCode!);

                    purchaseOrderHeader.StatusId = 2; // Partially Received hoặc chờ xử lý
                    _context.PurchaseOrders.Update(purchaseOrderHeader);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Thêm lệnh nhập kho thành công");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi thêm lệnh nhập kho: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteStockInAsync(string stockInCode)
        {
            if(string.IsNullOrEmpty(stockInCode))
            {
                return new ServiceResponse<bool>(false, "Mã nhập kho không được để trống");
            }    
            var stockIn = await _stockInRepository.GetStockInWithCode(stockInCode);
            if(stockIn == null)
            {
                return new ServiceResponse<bool>(false, "Lệnh nhập kho không tồn tại");
            }    
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _stockInRepository.DeleteAsync(stockInCode, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa lệnh nhập kho thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("foreign key"))
                        {
                            return new ServiceResponse<bool>(false, "Không thể xóa lệnh nhập vì có dữ liệu tham chiếu.");
                        }
                    }
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xóa lệnh nhập kho: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<PagedResponse<StockInResponseDTO>>> GetAllStockIns(string[] warehouseCodes, int page, int pageSize)
        {
            if(warehouseCodes.Length == 0 || page<1 ||pageSize<1)
            {
                return new ServiceResponse<PagedResponse<StockInResponseDTO>>(false, "Dữu liệu nhận vào không hợp lệ");
            }
            var query = _stockInRepository.GetAllStockInAsync(warehouseCodes);
            var result = await query
                         .Select(x=>new StockInResponseDTO
                         {
                             StockInCode = x.StockInCode,
                             OrderTypeCode = x.OrderTypeCode,
                             OrderTypeName = x.OrderTypeCodeNavigation!.OrderTypeName,
                             WarehouseCode = x.WarehouseCode,
                             WarehouseName = x.WarehouseCodeNavigation!.WarehouseName,
                             PurchaseOrderCode = x.PurchaseOrderCode,
                             SupplierCode = x.PurchaseOrderCodeNavigation!.SupplierCode,
                             SupplierName = x.PurchaseOrderCodeNavigation!.SupplierCodeNavigation!.SupplierName,
                             Responsible = x.Responsible,
                             FullNameResponsible = x.ResponsibleNavigation!.FullName,
                             StatusId = x.StatusId,
                             StatusName =x.Status!.StatusName,
                             OrderDeadline = x.OrderDeadline!.Value.ToString("dd/MM/yyyy"),
                             StockInDescription = x.StockInDescription,
                         })
                         .OrderBy(x=>x.StatusId)
                         .Skip((page-1) * pageSize) 
                         .Take(pageSize)
                         .ToListAsync();
            var totalItems = await query.CountAsync();
            var pagedResponse = new PagedResponse<StockInResponseDTO>(result,page,pageSize,totalItems);
            return new ServiceResponse<PagedResponse<StockInResponseDTO>>(true, "Lấy danh sách lệnh nhập kho thành công", pagedResponse);
        }


        public async Task<ServiceResponse<PagedResponse<StockInResponseDTO>>> GetAllStockInsWithStatus(string[] warehouseCodes, int statusId, int page, int pageSize)
        {
            if (warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<StockInResponseDTO>>(false, "Dữu liệu nhận vào không hợp lệ");
            }
            var query = _stockInRepository.GetAllStockInWithStatus(warehouseCodes,statusId);
            var result = await query
                         .Select(x => new StockInResponseDTO
                         {
                             StockInCode = x.StockInCode,
                             OrderTypeCode = x.OrderTypeCode,
                             OrderTypeName = x.OrderTypeCodeNavigation!.OrderTypeName,
                             WarehouseCode = x.WarehouseCode,
                             WarehouseName = x.WarehouseCodeNavigation!.WarehouseName,
                             PurchaseOrderCode = x.PurchaseOrderCode,
                             SupplierCode = x.PurchaseOrderCodeNavigation!.SupplierCode,
                             SupplierName = x.PurchaseOrderCodeNavigation!.SupplierCodeNavigation!.SupplierName,
                             Responsible = x.Responsible,
                             FullNameResponsible = x.ResponsibleNavigation!.FullName,
                             StatusId = x.StatusId,
                             StatusName = x.Status!.StatusName,
                             OrderDeadline = x.OrderDeadline!.Value.ToString("dd/MM/yyyy"),
                             StockInDescription = x.StockInDescription,
                         })
                         .OrderBy(x => x.StockInCode)
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize)
                         .ToListAsync();
            var totalItems = await query.CountAsync();
            var pagedResponse = new PagedResponse<StockInResponseDTO>(result, page, pageSize, totalItems);
            return new ServiceResponse<PagedResponse<StockInResponseDTO>>(true, "Lấy danh sách lệnh nhập kho thành công", pagedResponse);
        }

        public async Task<ServiceResponse<PagedResponse<StockInResponseDTO>>> GetAllStockInWithResponsible(string[] warehouseCodes, string responsible, int page, int pageSize)
        {
            if (warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<StockInResponseDTO>>(false, "Dữu liệu nhận vào không hợp lệ");
            }
            var query = _stockInRepository.GetAllStockInWithResponsible(warehouseCodes,responsible);
            var result = await query
                         .Select(x => new StockInResponseDTO
                         {
                             StockInCode = x.StockInCode,
                             OrderTypeCode = x.OrderTypeCode,
                             OrderTypeName = x.OrderTypeCodeNavigation!.OrderTypeName,
                             WarehouseCode = x.WarehouseCode,
                             WarehouseName = x.WarehouseCodeNavigation!.WarehouseName,
                             PurchaseOrderCode = x.PurchaseOrderCode,
                             SupplierCode = x.PurchaseOrderCodeNavigation!.SupplierCode,
                             SupplierName = x.PurchaseOrderCodeNavigation!.SupplierCodeNavigation!.SupplierName,
                             Responsible = x.Responsible,
                             FullNameResponsible = x.ResponsibleNavigation!.FullName,
                             StatusId = x.StatusId,
                             StatusName = x.Status!.StatusName,
                             OrderDeadline = x.OrderDeadline!.Value.ToString("dd/MM/yyyy"),
                             StockInDescription = x.StockInDescription,
                         })
                         .OrderBy(x => x.StatusId)
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize)
                         .ToListAsync();
            var totalItems = await query.CountAsync();
            var pagedResponse = new PagedResponse<StockInResponseDTO>(result, page, pageSize, totalItems);
            return new ServiceResponse<PagedResponse<StockInResponseDTO>>(true, "Lấy danh sách lệnh nhập kho thành công", pagedResponse);
        }

        public async Task<ServiceResponse<List<OrderTypeResponseDTO>>> GetListOrderType(string prefix)
        {
            if(string.IsNullOrEmpty(prefix))
            {
                return new ServiceResponse<List<OrderTypeResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }    
            var lstOrderTypePrefix = await _orderTypeRepository.GetOrderTypeByCode(prefix);
            var lstOrderTypeList = lstOrderTypePrefix.Select(x => new OrderTypeResponseDTO
            {
                OrderTypeCode = x.OrderTypeCode,
                OrderTypeName = x.OrderTypeName,
            }).ToList();
            return new ServiceResponse<List<OrderTypeResponseDTO>>(true, "Lấy danh sách loại nhập kho thành công", lstOrderTypeList);
        }

        public async Task<ServiceResponse<List<PurchaseOrderResponseDTO>>> GetListPurchaseOrder( string[] warehouseCodes, int[]? statusFilters =null)
        {
            if (warehouseCodes.Length == 0)
            {
                return new ServiceResponse<List<PurchaseOrderResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var query = _purchaseOrderRepository.GetAllPurchaseOrdersAsync(warehouseCodes);
            if (statusFilters != null && statusFilters.Length > 0)
            {
                query = query.Where(x => statusFilters.Contains(x.StatusId!.Value));
            }
            var purchaseOrders = await query
                .Select(x => new PurchaseOrderResponseDTO
                {
                    PurchaseOrderCode = x.PurchaseOrderCode,
                    WarehouseCode = x.WarehouseCode,
                    StatusId = x.StatusId,
                    WarehouseName = x.WarehouseCodeNavigation!.WarehouseName,
                    OrderDate = x.OrderDate!.Value.ToString("dd/MM/yyyy"),
                    ExpectedDate = x.ExpectedDate!.Value.ToString("dd/MM/yyyy"),
                    SupplierCode = x.SupplierCode,
                    PurchaseOrderDescription = x.PurchaseOrderDescription,
                    StatusName = x.Status!.StatusName,
                    SupplierName = x.SupplierCodeNavigation!.SupplierName
                }).ToListAsync();
            return new ServiceResponse<List<PurchaseOrderResponseDTO>>(true, "Lấy danh sách đặt hàng thành công", purchaseOrders);
        }

        public async Task<ServiceResponse<List<AccountManagementResponseDTO>>> GetListResponsibleAsync(string warehouseCode)
        {
            var lstResponsible = await _accountRepository.GetAllAccount(1, int.MaxValue);
            var lstResponsibleForSI = lstResponsible.Where(x => !x.GroupId!.StartsWith("ADMIN") && !x.GroupId.StartsWith("QLKHO") && !x.GroupId.StartsWith("QLSX") && x.Group!.GroupFunctions.Select(x => x.ApplicableLocation).FirstOrDefault() == warehouseCode)
                                                    .Select(x => new AccountManagementResponseDTO
                                                    {
                                                        UserName = x.UserName,
                                                        FullName = x.FullName!,
                                                        GroupID = x.GroupId!,
                                                        GroupName = x.Group!.GroupId,
                                                        Password = x.Password!,
                                                    }).ToList();
            return new ServiceResponse<List<AccountManagementResponseDTO>>(true, "Lấy danh sách nhân viên chịu trách nhiệm thành công", lstResponsibleForSI);
        }

        public async Task<ServiceResponse<List<StatusMasterResponseDTO>>> GetListStatusMaster()
        {
            var lstStatus = await _statusMasterRepository.GetAllStatuses();
            var lstStatusResponse = lstStatus.Select(x => new StatusMasterResponseDTO
            {
                StatusId = x.StatusId,
                StatusName = x.StatusName,
            }).ToList();
            return new ServiceResponse<List<StatusMasterResponseDTO>>(true, "Lấy danh sách trạng thái thành công", lstStatusResponse);
        }

        public async Task<ServiceResponse<List<StockInAndDetailDTO>>> GetListStockInToReport(string[] warehouseCodes, int month, int year)
        {
            if (warehouseCodes.Length == 0 || month < 1 || year < 1)
            {
                return new ServiceResponse<List<StockInAndDetailDTO>>(false, "Dữu liệu nhận vào không hợp lệ");
            }
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var query = _stockInRepository.GetAllStockInAsync(warehouseCodes)
                                          .Where(s => s.OrderDeadline >= startDate && s.OrderDeadline <= endDate)
                                          .OrderBy(s => s.OrderDeadline);
            var result = await query
                         .Select(x => new StockInAndDetailDTO
                         {
                             StockInCode = x.StockInCode,
                             OrderTypeCode = x.OrderTypeCode,
                             OrderTypeName = x.OrderTypeCodeNavigation!.OrderTypeName,
                             WarehouseCode = x.WarehouseCode,
                             WarehouseName = x.WarehouseCodeNavigation!.WarehouseName,
                             PurchaseOrderCode = x.PurchaseOrderCode,
                             SupplierCode = x.PurchaseOrderCodeNavigation!.SupplierCode,
                             SupplierName = x.PurchaseOrderCodeNavigation!.SupplierCodeNavigation!.SupplierName,
                             Responsible = x.Responsible,
                             FullNameResponsible = x.ResponsibleNavigation!.FullName,
                             StatusId = x.StatusId,
                             StatusName = x.Status!.StatusName,
                             OrderDeadline = x.OrderDeadline!.Value.ToString("dd/MM/yyyy"),
                             StockInDescription = x.StockInDescription,
                             stockInDetailDTOs = x.StockInDetails.Select(sd => new StockInDetailReportDTO
                             {
                                 StockInCode = sd.StockInCode,
                                 ProductCode = sd.ProductCode,
                                 ProductName = sd.ProductCodeNavigation.ProductName!,
                                 UOM = sd.ProductCodeNavigation.Uom!,
                                 LotNo = sd.LotNo,
                                 Demand = sd.Demand,
                                 Quantity = sd.Quantity,
                             }).ToList()
                         })
                         .ToListAsync();
           
            return new ServiceResponse<List<StockInAndDetailDTO>>(true, "Lấy danh sách lệnh nhập kho thành công", result);
        }

        public async Task<ServiceResponse<List<WarehouseMasterResponseDTO>>> GetListWarehousePermission(string[] warehouseCodes)
        {
            if(warehouseCodes.Length==0)
            {
                return new ServiceResponse<List<WarehouseMasterResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var response = await _warehouseMasterRepository.GetWarehouseMasters(1, int.MaxValue);
            var lstWarehouseMapping = response.Where(x => warehouseCodes.Contains(x.WarehouseCode))
                                              .Select(x => new WarehouseMasterResponseDTO
                                              {
                                                  WarehouseCode = x.WarehouseCode,
                                                  WarehouseName = x.WarehouseName,
                                                  WarehouseAddress = x.WarehouseAddress,
                                                  WarehouseDescription = x.WarehouseDescription,
                                              })
                                              .ToList();
            return new ServiceResponse<List<WarehouseMasterResponseDTO>>(true, "Lấy danh sách kho dựa theo quyền thành công", lstWarehouseMapping);
        }

        public async Task<ServiceResponse<StockInResponseDTO>> GetStockInByCode(string stockInCode)
        {
            if(string.IsNullOrEmpty(stockInCode))
            {
                return new ServiceResponse<StockInResponseDTO>(false, "Mã phiếu nhập không được để trống");
            }

            var stockIn = await _stockInRepository.GetStockInWithCode(stockInCode);
            if(stockIn==null)
            {
                return new ServiceResponse<StockInResponseDTO>(false, "Không tìm thấy phiếu nhập");
            }    
            var stockInResponse = new StockInResponseDTO
            {
                StockInCode = stockIn.StockInCode,
                OrderTypeCode = stockIn.OrderTypeCode,
                OrderTypeName = stockIn.OrderTypeCodeNavigation!.OrderTypeName,
                WarehouseCode = stockIn.WarehouseCode,
                WarehouseName = stockIn.WarehouseCodeNavigation!.WarehouseName,
                PurchaseOrderCode = stockIn.PurchaseOrderCode,
                SupplierCode = stockIn.PurchaseOrderCodeNavigation!.SupplierCode,
                SupplierName = stockIn.PurchaseOrderCodeNavigation!.SupplierCodeNavigation!.SupplierName,
                Responsible = stockIn.Responsible,
                FullNameResponsible = stockIn.ResponsibleNavigation!.FullName,
                StatusId = stockIn.StatusId,
                StatusName = stockIn.Status!.StatusName,
                OrderDeadline = stockIn.OrderDeadline!.Value.ToString("dd/MM/yyyy"),
                StockInDescription = stockIn.StockInDescription,
            };
            return new ServiceResponse<StockInResponseDTO>(true, "Lấy được dữ liệu của phiếu nhập", stockInResponse);
        }

        public async Task<ServiceResponse<PagedResponse<StockInResponseDTO>>> SearchStockInAsync(string[] warehouseCodes, string textToSearch, int page, int pageSize)
        {
            if (warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<StockInResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var query = _stockInRepository.SearchStockInAsync(warehouseCodes,textToSearch);
            var result = await query
                         .Select(x => new StockInResponseDTO
                         {
                             StockInCode = x.StockInCode,
                             OrderTypeCode = x.OrderTypeCode,
                             OrderTypeName = x.OrderTypeCodeNavigation!.OrderTypeName,
                             WarehouseCode = x.WarehouseCode,
                             WarehouseName = x.WarehouseCodeNavigation!.WarehouseName,
                             PurchaseOrderCode = x.PurchaseOrderCode,
                             SupplierCode = x.PurchaseOrderCodeNavigation!.SupplierCode,
                             SupplierName = x.PurchaseOrderCodeNavigation!.SupplierCodeNavigation!.SupplierName,
                             Responsible = x.Responsible,
                             FullNameResponsible = x.ResponsibleNavigation!.FullName,
                             StatusId = x.StatusId,
                             StatusName = x.Status!.StatusName,
                             OrderDeadline = x.OrderDeadline!.Value.ToString("dd/MM/yyyy"),
                             StockInDescription = x.StockInDescription,
                         })
                         .OrderBy(x => x.StatusId)
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize)
                         .ToListAsync();
            var totalItems = await query.CountAsync();
            var pagedResponse = new PagedResponse<StockInResponseDTO>(result, page, pageSize, totalItems);
            return new ServiceResponse<PagedResponse<StockInResponseDTO>>(true, "Lấy danh sách lệnh nhập kho thành công", pagedResponse);
        }

        public async Task<ServiceResponse<PagedResponse<StockInResponseDTO>>> SearchStockInWithResponsible(string[] warehouseCodes,string responsible, string textToSearch, int page, int pageSize)
        {
            if (warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<StockInResponseDTO>>(false, "Dữu liệu nhận vào không hợp lệ");
            }
            var query = _stockInRepository.SearchStockInWithResponsible(warehouseCodes,responsible, textToSearch);
            var result = await query
                         .Select(x => new StockInResponseDTO
                         {
                             StockInCode = x.StockInCode,
                             OrderTypeCode = x.OrderTypeCode,
                             OrderTypeName = x.OrderTypeCodeNavigation!.OrderTypeName,
                             WarehouseCode = x.WarehouseCode,
                             WarehouseName = x.WarehouseCodeNavigation!.WarehouseName,
                             PurchaseOrderCode = x.PurchaseOrderCode,
                             SupplierCode = x.PurchaseOrderCodeNavigation!.SupplierCode,
                             SupplierName = x.PurchaseOrderCodeNavigation!.SupplierCodeNavigation!.SupplierName,
                             Responsible = x.Responsible,
                             FullNameResponsible = x.ResponsibleNavigation!.FullName,
                             StatusId = x.StatusId,
                             StatusName = x.Status!.StatusName,
                             OrderDeadline = x.OrderDeadline!.Value.ToString("dd/MM/yyyy"),
                             StockInDescription = x.StockInDescription,
                         })
                         .OrderBy(x => x.StatusId)
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize)
                         .ToListAsync();
            var totalItems = await query.CountAsync();
            var pagedResponse = new PagedResponse<StockInResponseDTO>(result, page, pageSize, totalItems);
            return new ServiceResponse<PagedResponse<StockInResponseDTO>>(true, "Lấy danh sách lệnh nhập kho thành công", pagedResponse);
        }

        public async Task<ServiceResponse<bool>> UpdateStockIn(StockInRequestDTO stockIn)
        {
            if (stockIn == null) return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            var existingStockIn = await _stockInRepository.GetStockInWithCode(stockIn.StockInCode);
            if(existingStockIn == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy lệnh nhập kho");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    existingStockIn.StatusId = stockIn.StatusId;
                    existingStockIn.StockInDescription = stockIn.StockInDescription;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Cập nhật lệnh nhập kho thành công");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật lệnh nhập kho: {ex.Message}");
                }
            }
        }
    }
}
