using Chrome.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Chrome.Services.JWTService
{
    public class JWTService:IJWTService
    {
        private readonly IConfiguration _configuration;
        
        public JWTService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GenerateToken(AccountManagement accountManagement, List<string> permissions)
        {
            var claims = new List<Claim>
            {
                new Claim (ClaimTypes.NameIdentifier, accountManagement.UserName.ToString()),
                new Claim (ClaimTypes.Name, accountManagement.FullName!.ToString()),
                new Claim (ClaimTypes.Role, accountManagement.GroupId!.ToString()),
            };

            foreach (var permission in permissions)
            {
                claims.Add(new Claim("Permission", permission));
            }

            var secretKey = Encoding.UTF8.GetBytes(_configuration["AppSettings:SecretKey"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
