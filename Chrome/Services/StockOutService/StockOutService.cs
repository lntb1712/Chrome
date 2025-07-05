using Chrome.DTO;
using Chrome.DTO.AccountManagementDTO;
using Chrome.DTO.CustomerMasterDTO;
using Chrome.DTO.OrderTypeDTO;
using Chrome.DTO.ReservationDTO;
using Chrome.DTO.StatusMasterDTO;
using Chrome.DTO.StockOutDTO;
using Chrome.DTO.WarehouseMasterDTO;
using Chrome.Models;
using Chrome.Repositories.AccountRepository;
using Chrome.Repositories.CustomerMasterRepository;
using Chrome.Repositories.OrderTypeRepository;
using Chrome.Repositories.ReservationRepository;
using Chrome.Repositories.StatusMasterRepository;
using Chrome.Repositories.StockOutRepository;
using Chrome.Repositories.WarehouseMasterRepository;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Chrome.Services.StockOutService
{
    public class StockOutService : IStockOutService
    {
        private readonly IStockOutRepository _stockOutRepository;
        private readonly IOrderTypeRepository _orderTypeRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICustomerMasterRepository _customerMasterRepository;
        private readonly IStatusMasterRepository _statusMasterRepository;
        private readonly IWarehouseMasterRepository _warehouseMasterRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly ChromeContext _context;

        public StockOutService(
            IStockOutRepository stockOutRepository,
            IOrderTypeRepository orderTypeRepository,
            IAccountRepository accountRepository,
            ICustomerMasterRepository customerMasterRepository,
            IStatusMasterRepository statusMasterRepository,
            IWarehouseMasterRepository warehouseMasterRepository,
            IReservationRepository reservationRepository,
            ChromeContext context)
        {
            _stockOutRepository = stockOutRepository;
            _orderTypeRepository = orderTypeRepository;
            _accountRepository = accountRepository;
            _customerMasterRepository = customerMasterRepository;
            _statusMasterRepository = statusMasterRepository;
            _warehouseMasterRepository = warehouseMasterRepository;
            _reservationRepository = reservationRepository;
            _context = context;
        }

        public async Task<ServiceResponse<bool>> AddStockOut(StockOutRequestDTO stockOut)
        {
            if (stockOut == null) return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            if (string.IsNullOrEmpty(stockOut.StockOutCode)) return new ServiceResponse<bool>(false, "Mã xuất kho không được để trống");
            string[] formats = {
                "M/d/yyyy h:mm:ss tt",
                "MM/dd/yyyy hh:mm:ss tt",
                "dd/MM/yyyy"
            };
            if (!DateTime.TryParseExact(stockOut.StockOutDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return new ServiceResponse<bool>(false, "Ngày xuất kho không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy hoặc M/d/yyyy h:mm:ss tt.");
            }
            var stockOutRequest = new StockOut
            {
                StockOutCode = stockOut.StockOutCode,
                OrderTypeCode = stockOut.OrderTypeCode,
                WarehouseCode = stockOut.WarehouseCode,
                CustomerCode = stockOut.CustomerCode,
                Responsible = stockOut.Responsible,
                StatusId = 1,
                StockOutDate = parsedDate,
                StockOutDescription = stockOut.StockOutDescription,
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _stockOutRepository.AddAsync(stockOutRequest, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Thêm lệnh xuất kho thành công");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi thêm lệnh xuất kho: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteStockOutAsync(string stockOutCode)
        {
            if (string.IsNullOrEmpty(stockOutCode))
            {
                return new ServiceResponse<bool>(false, "Mã xuất kho không được để trống");
            }
            var stockOut = await _stockOutRepository.GetStockOutWithCode(stockOutCode);
            if (stockOut == null)
            {
                return new ServiceResponse<bool>(false, "Lệnh xuất kho không tồn tại");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _stockOutRepository.DeleteAsync(stockOut, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa lệnh xuất kho thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("foreign key"))
                        {
                            return new ServiceResponse<bool>(false, "Không thể xóa lệnh xuất vì có dữ liệu tham chiếu.");
                        }
                    }
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xóa lệnh xuất kho: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<PagedResponse<StockOutResponseDTO>>> GetAllStockOuts(string[] warehouseCodes, int page, int pageSize)
        {
            if (warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<StockOutResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var query = _stockOutRepository.GetAllStockOutAsync(warehouseCodes);
            var result = await query
                         .Select(x => new StockOutResponseDTO
                         {
                             StockOutCode = x.StockOutCode,
                             OrderTypeCode = x.OrderTypeCode,
                             OrderTypeName = x.OrderTypeCodeNavigation!.OrderTypeName,
                             WarehouseCode = x.WarehouseCode,
                             WarehouseName = x.WarehouseCodeNavigation!.WarehouseName,
                             CustomerCode = x.CustomerCode,
                             CustomerName = x.CustomerCodeNavigation!.CustomerName,
                             Responsible = x.Responsible,
                             FullNameResponsible = x.ResponsibleNavigation!.FullName,
                             StatusId = x.StatusId,
                             StatusName = x.Status!.StatusName,
                             StockOutDate = x.StockOutDate!.Value.ToString("dd/MM/yyyy"),
                             StockOutDescription = x.StockOutDescription,
                         })
                         .OrderBy(x => x.StockOutCode)
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize)
                         .ToListAsync();
            var totalItems = await query.CountAsync();
            var pagedResponse = new PagedResponse<StockOutResponseDTO>(result, page, pageSize, totalItems);
            return new ServiceResponse<PagedResponse<StockOutResponseDTO>>(true, "Lấy danh sách lệnh xuất kho thành công", pagedResponse);
        }

        public async Task<ServiceResponse<PagedResponse<StockOutResponseDTO>>> GetAllStockOutsWithStatus(string[] warehouseCodes, int statusId, int page, int pageSize)
        {
            if (warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<StockOutResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var query = _stockOutRepository.GetAllStockOutWithStatus(warehouseCodes, statusId);
            var result = await query
                         .Select(x => new StockOutResponseDTO
                         {
                             StockOutCode = x.StockOutCode,
                             OrderTypeCode = x.OrderTypeCode,
                             OrderTypeName = x.OrderTypeCodeNavigation!.OrderTypeName,
                             WarehouseCode = x.WarehouseCode,
                             WarehouseName = x.WarehouseCodeNavigation!.WarehouseName,
                             CustomerCode = x.CustomerCode,
                             CustomerName = x.CustomerCodeNavigation!.CustomerName,
                             Responsible = x.Responsible,
                             FullNameResponsible = x.ResponsibleNavigation!.FullName,
                             StatusId = x.StatusId,
                             StatusName = x.Status!.StatusName,
                             StockOutDate = x.StockOutDate!.Value.ToString("dd/MM/yyyy"),
                             StockOutDescription = x.StockOutDescription,
                         })
                         .OrderBy(x => x.StockOutCode)
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize)
                         .ToListAsync();
            var totalItems = await query.CountAsync();
            var pagedResponse = new PagedResponse<StockOutResponseDTO>(result, page, pageSize, totalItems);
            return new ServiceResponse<PagedResponse<StockOutResponseDTO>>(true, "Lấy danh sách lệnh xuất kho thành công", pagedResponse);
        }

        public async Task<ServiceResponse<List<OrderTypeResponseDTO>>> GetListOrderType(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                return new ServiceResponse<List<OrderTypeResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var lstOrderTypePrefix = await _orderTypeRepository.GetOrderTypeByCode(prefix);
            var lstOrderTypeList = lstOrderTypePrefix.Select(x => new OrderTypeResponseDTO
            {
                OrderTypeCode = x.OrderTypeCode,
                OrderTypeName = x.OrderTypeName,
            }).ToList();
            return new ServiceResponse<List<OrderTypeResponseDTO>>(true, "Lấy danh sách loại xuất kho thành công", lstOrderTypeList);
        }

        public async Task<ServiceResponse<List<AccountManagementResponseDTO>>> GetListResponsibleAsync(string warehouseCode)
        {
            var lstResponsible = await _accountRepository.GetAllAccount(1, int.MaxValue);
            var lstResponsibleForSO = lstResponsible.Where(x => !x.GroupId!.StartsWith("ADMIN") && !x.GroupId.StartsWith("QLKHO") && x.Group!.GroupFunctions.Select(x => x.ApplicableLocation).FirstOrDefault() == warehouseCode)
                                                    .Select(x => new AccountManagementResponseDTO
                                                    {
                                                        UserName = x.UserName,
                                                        FullName = x.FullName!,
                                                        GroupID = x.GroupId!,
                                                        GroupName = x.Group!.GroupId,
                                                        Password = x.Password!,
                                                    }).ToList();
            return new ServiceResponse<List<AccountManagementResponseDTO>>(true, "Lấy danh sách nhân viên chịu trách nhiệm thành công", lstResponsibleForSO);
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

        public async Task<ServiceResponse<List<CustomerMasterResponseDTO>>> GetListCustomerMasterAsync()
        {
            var lstCustomer = await _customerMasterRepository.GetAllCustomer(1, int.MaxValue);
            var lstCustomerResponse = lstCustomer.Select(x => new CustomerMasterResponseDTO
            {
                CustomerCode = x.CustomerCode,
                CustomerName = x.CustomerName,
                CustomerPhone = x.CustomerPhone,
                CustomerAddress = x.CustomerAddress,
            }).ToList();
            return new ServiceResponse<List<CustomerMasterResponseDTO>>(true, "Lấy danh sách khách hàng", lstCustomerResponse);
        }

        public async Task<ServiceResponse<List<WarehouseMasterResponseDTO>>> GetListWarehousePermission(string[] warehouseCodes)
        {
            if (warehouseCodes.Length == 0)
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

        public async Task<ServiceResponse<PagedResponse<StockOutResponseDTO>>> SearchStockOutAsync(string[] warehouseCodes, string textToSearch, int page, int pageSize)
        {
            if (warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<StockOutResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var query = _stockOutRepository.SearchStockOutAsync(warehouseCodes, textToSearch);
            var result = await query
                         .Select(x => new StockOutResponseDTO
                         {
                             StockOutCode = x.StockOutCode,
                             OrderTypeCode = x.OrderTypeCode,
                             OrderTypeName = x.OrderTypeCodeNavigation!.OrderTypeName,
                             WarehouseCode = x.WarehouseCode,
                             WarehouseName = x.WarehouseCodeNavigation!.WarehouseName,
                             CustomerCode = x.CustomerCode,
                             CustomerName = x.CustomerCodeNavigation!.CustomerName,
                             Responsible = x.Responsible,
                             FullNameResponsible = x.ResponsibleNavigation!.FullName,
                             StatusId = x.StatusId,
                             StatusName = x.Status!.StatusName,
                             StockOutDate = x.StockOutDate!.Value.ToString("dd/MM/yyyy"),
                             StockOutDescription = x.StockOutDescription,
                         })
                         .OrderBy(x => x.StockOutCode)
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize)
                         .ToListAsync();
            var totalItems = await query.CountAsync();
            var pagedResponse = new PagedResponse<StockOutResponseDTO>(result, page, pageSize, totalItems);
            return new ServiceResponse<PagedResponse<StockOutResponseDTO>>(true, "Lấy danh sách lệnh xuất kho thành công", pagedResponse);
        }

        public async Task<ServiceResponse<bool>> UpdateStockOut(StockOutRequestDTO stockOut)
        {
            if (stockOut == null) return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            var existingStockOut = await _stockOutRepository.GetStockOutWithCode(stockOut.StockOutCode);
            if (existingStockOut == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy lệnh xuất kho");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    existingStockOut.StatusId = stockOut.StatusId;
                    existingStockOut.StockOutDescription = stockOut.StockOutDescription;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Cập nhật lệnh xuất kho thành công");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật lệnh xuất kho: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<PagedResponse<StockOutResponseDTO>>> GetAllStockOutsWithResponsible(string[] warehouseCodes, string responsible, int page, int pageSize)
        {
            if (warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<StockOutResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var query = _stockOutRepository.GetAllStockOutAsync(warehouseCodes);
            var result = await query
                          .Where(x=>x.Responsible==responsible)
                         .Select(x => new StockOutResponseDTO
                         {
                             StockOutCode = x.StockOutCode,
                             OrderTypeCode = x.OrderTypeCode,
                             OrderTypeName = x.OrderTypeCodeNavigation!.OrderTypeName,
                             WarehouseCode = x.WarehouseCode,
                             WarehouseName = x.WarehouseCodeNavigation!.WarehouseName,
                             CustomerCode = x.CustomerCode,
                             CustomerName = x.CustomerCodeNavigation!.CustomerName,
                             Responsible = x.Responsible,
                             FullNameResponsible = x.ResponsibleNavigation!.FullName,
                             StatusId = x.StatusId,
                             StatusName = x.Status!.StatusName,
                             StockOutDate = x.StockOutDate!.Value.ToString("dd/MM/yyyy"),
                             StockOutDescription = x.StockOutDescription,
                         })
                         .OrderBy(x => x.StatusId)
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize)
                         .ToListAsync();
            var totalItems = await query.Where(x => x.Responsible == responsible).CountAsync();
            var pagedResponse = new PagedResponse<StockOutResponseDTO>(result, page, pageSize, totalItems);
            return new ServiceResponse<PagedResponse<StockOutResponseDTO>>(true, "Lấy danh sách lệnh xuất kho thành công", pagedResponse);
        }

        public async Task<ServiceResponse<PagedResponse<StockOutResponseDTO>>> SearchStockOutAsyncWithResponsible(string[] warehouseCodes, string responsible, string textToSearch, int page, int pageSize)
        {
            if (warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<StockOutResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var query = _stockOutRepository.SearchStockOutAsync(warehouseCodes, textToSearch);
            var result = await query
                         .Where(x=>x.Responsible==responsible)
                         .Select(x => new StockOutResponseDTO
                         {
                             StockOutCode = x.StockOutCode,
                             OrderTypeCode = x.OrderTypeCode,
                             OrderTypeName = x.OrderTypeCodeNavigation!.OrderTypeName,
                             WarehouseCode = x.WarehouseCode,
                             WarehouseName = x.WarehouseCodeNavigation!.WarehouseName,
                             CustomerCode = x.CustomerCode,
                             CustomerName = x.CustomerCodeNavigation!.CustomerName,
                             Responsible = x.Responsible,
                             FullNameResponsible = x.ResponsibleNavigation!.FullName,
                             StatusId = x.StatusId,
                             StatusName = x.Status!.StatusName,
                             StockOutDate = x.StockOutDate!.Value.ToString("dd/MM/yyyy"),
                             StockOutDescription = x.StockOutDescription,
                         })
                         .OrderBy(x => x.StatusId)
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize)
                         .ToListAsync();
            var totalItems = await query.Where(x => x.Responsible == responsible).CountAsync();
            var pagedResponse = new PagedResponse<StockOutResponseDTO>(result, page, pageSize, totalItems);
            return new ServiceResponse<PagedResponse<StockOutResponseDTO>>(true, "Lấy danh sách lệnh xuất kho thành công", pagedResponse);
        }
    } 
}
