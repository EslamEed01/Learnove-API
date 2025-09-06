using FluentValidation;
using Learnova.Business.Abstraction.Consts;

namespace Learnova.Business.DTOs.Contract.Users
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {

        public UpdateUserRequestValidator()
        {

            RuleFor(x => x.firstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

            RuleFor(x => x.lastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

            RuleFor(x => x.email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.password)
                .NotEmpty().WithMessage("Password is required.")
                .Matches(RegexPatterns.Password)
                .WithMessage("Password should be at least 8 digits and should contains Lowercase, NonAlphanumeric and Uppercase");

            RuleFor(x => x.Roles)
           .NotNull()
           .NotEmpty();

            RuleFor(x => x.Roles)
               .Must(x => x.Distinct().Count() == x.Count)
               .WithMessage("You cannot add duplicated role for the same user")
               .When(x => x.Roles != null);

        }


    }
}
