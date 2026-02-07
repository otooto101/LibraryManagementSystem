using FluentValidation;
using FluentValidation.Validators;

namespace Application.DTOs.Patron.Validators
{
    public class PatronCreateDtoValidator : AbstractValidator<PatronCreateDto>
    {
        public PatronCreateDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(100).WithMessage("First name cannot exceed 100 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress(EmailValidationMode.AspNetCoreCompatible)
                .WithMessage("A valid email address is required.")
                .MaximumLength(100);
        }
    }
}
