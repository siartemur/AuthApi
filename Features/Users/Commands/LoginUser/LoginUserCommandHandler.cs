using AuthApi.Models;
using AuthApi.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthApi.Features.Users.Commands.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommandRequest, LoginUserCommandResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthService _authService;

        public LoginUserCommandHandler(UserManager<ApplicationUser> userManager, IAuthService authService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public async Task<LoginUserCommandResponse> Handle(LoginUserCommandRequest request, CancellationToken cancellationToken)
        {
            // 📌 1️⃣ `Email` ve `Password` boş mu kontrol et
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return new LoginUserCommandResponse { Success = false, Message = "Email or Password cannot be empty" };
            }

            // 📌 2️⃣ Kullanıcıyı `FindByEmailAsync` ile bul
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new LoginUserCommandResponse { Success = false, Message = "Invalid email or password" };
            }

            // 📌 3️⃣ Şifreyi kontrol et (`CheckPasswordAsync`)
            bool isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
            {
                return new LoginUserCommandResponse { Success = false, Message = "Invalid email or password" };
            }

            // 📌 4️⃣ JWT Token oluştur
            string token;
            try
            {
                token = await _authService.GenerateJwtToken(user.Email);
                if (string.IsNullOrEmpty(token))
                {
                    return new LoginUserCommandResponse { Success = false, Message = "Failed to generate JWT token" };
                }
            }
            catch (Exception ex)
            {
                return new LoginUserCommandResponse { Success = false, Message = $"JWT Error: {ex.Message}" };
            }

            // ✅ Başarıyla giriş yaptı
            return new LoginUserCommandResponse { Success = true, Token = token, Message = "Login Successful" };
        }
    }
}
