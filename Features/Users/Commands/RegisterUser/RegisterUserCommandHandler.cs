using AuthApi.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace AuthApi.Features.Users.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommandRequest, RegisterUserCommandResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RegisterUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<RegisterUserCommandResponse> Handle(RegisterUserCommandRequest request, CancellationToken cancellationToken)
        {
            var user = new ApplicationUser { FullName = request.FullName, Email = request.Email, UserName = request.Email };
            var result = await _userManager.CreateAsync(user, request.Password);

            return new RegisterUserCommandResponse { Success = result.Succeeded, Message = result.Succeeded ? "User Created" : "Failed to Create User" };
        }
    }
}
