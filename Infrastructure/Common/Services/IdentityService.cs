using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Constants;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Persistence.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Common.Services
{
    public class IdentityService(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration) : IIdentityService
    {
        public async Task<AuthDto> LoginAsync(LoginDto loginDto)
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            return await GenerateAuthResponse(user);
        }

        public async Task<AuthDto> RegisterAsync(RegisterDto registerDto)
        {
            var userExists = await userManager.FindByEmailAsync(registerDto.Email);
            if (userExists != null)
            {
                throw new DuplicateResourceException($"User with email {registerDto.Email} already exists.", null!);
            }

            var user = new ApplicationUser
            {
                Email = registerDto.Email,
                UserName = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new DomainRuleViolationException($"Registration failed: {errors}", null!);
            }

            await userManager.AddToRoleAsync(user, Roles.Librarian);

            return await GenerateAuthResponse(user);
        }

        public async Task<AuthDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            var principal = GetPrincipalFromExpiredToken(refreshTokenDto.Token);
            var email = principal.FindFirstValue(ClaimTypes.Email);

            var user = await userManager.FindByEmailAsync(email!);

            if (user == null || user.RefreshToken != refreshTokenDto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token");
            }

            return await GenerateAuthResponse(user);
        }

        private async Task<AuthDto> GenerateAuthResponse(ApplicationUser user)
        {
            var token = await GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Refresh token lasts 7 days

            await userManager.UpdateAsync(user);

            return new AuthDto(user.Id, user.Email!, token, refreshToken);
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var userRoles = await userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(configuration["Jwt:ExpiryInMinutes"]!)),
                SigningCredentials = creds,
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                ValidateLifetime = false // Here we don't care if the token is expired
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}