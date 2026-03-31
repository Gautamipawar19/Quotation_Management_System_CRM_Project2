using Microsoft.IdentityModel.Tokens;
using QuotationManagementWebApi.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QuotationManagementWebApi.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(string username, string role)
        {
            var jwtSection = _configuration.GetSection("Jwt");

            var keyString = jwtSection["Key"]
                ?? throw new InvalidOperationException("JWT Key is missing in configuration.");

            var issuer = jwtSection["Issuer"]
                ?? throw new InvalidOperationException("JWT Issuer is missing in configuration.");

            var audience = jwtSection["Audience"]
                ?? throw new InvalidOperationException("JWT Audience is missing in configuration.");

            var expiryMinutesString = jwtSection["ExpiryMinutes"]
                ?? throw new InvalidOperationException("JWT ExpiryMinutes is missing in configuration.");

            if (!double.TryParse(expiryMinutesString, out double expiryMinutes))
            {
                throw new InvalidOperationException("JWT ExpiryMinutes is invalid.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}