using AuthApi.Models;
using AuthApi.Repositories;
using AuthApi.UnitOfWork;
using MediatR;

namespace AuthApi.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommandRequest, UpdateUserCommandResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<UpdateUserCommandResponse> Handle(UpdateUserCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
                return new UpdateUserCommandResponse { Success = false, Message = "User not found" };

            user.FullName = request.FullName;
            user.Email = request.Email;

            _userRepository.Update(user);
            await _unitOfWork.CompleteAsync();

            return new UpdateUserCommandResponse { Success = true, Message = "User updated successfully" };
        }
    }
}
