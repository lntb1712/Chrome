using Chrome.DTO;
using Chrome.DTO.AccountManagementDTO;
using Chrome.DTO.OrderTypeDTO;
using Chrome.DTO.StatusMasterDTO;
using Chrome.DTO.TransferDTO;
using Chrome.DTO.WarehouseMasterDTO;
using Chrome.Models;
using Chrome.Repositories.AccountRepository;
using Chrome.Repositories.OrderTypeRepository;
using Chrome.Repositories.StatusMasterRepository;
using Chrome.Repositories.TransferRepository;
using Chrome.Repositories.WarehouseMasterRepository;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Chrome.Services.TransferService
{
    public class TransferService : ITransferService
    {
        private readonly ITransferRepository _transferRepository;
        private readonly IOrderTypeRepository _orderTypeRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IStatusMasterRepository _statusMasterRepository;
        private readonly IWarehouseMasterRepository _warehouseMasterRepository;
        private readonly ChromeContext _context;

        public TransferService(
            ITransferRepository transferRepository,
            IOrderTypeRepository orderTypeRepository,
            IAccountRepository accountRepository,
            IStatusMasterRepository statusMasterRepository,
            IWarehouseMasterRepository warehouseMasterRepository,
            ChromeContext context)
        {
            _transferRepository = transferRepository;
            _orderTypeRepository = orderTypeRepository;
            _accountRepository = accountRepository;
            _statusMasterRepository = statusMasterRepository;
            _warehouseMasterRepository = warehouseMasterRepository;
            _context = context;
        }

        public async Task<ServiceResponse<bool>> AddTransfer(TransferRequestDTO transfer)
        {
            if (transfer == null) return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            if (string.IsNullOrEmpty(transfer.TransferCode)) return new ServiceResponse<bool>(false, "Mã chuyển kho không được để trống");
            string[] formats = {
                "M/d/yyyy h:mm:ss tt",
                "MM/dd/yyyy hh:mm:ss tt",
                "dd/MM/yyyy"
            };
            if (!DateTime.TryParseExact(transfer.TransferDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return new ServiceResponse<bool>(false, "Ngày chuyển kho không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy hoặc M/d/yyyy h:mm:ss tt.");
            }
            var transferRequest = new Transfer
            {
                TransferCode = transfer.TransferCode,
                OrderTypeCode = transfer.OrderTypeCode,
                FromWarehouseCode = transfer.FromWarehouseCode,
                ToWarehouseCode = transfer.ToWarehouseCode,
                FromResponsible = transfer.FromResponsible,
                ToResponsible = transfer.ToResponsible,
                StatusId = 1,
                TransferDate = parsedDate,
                TransferDescription = transfer.TransferDescription
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _transferRepository.AddAsync(transferRequest, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Thêm lệnh chuyển kho thành công");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi thêm lệnh chuyển kho: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteTransferAsync(string transferCode)
        {
            if (string.IsNullOrEmpty(transferCode))
            {
                return new ServiceResponse<bool>(false, "Mã chuyển kho không được để trống");
            }
            var transfer = await _transferRepository.GetTransferWithTransferCode(transferCode);
            if (transfer == null)
            {
                return new ServiceResponse<bool>(false, "Lệnh chuyển kho không tồn tại");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _transferRepository.DeleteAsync(transferCode, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa lệnh chuyển kho thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("foreign key"))
                        {
                            return new ServiceResponse<bool>(false, "Không thể xóa lệnh chuyển kho vì có dữ liệu tham chiếu.");
                        }
                    }
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xóa lệnh chuyển kho: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<PagedResponse<TransferResponseDTO>>> GetAllTransfers(string[] warehouseCodes, int page, int pageSize)
        {
            if (warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<TransferResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var query = _transferRepository.GetAllTransfersAsync(warehouseCodes);
            var result = await query
                .Select(x => new TransferResponseDTO
                {
                    TransferCode = x.TransferCode,
                    OrderTypeCode = x.OrderTypeCode,
                    OrderTypeName = x.OrderTypeCodeNavigation!.OrderTypeName,
                    FromWarehouseCode = x.FromWarehouseCode,
                    FromWarehouseName = x.FromWarehouseCodeNavigation!.WarehouseName,
                    ToWarehouseCode = x.ToWarehouseCode,
                    ToWarehouseName = x.ToWarehouseCodeNavigation!.WarehouseName,
                    FromResponsible = x.FromResponsible,
                    FullNameFromResponsible = x.FromResponsibleNavigation!.FullName,
                    ToResponsible = x.ToResponsible,
                    FullNameToResponsible = x.ToResponsibleNavigation!.FullName,
                    StatusId = x.StatusId,
                    StatusName = x.Status!.StatusName,
                    TransferDate = x.TransferDate!.Value.ToString("dd/MM/yyyy"),
                    TransferDescription = x.TransferDescription
                })
                .OrderBy(x => x.StatusId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var totalItems = await query.CountAsync();
            var pagedResponse = new PagedResponse<TransferResponseDTO>(result, page, pageSize, totalItems);
            return new ServiceResponse<PagedResponse<TransferResponseDTO>>(true, "Lấy danh sách lệnh chuyển kho thành công", pagedResponse);
        }

        public async Task<ServiceResponse<PagedResponse<TransferResponseDTO>>> GetAllTransfersWithStatus(string[] warehouseCodes, int statusId, int page, int pageSize)
        {
            if (warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<TransferResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var query = _transferRepository.GetAllTransfersWithStatus(warehouseCodes, statusId);
            var result = await query
                .Select(x => new TransferResponseDTO
                {
                    TransferCode = x.TransferCode,
                    OrderTypeCode = x.OrderTypeCode,
                    OrderTypeName = x.OrderTypeCodeNavigation!.OrderTypeName,
                    FromWarehouseCode = x.FromWarehouseCode,
                    FromWarehouseName = x.FromWarehouseCodeNavigation!.WarehouseName,
                    ToWarehouseCode = x.ToWarehouseCode,
                    ToWarehouseName = x.ToWarehouseCodeNavigation!.WarehouseName,
                    FromResponsible = x.FromResponsible,
                    FullNameFromResponsible = x.FromResponsibleNavigation!.FullName,
                    ToResponsible = x.ToResponsible,
                    FullNameToResponsible = x.ToResponsibleNavigation!.FullName,
                    StatusId = x.StatusId,
                    StatusName = x.Status!.StatusName,
                    TransferDate = x.TransferDate!.Value.ToString("dd/MM/yyyy"),
                    TransferDescription = x.TransferDescription
                })
                .OrderBy(x => x.TransferCode)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var totalItems = await query.CountAsync();
            var pagedResponse = new PagedResponse<TransferResponseDTO>(result, page, pageSize, totalItems);
            return new ServiceResponse<PagedResponse<TransferResponseDTO>>(true, "Lấy danh sách lệnh chuyển kho thành công", pagedResponse);
        }

        public async Task<ServiceResponse<List<AccountManagementResponseDTO>>> GetListFromResponsibleAsync(string warehouseCode)
        {
            if (string.IsNullOrEmpty(warehouseCode))
            {
                return new ServiceResponse<List<AccountManagementResponseDTO>>(false, "Mã kho không được để trống");
            }
            try
            {
                var lstResponsible = await _accountRepository.GetAllAccount(1, int.MaxValue);
                var lstResponsibleForTransfer = lstResponsible
                    .Where(x => !x.GroupId!.StartsWith("ADMIN") && !x.GroupId.StartsWith("QLKHO") && !x.GroupId.StartsWith("QLSX") && x.Group!.GroupFunctions.Select(x => x.ApplicableLocation).FirstOrDefault() == warehouseCode)
                    .Select(x => new AccountManagementResponseDTO
                    {
                        UserName = x.UserName,
                        FullName = x.FullName!,
                        GroupID = x.GroupId!,
                        GroupName = x.Group!.GroupId,
                        Password = x.Password!
                    }).ToList();
                return new ServiceResponse<List<AccountManagementResponseDTO>>(true, "Lấy danh sách nhân viên chịu trách nhiệm nguồn thành công", lstResponsibleForTransfer);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<AccountManagementResponseDTO>>(false, $"Lỗi khi lấy danh sách nhân viên chịu trách nhiệm nguồn: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<List<AccountManagementResponseDTO>>> GetListToResponsibleAsync(string warehouseCode)
        {
            if (string.IsNullOrEmpty(warehouseCode))
            {
                return new ServiceResponse<List<AccountManagementResponseDTO>>(false, "Mã kho không được để trống");
            }
            try
            {
                var lstResponsible = await _accountRepository.GetAllAccount(1, int.MaxValue);
                var lstResponsibleForTransfer = lstResponsible
                    .Where(x => !x.GroupId!.StartsWith("ADMIN") && !x.GroupId.StartsWith("QLKHO") && x.Group!.GroupFunctions.Select(x => x.ApplicableLocation).FirstOrDefault() == warehouseCode)
                    .Select(x => new AccountManagementResponseDTO
                    {
                        UserName = x.UserName,
                        FullName = x.FullName!,
                        GroupID = x.GroupId!,
                        GroupName = x.Group!.GroupId,
                        Password = x.Password!
                    }).ToList();
                return new ServiceResponse<List<AccountManagementResponseDTO>>(true, "Lấy danh sách nhân viên chịu trách nhiệm đích thành công", lstResponsibleForTransfer);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<AccountManagementResponseDTO>>(false, $"Lỗi khi lấy danh sách nhân viên chịu trách nhiệm đích: {ex.Message}");
            }
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
                OrderTypeName = x.OrderTypeName
            }).ToList();
            return new ServiceResponse<List<OrderTypeResponseDTO>>(true, "Lấy danh sách loại chuyển kho thành công", lstOrderTypeList);
        }

        public async Task<ServiceResponse<List<StatusMasterResponseDTO>>> GetListStatusMaster()
        {
            var lstStatus = await _statusMasterRepository.GetAllStatuses();
            var lstStatusResponse = lstStatus.Select(x => new StatusMasterResponseDTO
            {
                StatusId = x.StatusId,
                StatusName = x.StatusName
            }).ToList();
            return new ServiceResponse<List<StatusMasterResponseDTO>>(true, "Lấy danh sách trạng thái thành công", lstStatusResponse);
        }

        public async Task<ServiceResponse<List<WarehouseMasterResponseDTO>>> GetListWarehousePermission(string[] warehouseCodes)
        {
            if (warehouseCodes.Length == 0)
            {
                return new ServiceResponse<List<WarehouseMasterResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var response = await _warehouseMasterRepository.GetWarehouseMasters(1, int.MaxValue);
            var lstWarehouseMapping = response
                .Where(x => warehouseCodes.Contains(x.WarehouseCode))
                .Select(x => new WarehouseMasterResponseDTO
                {
                    WarehouseCode = x.WarehouseCode,
                    WarehouseName = x.WarehouseName,
                    WarehouseAddress = x.WarehouseAddress,
                    WarehouseDescription = x.WarehouseDescription
                })
                .ToList();
            return new ServiceResponse<List<WarehouseMasterResponseDTO>>(true, "Lấy danh sách kho dựa theo quyền thành công", lstWarehouseMapping);
        }

        public async Task<ServiceResponse<PagedResponse<TransferResponseDTO>>> SearchTransfersAsync(string[] warehouseCodes, string textToSearch, int page, int pageSize)
        {
            if (warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<TransferResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var query = _transferRepository.SearchTransfersAsync(warehouseCodes, textToSearch);
            var result = await query
                .Select(x => new TransferResponseDTO
                {
                    TransferCode = x.TransferCode,
                    OrderTypeCode = x.OrderTypeCode,
                    OrderTypeName = x.OrderTypeCodeNavigation!.OrderTypeName,
                    FromWarehouseCode = x.FromWarehouseCode,
                    FromWarehouseName = x.FromWarehouseCodeNavigation!.WarehouseName,
                    ToWarehouseCode = x.ToWarehouseCode,
                    ToWarehouseName = x.ToWarehouseCodeNavigation!.WarehouseName,
                    FromResponsible = x.FromResponsible,
                    FullNameFromResponsible = x.FromResponsibleNavigation!.FullName,
                    ToResponsible = x.ToResponsible,
                    FullNameToResponsible = x.ToResponsibleNavigation!.FullName,
                    StatusId = x.StatusId,
                    StatusName = x.Status!.StatusName,
                    TransferDate = x.TransferDate!.Value.ToString("dd/MM/yyyy"),
                    TransferDescription = x.TransferDescription
                })
                .OrderBy(x => x.StatusId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var totalItems = await query.CountAsync();
            var pagedResponse = new PagedResponse<TransferResponseDTO>(result, page, pageSize, totalItems);
            return new ServiceResponse<PagedResponse<TransferResponseDTO>>(true, "Lấy danh sách lệnh chuyển kho thành công", pagedResponse);
        }

        public async Task<ServiceResponse<bool>> UpdateTransfer(TransferRequestDTO transfer)
        {
            if (transfer == null) return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            var existingTransfer = await _transferRepository.GetTransferWithTransferCode(transfer.TransferCode);
            if (existingTransfer == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy lệnh chuyển kho");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    existingTransfer.StatusId = transfer.StatusId;
                    existingTransfer.TransferDescription = transfer.TransferDescription;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Cập nhật lệnh chuyển kho thành công");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật lệnh chuyển kho: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<PagedResponse<TransferResponseDTO>>> GetAllTransfersWithResponsible(string[] warehouseCodes, string responsible, int page, int pageSize)
        {
            if (warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<TransferResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var query = _transferRepository.GetAllTransfersAsync(warehouseCodes);
            var result = await query
                .Where(x=>x.FromResponsible==responsible|| x.ToResponsible== responsible)
                .Select(x => new TransferResponseDTO
                {
                    TransferCode = x.TransferCode,
                    OrderTypeCode = x.OrderTypeCode,
                    OrderTypeName = x.OrderTypeCodeNavigation!.OrderTypeName,
                    FromWarehouseCode = x.FromWarehouseCode,
                    FromWarehouseName = x.FromWarehouseCodeNavigation!.WarehouseName,
                    ToWarehouseCode = x.ToWarehouseCode,
                    ToWarehouseName = x.ToWarehouseCodeNavigation!.WarehouseName,
                    FromResponsible = x.FromResponsible,
                    FullNameFromResponsible = x.FromResponsibleNavigation!.FullName,
                    ToResponsible = x.ToResponsible,
                    FullNameToResponsible = x.ToResponsibleNavigation!.FullName,
                    StatusId = x.StatusId,
                    StatusName = x.Status!.StatusName,
                    TransferDate = x.TransferDate!.Value.ToString("dd/MM/yyyy"),
                    TransferDescription = x.TransferDescription
                })
                .OrderBy(x => x.StatusId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var totalItems = await query.Where(x => x.FromResponsible == responsible || x.ToResponsible == responsible).CountAsync();
            var pagedResponse = new PagedResponse<TransferResponseDTO>(result, page, pageSize, totalItems);
            return new ServiceResponse<PagedResponse<TransferResponseDTO>>(true, "Lấy danh sách lệnh chuyển kho thành công", pagedResponse);
        }

        public async Task<ServiceResponse<PagedResponse<TransferResponseDTO>>> SearchTransfersAsyncWithResponsible(string[] warehouseCodes, string responsible, string textToSearch, int page, int pageSize)
        {
            if (warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<TransferResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var query = _transferRepository.SearchTransfersAsync(warehouseCodes, textToSearch);
            var result = await query
                .Where(x => x.FromResponsible == responsible || x.ToResponsible == responsible)
                .Select(x => new TransferResponseDTO
                {
                    TransferCode = x.TransferCode,
                    OrderTypeCode = x.OrderTypeCode,
                    OrderTypeName = x.OrderTypeCodeNavigation!.OrderTypeName,
                    FromWarehouseCode = x.FromWarehouseCode,
                    FromWarehouseName = x.FromWarehouseCodeNavigation!.WarehouseName,
                    ToWarehouseCode = x.ToWarehouseCode,
                    ToWarehouseName = x.ToWarehouseCodeNavigation!.WarehouseName,
                    FromResponsible = x.FromResponsible,
                    FullNameFromResponsible = x.FromResponsibleNavigation!.FullName,
                    ToResponsible = x.ToResponsible,
                    FullNameToResponsible = x.ToResponsibleNavigation!.FullName,
                    StatusId = x.StatusId,
                    StatusName = x.Status!.StatusName,
                    TransferDate = x.TransferDate!.Value.ToString("dd/MM/yyyy"),
                    TransferDescription = x.TransferDescription
                })
                .OrderBy(x => x.StatusId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var totalItems = await query.Where(x => x.FromResponsible == responsible || x.ToResponsible == responsible).CountAsync();
            var pagedResponse = new PagedResponse<TransferResponseDTO>(result, page, pageSize, totalItems);
            return new ServiceResponse<PagedResponse<TransferResponseDTO>>(true, "Lấy danh sách lệnh chuyển kho thành công", pagedResponse);
        }
    }
}
