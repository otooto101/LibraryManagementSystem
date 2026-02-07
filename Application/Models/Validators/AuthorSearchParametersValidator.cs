using FluentValidation;

namespace Application.Models.Validators
{
    public class AuthorSearchParametersValidator : PagedSearchParametersValidator<AuthorSearchParameters>
    {
        public AuthorSearchParametersValidator()
        {
            RuleFor(x => x.FullName)
                .MaximumLength(100).WithMessage("Search name is too long.")
                .When(x => !string.IsNullOrEmpty(x.FullName));

            RuleFor(x => x.SortBy)
                .IsInEnum().WithMessage("Please select a valid sorting option.");
        }
    }
}
