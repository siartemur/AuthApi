using FluentValidation;
using AuthApi.Features.Users.Commands.DeleteUser;

namespace AuthApi.Validations
{
    public class DeleteUserValidator : AbstractValidator<DeleteUserCommandRequest>
    {
        public DeleteUserValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.");
        }
    }
}
