using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductPortal.Business.Abstract;
using ProductPortal.Core.Entities.DTOs;
using ProductPortal.Core.Utilities.Security;

namespace ProductPortal.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthController(
            IAuthService authService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserCreateDTO createDto)
        {
            try
            {

                var result = await _authService.RegisterAsync(createDto);
                if (!result.Success)
                    return BadRequest(result);

                var tokenResult = await _authService.CreateAccessTokenAsync(result.Data);
                if (!tokenResult.Success)
                    return BadRequest(tokenResult);

                Response.Cookies.Append("jwt", tokenResult.Data.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(1)
                });

                return Ok(tokenResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration error");
                return StatusCode(500, new { success = false, message = "Registration failed" });
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO loginDto)
        {
            try
            {
                var result = await _authService.LoginAsync(loginDto);
                if (!result.Success)
                    return BadRequest(new { success = false, message = result.Message });

                SetAuthCookie(result.Data.AccessToken.Token);

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        accessToken = result.Data.AccessToken,
                        userId = result.Data.UserId,
                        username = result.Data.Username,
                        role = result.Data.Role
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Login failed" });
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return Ok(new { success = true, message = "Logged out successfully" });
        }

        private void SetAuthCookie(string token)
        {
            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(1)
            });
        }
    }
}