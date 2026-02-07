using FluentValidation;


namespace Application.DTOs.Auth.Validators
{
    public class RefreshTokenDtoValidator : AbstractValidator<RefreshTokenDto>
    {
        public RefreshTokenDtoValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("The expired JWT token is required.");

            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("The refresh token is required.");
        }
    }
}
