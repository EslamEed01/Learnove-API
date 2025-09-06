using FluentValidation;

namespace Learnova.Business.DTOs.Contract.Users
{
    public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
    {

        public UpdateProfileRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

        }

    }
}
