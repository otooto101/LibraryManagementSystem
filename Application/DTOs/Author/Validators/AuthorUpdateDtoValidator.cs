using FluentValidation;

namespace Application.DTOs.Author.Validators
{
    public class AuthorUpdateDtoValidator : AbstractValidator<AuthorUpdateDto>
    {
        public AuthorUpdateDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(100)
                .When(x => x.FirstName != null);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(100)
                .When(x => x.LastName != null);

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Date of birth cannot be in the future.")
                .When(x => x.DateOfBirth.HasValue);

            RuleFor(x => x.Biography)
                .MaximumLength(2000)
                .WithMessage("Maximal characters you can use for biography is 2000!")
                .When(x => x.Biography != null);
        }
    }
}
