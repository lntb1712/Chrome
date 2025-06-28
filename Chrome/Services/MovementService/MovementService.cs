using Chrome.DTO;
using Chrome.DTO.AccountManagementDTO;
using Chrome.DTO.LocationMasterDTO;
using Chrome.DTO.MovementDTO;
using Chrome.DTO.OrderTypeDTO;
using Chrome.DTO.PickListDTO;
using Chrome.DTO.PutAwayDTO;
using Chrome.DTO.StatusMasterDTO;
using Chrome.DTO.WarehouseMasterDTO;
using Chrome.Models;
using Chrome.Repositories.AccountRepository;
using Chrome.Repositories.LocationMasterRepository;
using Chrome.Repositories.MovementRepository;
using Chrome.Repositories.OrderTypeRepository;
using Chrome.Repositories.PickListRepository;
using Chrome.Repositories.PutawayRepository;
using Chrome.Repositories.StatusMasterRepository;
using Chrome.Repositories.WarehouseMasterRepository;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Chrome.Services.MovementService
{
    public class MovementService : IMovementService
    {
        private readonly IMovementRepository _movementRepository;
        private readonly IOrderTypeRepository _orderTypeRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IStatusMasterRepository _statusMasterRepository;
        private readonly IWarehouseMasterRepository _warehouseMasterRepository;
        private readonly ILocationMasterRepository _locationMasterRepository;
        private readonly IPutAwayRepository _putAwayRepository;
        private readonly IPickListRepository _pickListRepository;
        private readonly ChromeContext _context;

        public MovementService(
            IMovementRepository movementRepository,
            IOrderTypeRepository orderTypeRepository,
            IAccountRepository accountRepository,
            IStatusMasterRepository statusMasterRepository,
            IWarehouseMasterRepository warehouseMasterRepository,
            ILocationMasterRepository locationMasterRepository,
            IPutAwayRepository putAwayRepository,
            IPickListRepository pickListRepository,
            ChromeContext context)
        {
            _movementRepository = movementRepository;
            _orderTypeRepository = orderTypeRepository;
            _accountRepository = accountRepository;
            _statusMasterRepository = statusMasterRepository;
            _warehouseMasterRepository = warehouseMasterRepository;
            _locationMasterRepository = locationMasterRepository;
            _putAwayRepository = putAwayRepository;
            _pickListRepository = pickListRepository;
            _context = context;
        }

        public async Task<ServiceResponse<bool>> AddMovement(MovementRequestDTO movement)
        {
            if (movement == null) return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            if (string.IsNullOrEmpty(movement.MovementCode)) return new ServiceResponse<bool>(false, "Mã chuyển kệ không được để trống");
            string[] formats = {
                "M/d/yyyy h:mm:ss tt",
                "MM/dd/yyyy hh:mm:ss tt",
                "dd/MM/yyyy"
            };
            if (!DateTime.TryParseExact(movement.MovementDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return new ServiceResponse<bool>(false, "Ngày chuyển kệ không đúng định dạng. Vui lòng sử dụng dd/MM/yyyy hoặc M/d/yyyy h:mm:ss tt.");
            }
            var movementRequest = new Movement
            {
                MovementCode = movement.MovementCode,
                OrderTypeCode = movement.OrderTypeCode,
                WarehouseCode = movement.WarehouseCode,
                FromLocation = movement.FromLocation,
                ToLocation = movement.ToLocation,
                Responsible = movement.Responsible,
                StatusId = 1,
                MovementDate = parsedDate,
                MovementDescription = movement.MovementDescription
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _movementRepository.AddAsync(movementRequest, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Thêm lệnh chuyển kệ thành công");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi thêm lệnh chuyển kệ: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteMovementAsync(string movementCode)
        {
            if (string.IsNullOrEmpty(movementCode))
            {
                return new ServiceResponse<bool>(false, "Mã chuyển kệ không được để trống");
            }
            var movement = await _movementRepository.GetMovementWithMovementCode(movementCode);
            if (movement == null)
            {
                return new ServiceResponse<bool>(false, "Lệnh chuyển kệ không tồn tại");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _movementRepository.DeleteAsync(movementCode, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa lệnh chuyển kệ thành công");
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("foreign key"))
                        {
                            return new ServiceResponse<bool>(false, "Không thể xóa lệnh chuyển vì có dữ liệu tham chiếu.");
                        }
                    }
                    return new ServiceResponse<bool>(false, $"Lỗi database: {dbEx.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, $"Lỗi khi xóa lệnh chuyển kệ: {ex.Message}");
                }
            }
        }

        public async Task<ServiceResponse<PagedResponse<MovementResponseDTO>>> GetAllMovements(string[] warehouseCodes, int page, int pageSize)
        {
            if (warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<MovementResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var query = _movementRepository.GetAllMovementAsync(warehouseCodes);
            var result = await query
                .Select(x => new MovementResponseDTO
                {
                    MovementCode = x.MovementCode,
                    OrderTypeCode = x.OrderTypeCode,
                    OrderTypeName = x.OrderTypeCodeNavigation!.OrderTypeName,
                    WarehouseCode = x.WarehouseCode,
                    WarehouseName = x.WarehouseCodeNavigation!.WarehouseName,
                    FromLocation = x.FromLocation,
                    FromLocationName = x.FromLocationNavigation!.LocationName,
                    ToLocation = x.ToLocation,
                    ToLocationName = x.ToLocationNavigation!.LocationName,
                    Responsible = x.Responsible,
                    FullNameResponible = x.ResponsibleNavigation!.FullName,
                    StatusId = x.StatusId,
                    StatusName = x.Status!.StatusName,
                    MovementDate = x.MovementDate!.Value.ToString("dd/MM/yyyy"),
                    MovementDescription = x.MovementDescription
                })
                .OrderBy(x => x.MovementCode)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var totalItems = await query.CountAsync();
            var pagedResponse = new PagedResponse<MovementResponseDTO>(result, page, pageSize, totalItems);
            return new ServiceResponse<PagedResponse<MovementResponseDTO>>(true, "Lấy danh sách lệnh chuyển kệ thành công", pagedResponse);
        }

        public async Task<ServiceResponse<PagedResponse<MovementResponseDTO>>> GetAllMovementsWithStatus(string[] warehouseCodes, int statusId, int page, int pageSize)
        {
            if (warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<MovementResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var query = _movementRepository.GetAllMovementWithStatus(warehouseCodes, statusId);
            var result = await query
                .Select(x => new MovementResponseDTO
                {
                    MovementCode = x.MovementCode,
                    OrderTypeCode = x.OrderTypeCode,
                    OrderTypeName = x.OrderTypeCodeNavigation!.OrderTypeName,
                    WarehouseCode = x.WarehouseCode,
                    WarehouseName = x.WarehouseCodeNavigation!.WarehouseName,
                    FromLocation = x.FromLocation,
                    FromLocationName = x.FromLocationNavigation!.LocationName,
                    ToLocation = x.ToLocation,
                    ToLocationName = x.ToLocationNavigation!.LocationName,
                    Responsible = x.Responsible,
                    FullNameResponible = x.ResponsibleNavigation!.FullName,
                    StatusId = x.StatusId,
                    StatusName = x.Status!.StatusName,
                    MovementDate = x.MovementDate!.Value.ToString("dd/MM/yyyy"),
                    MovementDescription = x.MovementDescription
                })
                .OrderBy(x => x.MovementCode)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var totalItems = await query.CountAsync();
            var pagedResponse = new PagedResponse<MovementResponseDTO>(result, page, pageSize, totalItems);
            return new ServiceResponse<PagedResponse<MovementResponseDTO>>(true, "Lấy danh sách lệnh chuyển kệ thành công", pagedResponse);
        }
      
        public async Task<ServiceResponse<List<LocationMasterResponseDTO>>> GetListToLocation(string warehouseCode,string fromLocation)
        {
            if (string.IsNullOrEmpty(warehouseCode))
            {
                return new ServiceResponse<List<LocationMasterResponseDTO>>(false, "Mã kho không được để trống");
            }
            try
            {
                var locations = await _locationMasterRepository.GetAllLocationMaster(warehouseCode, 1, int.MaxValue);
                var fromLocationStorage = await _locationMasterRepository.GetLocationMasterWithCode(warehouseCode, fromLocation);
                var locationResponse = locations.Where(x => x.IsEmpty == true && x.StorageProduct!.ProductCode == fromLocationStorage.StorageProduct!.ProductCode)
                .Select(l => new LocationMasterResponseDTO
                {
                    LocationCode = l.LocationCode,
                    LocationName = l.LocationName,
                    WarehouseCode = l.WarehouseCode,
                    WarehouseName = l.WarehouseCodeNavigation!.WarehouseName,
                    StorageProductId = l.StorageProductId,
                    StorageProductName = l.StorageProduct!.StorageProductName,
                    IsEmpty = l.IsEmpty,
                }).ToList();

                return new ServiceResponse<List<LocationMasterResponseDTO>>(true, "Lấy danh sách vị trí thành công", locationResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<LocationMasterResponseDTO>>(false, $"Lỗi khi lấy danh sách vị trí: {ex.Message}");
            }

        }
       
        public async Task<ServiceResponse<List<LocationMasterResponseDTO>>> GetListFromLocation(string warehouseCode)
        {
            if (string.IsNullOrEmpty(warehouseCode))
            {
                return new ServiceResponse<List<LocationMasterResponseDTO>>(false, "Mã kho không được để trống");
            }
            try
            {
                var locations = await _locationMasterRepository.GetAllLocationMaster(warehouseCode,1,int.MaxValue);
                var locationResponse = locations
                .Select(l => new LocationMasterResponseDTO
                {
                    LocationCode = l.LocationCode,
                    LocationName = l.LocationName,
                    WarehouseCode = l.WarehouseCode,
                    WarehouseName = l.WarehouseCodeNavigation!.WarehouseName,
                    StorageProductId = l.StorageProductId,
                    StorageProductName = l.StorageProduct!.StorageProductName,
                    IsEmpty = l.IsEmpty,
                }).ToList();

                return new ServiceResponse<List<LocationMasterResponseDTO>>(true, "Lấy danh sách vị trí thành công", locationResponse);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<LocationMasterResponseDTO>>(false, $"Lỗi khi lấy danh sách vị trí: {ex.Message}");
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
            return new ServiceResponse<List<OrderTypeResponseDTO>>(true, "Lấy danh sách loại chuyển kệ thành công", lstOrderTypeList);
        }

        public async Task<ServiceResponse<List<AccountManagementResponseDTO>>> GetListResponsibleAsync()
        {
            var lstResponsible = await _accountRepository.GetAllAccount(1, int.MaxValue);
            var lstResponsibleForMovement = lstResponsible
                .Where(x => !x.GroupId!.StartsWith("ADMIN") && !x.GroupId.StartsWith("QLKHO"))
                .Select(x => new AccountManagementResponseDTO
                {
                    UserName = x.UserName,
                    FullName = x.FullName!,
                    GroupID = x.GroupId!,
                    GroupName = x.Group!.GroupId,
                    Password = x.Password!
                }).ToList();
            return new ServiceResponse<List<AccountManagementResponseDTO>>(true, "Lấy danh sách nhân viên chịu trách nhiệm thành công", lstResponsibleForMovement);
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
            return new ServiceResponse<List<WarehouseMasterResponseDTO>>(true, "Lấy danh sách kệ dựa theo quyền thành công", lstWarehouseMapping);
        }

        public async Task<ServiceResponse<PickListResponseDTO>> GetPickListContainsMovement(string movementCode)
        {
            if(string.IsNullOrEmpty(movementCode))
            {
                return new ServiceResponse<PickListResponseDTO>(false, "Mã chuyển kệ không được để trống ");
            }    
            var pickList=  await _pickListRepository.GetPickListContainCode(movementCode);
            var pickListResponse = new PickListResponseDTO
            {
                PickNo = pickList.PickNo,
                ReservationCode = pickList.ReservationCode,
                WarehouseCode = pickList.WarehouseCode,
                WarehouseName = pickList.WarehouseCodeNavigation!.WarehouseName,
                PickDate = pickList.PickDate!.Value.ToString("dd/MM/yyyy"),
                StatusId = pickList.StatusId,
                StatusName = pickList.Status!.StatusName,
            };
            return new ServiceResponse<PickListResponseDTO>(true, "Lấy thông tin lệnh lấy hàng cho chuyển kệ thành công", pickListResponse);
        }

        public async Task<ServiceResponse<PutAwayResponseDTO>> GetPutAwayContainsMovement(string movementCode)
        {
            if(string.IsNullOrEmpty(movementCode))
            {
                return new ServiceResponse<PutAwayResponseDTO>(false, "Mã chuyển kệ không được để trống");
            }

            var putAway = await _putAwayRepository.GetPutAwayContainsMovement(movementCode);
            var putAwayResponse = new PutAwayResponseDTO
            {
                PutAwayCode = putAway.PutAwayCode,
                LocationCode = putAway.LocationCode,
                LocationName = putAway.LocationCodeNavigation!.LocationName,
                OrderTypeCode = putAway.OrderTypeCode,
                OrderTypeName = putAway.OrderTypeCodeNavigation!.OrderTypeName,
                Responsible = putAway.Responsible,
                FullNameResponsible = putAway.ResponsibleNavigation!.FullName,
                PutAwayDate = putAway.PutAwayDate!.Value.ToString("dd/MM/yyyy"),
                PutAwayDescription = putAway.PutAwayDescription,
                StatusId = putAway.StatusId,
                StatusName = putAway.Status!.StatusName,
            };

            return new ServiceResponse<PutAwayResponseDTO>(true, "Lấy thông tin lệnh để hàng cho chuyển kệ thành công", putAwayResponse);
        }

        public async Task<ServiceResponse<PagedResponse<MovementResponseDTO>>> SearchMovementAsync(string[] warehouseCodes, string textToSearch, int page, int pageSize)
        {
            if (warehouseCodes.Length == 0 || page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<MovementResponseDTO>>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var query = _movementRepository.SearchMovementAsync(warehouseCodes, textToSearch);
            var result = await query
                .Select(x => new MovementResponseDTO
                {
                    MovementCode = x.MovementCode,
                    OrderTypeCode = x.OrderTypeCode,
                    OrderTypeName = x.OrderTypeCodeNavigation!.OrderTypeName,
                    WarehouseCode = x.WarehouseCode,
                    WarehouseName = x.WarehouseCodeNavigation!.WarehouseName,
                    FromLocation = x.FromLocation,
                    FromLocationName = x.FromLocationNavigation!.LocationName,
                    ToLocation = x.ToLocation,
                    ToLocationName = x.ToLocationNavigation!.LocationName,
                    Responsible = x.Responsible,
                    FullNameResponible = x.ResponsibleNavigation!.FullName,
                    StatusId = x.StatusId,
                    StatusName = x.Status!.StatusName,
                    MovementDate = x.MovementDate!.Value.ToString("dd/MM/yyyy"),
                    MovementDescription = x.MovementDescription
                })
                .OrderBy(x => x.MovementCode)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var totalItems = await query.CountAsync();
            var pagedResponse = new PagedResponse<MovementResponseDTO>(result, page, pageSize, totalItems);
            return new ServiceResponse<PagedResponse<MovementResponseDTO>>(true, "Lấy danh sách lệnh chuyển kệ thành công", pagedResponse);
        }

        public async Task<ServiceResponse<bool>> UpdateMovement(MovementRequestDTO movement)
        {
            if (movement == null) return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            var existingMovement = await _movementRepository.GetMovementWithMovementCode(movement.MovementCode);
            if (existingMovement == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy lệnh chuyển kệ");
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    existingMovement.StatusId = movement.StatusId;
                    existingMovement.MovementDescription = movement.MovementDescription;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Cập nhật lệnh chuyển kệ thành công");
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
                    return new ServiceResponse<bool>(false, $"Lỗi khi cập nhật lệnh chuyển kệ: {ex.Message}");
                }
            }
        }
    }
}
