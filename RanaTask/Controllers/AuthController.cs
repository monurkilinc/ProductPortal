using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductPortal.Business.Abstract;
using ProductPortal.Core.Entities.DTOs;
using ProductPortal.Core.Utilities.Results;

namespace ProductPortal.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
       private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserRegisterDTO registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);

            if (!result.Success)
                return BadRequest(result);

            // Kayıt başarılıysa token oluştur
            var tokenResult = await _authService.CreateAccessTokenAsync(result.Data);
            if (!tokenResult.Success)
                return BadRequest(tokenResult);

            return Ok(tokenResult);
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO loginDto)  
        {
            var result = await _authService.LoginAsync(loginDto);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
