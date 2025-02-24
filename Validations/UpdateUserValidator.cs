using FluentValidation;
using AuthApi.Features.Users.Commands.UpdateUser;

namespace Authapi.Validations
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserCommandRequest>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.");
            RuleFor(x => x.FullName).NotEmpty().WithMessage("Full Name is required.");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Invalid email format.");
        }
    }
}
