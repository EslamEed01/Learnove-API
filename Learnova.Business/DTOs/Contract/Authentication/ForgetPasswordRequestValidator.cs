using FluentValidation;

namespace Learnova.Business.DTOs.Contract.Authentication
{
    public class ForgetPasswordRequestValidator : AbstractValidator<ForgetPasswordRequest>
    {
        public ForgetPasswordRequestValidator()
        {
            RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
        }

    }
}
