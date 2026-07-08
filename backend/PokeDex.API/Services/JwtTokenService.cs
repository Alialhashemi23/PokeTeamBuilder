using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PokeDex.API.DTOs;
using PokeDex.Data;

namespace PokeDex.API.Services
{
    public interface IJwtTokenService
    {
        AuthResponse CreateToken(AppUser user);
    }

    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public AuthResponse CreateToken(AppUser user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]
                    ?? throw new InvalidOperationException("Jwt:Key is not configured")));

            var lifetimeDays = _configuration.GetValue("Jwt:LifetimeDays", 7);
            var expiresAt = DateTime.UtcNow.AddDays(lifetimeDays);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                claims: new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
                },
                expires: expiresAt,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            return new AuthResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Email = user.Email ?? string.Empty,
                ExpiresAt = expiresAt
            };
        }
    }
}
