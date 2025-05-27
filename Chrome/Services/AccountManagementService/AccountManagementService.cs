using Chrome.DTO;
using Chrome.DTO.AccountManagementDTO;
using Chrome.Models;
using Chrome.Repositories.AccountRepository;
using Microsoft.EntityFrameworkCore;

namespace Chrome.Services.AccountManagementService
{
    public class AccountManagementService : IAccountManagementService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ChromeContext _context;

        public AccountManagementService(IAccountRepository accountRepository, ChromeContext context)
        {
            _accountRepository = accountRepository;
            _context = context;
        }

        public async Task<ServiceResponse<PagedResponse<AccountManagementResponseDTO>>> GetAllAccount(int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<AccountManagementResponseDTO>>(false, "Thông số phân trang không hợp lệ");
            }

            var lstAccount = await _accountRepository.GetAllAccount(page, pageSize);
            var totalItems = await _accountRepository.GetTotalAccountCount();

            if (lstAccount == null || lstAccount.Count == 0)
            {
                return new ServiceResponse<PagedResponse<AccountManagementResponseDTO>>(false, "Không có tài khoản nào trong hệ thống");
            }

            var lstAccountResponseDTO = lstAccount.Select(x => new AccountManagementResponseDTO
            {
                UserName = x.UserName,
                Password = x.Password!,
                FullName = x.FullName!,
                GroupID = x.GroupId!,
                GroupName = x.Group!.GroupName!,
                UpdateBy = x.UpdateBy!,
                UpdateTime = x.UpdateTime!.Value.ToString("dd-MM-yyyy"),
            }).ToList();

            var pagedResponse = new PagedResponse<AccountManagementResponseDTO>(lstAccountResponseDTO, page, pageSize, totalItems);
            return new ServiceResponse<PagedResponse<AccountManagementResponseDTO>>(true, "Lấy danh sách người dùng thành công", pagedResponse);
        }

        public async Task<ServiceResponse<PagedResponse<AccountManagementResponseDTO>>> GetAllAccountWithGroupId(string groupId, int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<AccountManagementResponseDTO>>(false, "Thông số phân trang không hợp lệ");
            }

            var lstAccountWithGroupId = await _accountRepository.GetAllWithRole(groupId, page, pageSize);
            var totalItems = await _accountRepository.GetTotalWithRoleCount(groupId);

            if (lstAccountWithGroupId == null || lstAccountWithGroupId.Count == 0)
            {
                return new ServiceResponse<PagedResponse<AccountManagementResponseDTO>>(false, "Không có tài khoản nào trong hệ thống");
            }

            var lstAccountResponseDTO = lstAccountWithGroupId.Select(x => new AccountManagementResponseDTO
            {
                UserName = x.UserName,
                Password = x.Password!,
                FullName = x.FullName!,
                GroupID = x.GroupId!,
                GroupName = x.Group!.GroupName!,
                UpdateBy = x.UpdateBy!,
                UpdateTime = x.UpdateTime!.Value.ToString("dd-MM-yyyy"),
            }).ToList();

            var pagedResponse = new PagedResponse<AccountManagementResponseDTO>(lstAccountResponseDTO, page, pageSize, totalItems);
            return new ServiceResponse<PagedResponse<AccountManagementResponseDTO>>(true, "Lấy danh sách người dùng thành công", pagedResponse);
        }

        public async Task<ServiceResponse<PagedResponse<AccountManagementResponseDTO>>> SearchAccount(string textToSearch, int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                return new ServiceResponse<PagedResponse<AccountManagementResponseDTO>>(false, "Thông số phân trang không hợp lệ");
            }

            var lstSearchAccount = await _accountRepository.SearchAccount(textToSearch, page, pageSize);
            var totalItems = await _accountRepository.GetTotalSearchCount(textToSearch);

            if (lstSearchAccount == null || lstSearchAccount.Count == 0)
            {
                return new ServiceResponse<PagedResponse<AccountManagementResponseDTO>>(false, "Không có tài khoản nào trong hệ thống");
            }

            var lstAccountResponseDTO = lstSearchAccount.Select(x => new AccountManagementResponseDTO
            {
                UserName = x.UserName,
                Password = x.Password!,
                FullName = x.FullName!,
                GroupID = x.GroupId!,
                GroupName = x.Group!.GroupName!,
                UpdateBy = x.UpdateBy!,
                UpdateTime = x.UpdateTime!.Value.ToString("dd-MM-yyyy"),
            }).ToList();

            var pagedResponse = new PagedResponse<AccountManagementResponseDTO>(lstAccountResponseDTO, page, pageSize, totalItems);
            return new ServiceResponse<PagedResponse<AccountManagementResponseDTO>>(true, "Lấy danh sách người dùng thành công", pagedResponse);
        }

        public async Task<ServiceResponse<AccountManagementResponseDTO>> GetUserInformation(string userName)
        {
            var account = await _accountRepository.GetAccountWithUserName(userName);
            if (account == null)
            {
                return new ServiceResponse<AccountManagementResponseDTO>(false, "Không tìm thấy tài khoản hợp lệ");
            }
            var accountResponse = new AccountManagementResponseDTO
            {
                UserName = account.UserName,
                Password = account.Password!,
                FullName = account.FullName!,
                GroupID = account.GroupId!,
                GroupName = account.Group!.GroupName!,
            };
            return new ServiceResponse<AccountManagementResponseDTO>(true, "Lấy thông tin người dùng thành công", accountResponse);
        }

        public async Task<ServiceResponse<bool>> AddAccountManagement(AccountManagementRequestDTO accountRequest)
        {
            if (accountRequest == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            var account = new AccountManagement
            {
                UserName = accountRequest.UserName,
                Password = BCrypt.Net.BCrypt.HashPassword(accountRequest.Password, 12),
                FullName = accountRequest.FullName,
                GroupId = accountRequest.GroupID,
                UpdateBy = accountRequest.UpdateBy,
                UpdateTime = DateTime.Now
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _accountRepository.AddAsync(account, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Thêm mới tài khoản thành công", true);
                }
                catch (DbUpdateException dBEx)
                {
                    await transaction.RollbackAsync();
                    if (dBEx.InnerException != null)
                    {
                        string error = dBEx.InnerException.Message.ToLower();
                        if (error.Contains("unique") || error.Contains("duplicate") || error.Contains("primary key"))
                        {
                            return new ServiceResponse<bool>(false, "Dữ liệu đã tồn tại");
                        }

                        if (error.Contains("foreign key"))
                        {
                            return new ServiceResponse<bool>(false, "Dữ liệu tham chiếu không hợp lệ");
                        }
                    }
                    return new ServiceResponse<bool>(false, "Lỗi database: " + dBEx.InnerException?.Message);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, "Lỗi không xác định: " + ex.Message);
                }
            }
        }

        public async Task<ServiceResponse<bool>> UpdateAccountManagement(AccountManagementRequestDTO accountResponse)
        {
            if (accountResponse == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var account = await _accountRepository.GetAccountWithUserName(accountResponse.UserName);
            if (account == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy user hợp lệ");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var password = accountResponse.Password; // Original password (plaintext or hashed)

                    // Check if the password is already hashed (e.g., starts with $2a$, $2b$, etc., typical BCrypt prefix)
                    if (!string.IsNullOrEmpty(password) && !password.StartsWith("$2"))
                    {
                        // If plaintext, hash it
                        account.Password = BCrypt.Net.BCrypt.HashPassword(password, 12);
                    }
                    else
                    {
                        // If already hashed (e.g., from database), use it as is
                        account.Password = password;
                    }
                    account.FullName = accountResponse.FullName;
                    account.GroupId = accountResponse.GroupID;
                    account.UpdateBy = accountResponse.UpdateBy;
                    account.UpdateTime = DateTime.Now;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Cập nhật thành công", true);
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
                    }
                    return new ServiceResponse<bool>(false, "Lỗi database: " + dbEx.InnerException?.Message);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, "Lỗi không xác định: " + ex.Message);
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteAccountManagement(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var account = await _accountRepository.GetAccountWithUserName(id);
            if(account==null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy tài khoản cần xóa");
            }    
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _accountRepository.DeleteAsync(id, saveChanges: false);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Xóa tài khoản thành công", true);
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error = dbEx.InnerException.Message.ToLower();
                        if (error.Contains("reference"))
                        {
                            return new ServiceResponse<bool>(false, "Không thể xóa tài khoản này vì đang được sử dụng");
                        }
                    }
                    return new ServiceResponse<bool>(false, "Lỗi database: " + dbEx.InnerException?.Message);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<bool>(false, "Lỗi không xác định: " + ex.Message);
                }
            }
        }

        public async Task<ServiceResponse<int>> GetTotalAccount()
        {
            try
            {
                var response = await _accountRepository.GetTotalAccountCount();
                return new ServiceResponse<int>(true, "Lấy tổng người dùng thành công", response);
            }
            catch(Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message);
            }
        }
    }
}