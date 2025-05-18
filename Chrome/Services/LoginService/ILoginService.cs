using Chrome.DTO.LoginDTO;

namespace Chrome.Services.LoginService
{
    public interface ILoginService
    {
        Task<LoginResponse> LoginAsync(LoginRequest loginRequest);
    }
}
