using AuthApi.Repositories;
using AuthApi.UnitOfWork;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace AuthApi.Features.Users.Commands.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommandRequest, DeleteUserCommandResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<DeleteUserCommandResponse> Handle(DeleteUserCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
                return new DeleteUserCommandResponse { Success = false, Message = "User not found" };

            _userRepository.Delete(user);
            await _unitOfWork.CompleteAsync();

            return new DeleteUserCommandResponse { Success = true, Message = "User deleted successfully" };
        }
    }
}
