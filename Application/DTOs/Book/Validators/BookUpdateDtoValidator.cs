using Application.Extensions;
using FluentValidation;

namespace Application.DTOs.Book.Validators
{
    public class BookUpdateDtoValidator : AbstractValidator<BookUpdateDto>
    {
        public BookUpdateDtoValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(200)
                .NotEmpty()
                .When(x => x.Title != null);

            RuleFor(x => x.ISBN)!
                .MustBeValidIsbn()
                .When(x => !string.IsNullOrEmpty(x.ISBN));

            RuleFor(x => x.Description)
                .MaximumLength(2000)
                .When(x => x.Description != null);
            
            RuleFor(x => x.CoverImageUrl)
                .MustBeValidUrl()
                .When(x => !string.IsNullOrEmpty(x.CoverImageUrl));
        }
    }
}
