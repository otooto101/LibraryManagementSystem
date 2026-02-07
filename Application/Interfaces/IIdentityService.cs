
using Application.DTOs.Auth;

namespace Application.Interfaces
{
    public interface IIdentityService
    {
        Task<AuthDto> LoginAsync(LoginDto loginDto);
        Task<AuthDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
    }
}
