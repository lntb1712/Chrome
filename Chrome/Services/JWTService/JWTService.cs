using Chrome.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Chrome.Services.JWTService
{
    public class JWTService : IJWTService
    {
        private readonly IConfiguration _configuration;

        public JWTService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GenerateToken(AccountManagement accountManagement, List<string> permissions, List<string> warehouses)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, accountManagement.UserName.ToString()),
                new Claim(ClaimTypes.Name, accountManagement.FullName!.ToString()),
                new Claim(ClaimTypes.Role, accountManagement.GroupId!.ToString()),
            };

            // Thêm Permission claims
            foreach (var permission in permissions)
            {
                claims.Add(new Claim("Permission", permission));
            }

            if (warehouses.Count == 1)
            {
                var warehouseJson = JsonSerializer.Serialize(warehouses);
                claims.Add(new Claim("Warehouse", warehouseJson));
            }
            else
            {
                // Thêm Warehouse claims
                foreach (var warehouse in warehouses)
                {
                    
                    claims.Add(new Claim("Warehouse", warehouse));
                }
            }

            var secretKey = Encoding.UTF8.GetBytes(_configuration["AppSettings:SecretKey"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8), // Token sống 8 tiếng chẳng hạn
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            await Task.CompletedTask;
            return tokenHandler.WriteToken(token);
        }
    }
}
