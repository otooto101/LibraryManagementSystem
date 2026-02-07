using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController(IIdentityService identityService) : ControllerBase
    {
        /// <summary>
        /// Logs in a user (Admin or Librarian) and returns a JWT Token.
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await identityService.LoginAsync(loginDto);
            return Ok(result);
        }

        /// <summary>
        /// Registers a new Librarian. Only an existing Admin can call this.
        /// </summary>
        [HttpPost("register")]
        [Authorize(Roles = Roles.Admin)] // SECURE: Only Admins can create new users
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await identityService.RegisterAsync(registerDto);
            return Ok(result);
        }

        /// <summary>
        /// Refreshes an expired JWT using a valid Refresh Token.
        /// </summary>
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto refreshTokenDto)
        {
            var result = await identityService.RefreshTokenAsync(refreshTokenDto);
            return Ok(result);
        }
    }
}