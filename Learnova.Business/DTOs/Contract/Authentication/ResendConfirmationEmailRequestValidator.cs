using FluentValidation;

namespace Learnova.Business.DTOs.Contract.Authentication
{
    public class ResendConfirmationEmailRequestValidator : AbstractValidator<ResendConfirmationEmailRequest>
    {

        public ResendConfirmationEmailRequestValidator()
        {
            RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
        }
    }
}
