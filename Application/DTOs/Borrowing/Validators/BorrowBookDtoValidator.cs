using FluentValidation;

namespace Application.DTOs.Borrowing.Validators
{
    public class BorrowBookDtoValidator : AbstractValidator<BorrowBookDto>
    {
        public BorrowBookDtoValidator()
        {
            RuleFor(x => x.PatronId)
                .NotEmpty()
                .WithMessage("A valid Patron ID is required.");

            RuleFor(x => x.BookId)
                .NotEmpty()
                .WithMessage("A valid Book ID is required.");

            RuleFor(x => x.DaysAllowed)
                .InclusiveBetween(1, 365)
                .WithMessage("Borrowing duration must be between 1 day and 1 year.");
        }
    }
}
