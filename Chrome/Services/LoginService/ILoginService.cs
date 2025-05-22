using Chrome.DTO;
using Chrome.DTO.LoginDTO;

namespace Chrome.Services.LoginService
{
    public interface ILoginService
    {
        Task<ServiceResponse<LoginResponse>> LoginAsync(LoginRequest loginRequest);
    }
}
