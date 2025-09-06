using FluentValidation;
using Learnova.Business.Abstraction.Consts;

namespace Learnova.Business.DTOs.Contract.Authentication
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .Matches(RegexPatterns.Password)
                .WithMessage("Password should be at least 8 digits and should contains Lowercase, NonAlphanumeric and Uppercase");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .Length(3, 100);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .Length(3, 100);

            RuleFor(x => x.UserName)
                .NotEmpty()
                .Length(3, 100);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .Length(3, 100);

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required.")
                .LessThan(DateTime.Today.AddYears(-13)).WithMessage("You must be at least 13 years old to register.")
                .GreaterThan(DateTime.Today.AddYears(-120)).WithMessage("Invalid date of birth.");

            RuleFor(x => x.Bio)
                .MaximumLength(500).WithMessage("Bio cannot exceed 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Bio));
        }
    }
}
