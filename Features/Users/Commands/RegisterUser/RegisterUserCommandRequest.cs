using MediatR;

namespace AuthApi.Features.Users.Commands.RegisterUser
{
    public class RegisterUserCommandRequest : IRequest<RegisterUserCommandResponse>
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
