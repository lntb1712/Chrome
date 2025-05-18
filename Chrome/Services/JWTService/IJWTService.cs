using Chrome.Models;

namespace Chrome.Services.JWTService
{
    public interface IJWTService
    {
        Task<string> GenerateToken(AccountManagement accountManagement, List<string> permissions);
    }
}
