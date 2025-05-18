using Chrome.DTO;
using Chrome.DTO.AccountManagementDTO;
using Chrome.Models;
using Chrome.Repositories.AccountRepository;
using Chrome.Services.AccountManagementService;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

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

        public async Task<ServiceResponse<bool>> DeleteAccountManagement(string id)
        {
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

        public async Task<List<AccountManagementResponseDTO>> GetAllAccount()
        {
            var lstAccount = await _accountRepository.GetAllAccount();

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
            return lstAccountResponseDTO;
        }

        public async Task<List<AccountManagementResponseDTO>> GetAllAccountWithGroupId(string groupId)
        {
            var lstAccountWithGroupId= await _accountRepository.GetAllWithRole(groupId);

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
            return lstAccountResponseDTO;
        }

        public async Task<AccountManagementResponseDTO> GetUserInformation(string userName)
        {
            var account = await _accountRepository.GetAccountWithUserName(userName);
            var accountResponse = new AccountManagementResponseDTO
            {
                UserName = account.UserName,
                Password = account.Password!,
                FullName = account.FullName!,
                GroupID = account.GroupId!,
                GroupName = account.Group!.GroupName!,

            };
            return accountResponse;

        }

        public async Task<List<AccountManagementResponseDTO>> SearchAccount(string textToSearch)
        {
            var lstSearchAccoụnt = await _accountRepository.SearchAccount(textToSearch);
            var lstAccountResponseDTO = lstSearchAccoụnt.Select(x => new AccountManagementResponseDTO
            {
                UserName = x.UserName,
                Password = x.Password!,
                FullName = x.FullName!,
                GroupID = x.GroupId!,
                GroupName = x.Group!.GroupName!,
                UpdateBy = x.UpdateBy!,
                UpdateTime = x.UpdateTime!.Value.ToString("dd-MM-yyyy"),
            }).ToList();
            return lstAccountResponseDTO;
        }

        public async Task<ServiceResponse<bool>> UpdateAccountManagement(AccountManagementRequestDTO accountResponse)
        {
            if (accountResponse == null)
            {
                return new ServiceResponse<bool>(false, "Dữ liệu nhận vào không hợp lệ");
            }
            var account =await _accountRepository.GetAccountWithUserName(accountResponse.UserName);
            if (account == null)
            {
                return new ServiceResponse<bool>(false, "Không tìm thấy user hợp lệ");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    account.Password = BCrypt.Net.BCrypt.HashPassword(accountResponse.Password, 12) ;
                    account.FullName = accountResponse.FullName;
                    account.GroupId = accountResponse.GroupID;
                    account.UpdateBy = accountResponse.UpdateBy;
                    account.UpdateTime = DateTime.Now;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse<bool>(true, "Cập nhật thành công", true);

                }
                catch(DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    if (dbEx.InnerException != null)
                    {
                        string error= dbEx.InnerException.Message.ToLower();
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
                    return new ServiceResponse<bool> (false,"Lỗi không xác định: "+ex.Message);
                }
            }

        }
    }
}
