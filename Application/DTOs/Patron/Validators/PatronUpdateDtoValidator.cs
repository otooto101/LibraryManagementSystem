using FluentValidation;

namespace Application.DTOs.Patron.Validators
{
    public class PatronUpdateDtoValidator : AbstractValidator<PatronUpdateDto>
    {
        public PatronUpdateDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name cannot be empty if provided.")
                .MaximumLength(100)
                .When(x => x.FirstName != null);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name cannot be empty if provided.")
                .MaximumLength(100)
                .When(x => x.LastName != null);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email cannot be empty if provided.")
                .EmailAddress().WithMessage("A valid email address is required.")
                .MaximumLength(100)
                .When(x => x.Email != null);
        }
    }
}
