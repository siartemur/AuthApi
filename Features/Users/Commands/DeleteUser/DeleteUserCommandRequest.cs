using AuthApi.Features.Users.Commands.DeleteUser;
using MediatR;

public class DeleteUserCommandRequest : IRequest<DeleteUserCommandResponse>
{
    public string? UserId { get; set; } // ❌ Zorunlu Olmayan Hale Getirildi
}
