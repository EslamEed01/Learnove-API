using FluentValidation;
using Learnova.Business.Abstraction.Consts;

namespace Learnova.Business.DTOs.Contract.Users
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {

            RuleFor(x => x.CurrentPassword)
                .NotEmpty();


            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .Matches(RegexPatterns.Password)
                .NotEqual(x => x.CurrentPassword)
                .WithMessage("New password must be different from the current password.");


        }


    }
}
