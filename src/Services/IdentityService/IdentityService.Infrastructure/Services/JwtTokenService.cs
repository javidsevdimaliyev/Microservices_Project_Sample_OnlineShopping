using IdentityService.Application.Constants;
using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityService.Infrastructure.Services
{
    public class JwtTokenService : IJwtTokenService
    {
       
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtTokenService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {                    
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

    
        public string GenerateToken(UserDto user)
        {
            // appsettings.json'dan ayarları alın
            var tokenSettings = _configuration.GetSection("Token");
            var issuer = tokenSettings["Issuer"];
            var audience = tokenSettings["Audience"];
            var key = tokenSettings["Key"];

            // Anahtar ve güvenlik ayarlarını tanımlayın
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var securityKey = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var remoteIpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            var claims = new List<System.Security.Claims.Claim>()
            {
                new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new System.Security.Claims.Claim(CustomClaimTypeConsts.RemoteIpAddress, remoteIpAddress)
            };

            // Token ayarlarını oluşturun
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Subject = new ClaimsIdentity(claims),
                Audience = audience,
                SigningCredentials = credentials,
                Expires = DateTime.UtcNow.AddHours(1) // Token'ın geçerlilik süresi
            };

            // Token'ı oluşturun
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

    
    }
}
