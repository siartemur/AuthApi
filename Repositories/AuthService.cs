using AuthApi.Configurations;
using AuthApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace AuthApi.Repositories
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtConfig _jwtConfig;

        public AuthService(UserManager<ApplicationUser> userManager, IOptions<JwtConfig> jwtConfig)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _jwtConfig = jwtConfig?.Value ?? throw new ArgumentNullException(nameof(jwtConfig));
        }

        public async Task<string> GenerateJwtToken(string email)
        {
            // 📌 1️⃣ Email'in `null` olup olmadığını kontrol et
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email), "Email cannot be null or empty");
            }

            // 📌 2️⃣ Kullanıcıyı `FindByEmailAsync` ile bul
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            // 📌 3️⃣ JWT Config `Secret` ve `ExpirationMinutes` değerlerini doğrula
            if (string.IsNullOrEmpty(_jwtConfig?.Secret))
            {
                throw new InvalidOperationException("JWT Secret is missing in configuration");
            }

            if (_jwtConfig == null || _jwtConfig.ExpirationMinutes <= 0)
            {
                throw new InvalidOperationException("Invalid JWT expiration time or config is null");
            }

            // 📌 4️⃣ Kullanıcının Rollerini Al (Yetkilendirme için Gerekli!)
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

            // 📌 5️⃣ JWT Token oluştur
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            claims.AddRange(roleClaims); // 📌 Rolleri JWT'ye ekle

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtConfig.ExpirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
