using BCrypt.Net;
using Chrome.DTO.LoginDTO;
using Chrome.Repositories.AccountRepository;
using Chrome.Repositories.GroupFunctionRepository;
using Chrome.Services.JWTService;

namespace Chrome.Services.LoginService
{
    public class LoginService:ILoginService
    {
        private readonly IJWTService _jwtService;
        private readonly IAccountRepository _accountRepository;
        private readonly IGroupFunctionRepository _groupFunctionRepository;

        public LoginService(IJWTService jwtService, IAccountRepository accountRepository, IGroupFunctionRepository groupFunctionRepository)
        {
            _jwtService = jwtService;
            _accountRepository = accountRepository;
            _groupFunctionRepository = groupFunctionRepository;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            if (loginRequest == null|| string.IsNullOrEmpty(loginRequest.Username)||string.IsNullOrEmpty(loginRequest.Password))
            {
                return new LoginResponse { ErrorMessage = "Tên đăng nhập hoặc mật khẩu không hợp lệ" };
            }

            var account = await _accountRepository.GetAccountWithUserName(loginRequest.Username.Trim());

            if (account == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, account.Password))
            {
                return new LoginResponse { ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng" };
            }

            var permissions = await _groupFunctionRepository.GetListFunctionIDOfGroup(account.GroupId!);

            var token = await _jwtService.GenerateToken(account, permissions);

            if (string.IsNullOrEmpty(token))
            {
                return new LoginResponse { ErrorMessage = "Lỗi phát sinh trong quá trình tạo token" };
            }

            return new LoginResponse
            {
                Token = token,
                Username = account.UserName,
                GroupId = account.GroupId,
                ErrorMessage = ""
            };
        }
    }
}
