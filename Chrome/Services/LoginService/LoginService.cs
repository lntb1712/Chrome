using BCrypt.Net;
using Chrome.DTO;
using Chrome.DTO.LoginDTO;
using Chrome.Repositories.AccountRepository;
using Chrome.Repositories.GroupFunctionRepository;
using Chrome.Services.JWTService;

namespace Chrome.Services.LoginService
{
    public class LoginService : ILoginService
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

        public async Task<ServiceResponse<LoginResponse>> LoginAsync(LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return new ServiceResponse<LoginResponse>(false, "Dữ liệu nhận vào không hợp lệ");
            }

            var account = await _accountRepository.GetAccountWithUserName(loginRequest.Username.Trim());

            if (account == null)
            {
                return new ServiceResponse<LoginResponse>(false, "Tài khoản không tồn tại");
            }
            else if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password.Trim(), account.Password!))
            {
                // Kiểm tra mật khẩu
                return new ServiceResponse<LoginResponse>(false, "Mật khẩu không đúng");
            }

            var permissions = await _groupFunctionRepository.GetListFunctionIDOfGroup(account.GroupId!);

            var token = await _jwtService.GenerateToken(account, permissions);
            if (string.IsNullOrEmpty(token))
            {
                return new ServiceResponse<LoginResponse>(false, "Lỗi trong quá trình tạo token");
            }

            var loginResponse = new LoginResponse
            {
                Token = token,
                GroupId = account.GroupId,
                Username = account.UserName,
            };
            return new ServiceResponse<LoginResponse>(true, "Đăng nhập thành công", loginResponse);

        }
    }
}
