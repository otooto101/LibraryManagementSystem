using FluentValidation;

namespace Application.DTOs.Author.Validators
{
    public class AuthorCreateDtoValidator : AbstractValidator<AuthorCreateDto>
    {
        public AuthorCreateDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.DateOfBirth)
                .NotEmpty()
                .LessThan(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Date of birth cannot be in the future.");

            RuleFor(x => x.Biography)
                .MaximumLength(2000);
        }
    }
}
