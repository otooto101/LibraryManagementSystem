using FluentValidation;

namespace Application.Extensions
{
    public static class ValidatorExtensions
    {
        private const string IsbnRegex = @"^(?=(?:\D*\d){10}(?:(?:\D*\d){3})?$)[\d-]+$";

        public static IRuleBuilderOptions<T, string?> MustBeValidUrl<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .Must(url =>
                    string.IsNullOrEmpty(url) ||
                    (Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
                     (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                )
                .WithMessage("The field '{PropertyName}' must be a valid HTTP or HTTPS URL.");
        }

        public static IRuleBuilderOptions<T, string?> MustBeValidIsbn<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .Matches(IsbnRegex)
                .WithMessage("The provided ISBN is not in a valid format (ISBN-10 or ISBN-13 allowed).");
        }
    }
}
