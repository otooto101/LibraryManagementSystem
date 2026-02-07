using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Validators
{
    public class PagedSearchParametersValidator<T> : AbstractValidator<T> where T : PagedSearchParameters
    {
        public PagedSearchParametersValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page number must be at least 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 50)
                .WithMessage("Page size must be between 1 and 50.");
        }
    }
}
