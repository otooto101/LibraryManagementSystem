using FluentValidation;

namespace Application.Models.Validators
{
    public class BookSearchParametersValidator : PagedSearchParametersValidator<BookSearchParameters>
    {
        public BookSearchParametersValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(200)
                .When(x => !string.IsNullOrEmpty(x.Title));

            RuleFor(x => x.Author)
                .MaximumLength(100)
                .When(x => !string.IsNullOrEmpty(x.Author));
        }
    }
}
