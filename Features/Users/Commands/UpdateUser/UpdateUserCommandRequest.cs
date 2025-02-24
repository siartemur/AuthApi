using AuthApi.Features.Users.Commands.UpdateUser;
using MediatR;

public class UpdateUserCommandRequest : IRequest<UpdateUserCommandResponse>
{
    public string? UserId { get; set; } // ❌ Zorunlu Olmayan Hale Getirildi
    public string FullName { get; set; }
    public string Email { get; set; }
}
