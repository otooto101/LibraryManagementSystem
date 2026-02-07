using Application.Extensions;
using FluentValidation;

namespace Application.DTOs.Book.Validators
{
    public class BookCreateDtoValidator : AbstractValidator<BookCreateDto>
    {
        public BookCreateDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

            RuleFor(x => x.AuthorId)
                .NotEmpty().WithMessage("AuthorId is required.");

            RuleFor(x => x.ISBN)
                .NotEmpty()
                .MustBeValidIsbn();

            RuleFor(x => x.PublicationYear)
                .GreaterThanOrEqualTo(-5000).WithMessage("Year must be greater than -5000 (5000 BC).")
                .LessThanOrEqualTo(DateTime.UtcNow.Year).WithMessage($"Publication year cannot be in the future.")
                .Must(y => y != 0).WithMessage("Year 0 does not exist. Use -1 for 1 BC.");

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(1).WithMessage("Quantity must be at least 1.");

            RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.CoverImageUrl)
                .MustBeValidUrl()
                .When(x => !string.IsNullOrEmpty(x.CoverImageUrl));
        }
    }
}
