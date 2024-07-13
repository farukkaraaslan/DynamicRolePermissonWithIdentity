using Business.Abstract;
using Business.Dto;
using Core.Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
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

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)

        {
          var userToLogin= await _authService.Login(userLoginDto.UserName,userLoginDto.Password);
         
            if (!userToLogin.Success)
            {
                return BadRequest(userToLogin);
            }
            var result = await _authService.CreateAccessToken(userLoginDto.UserName);
            if (result.Success)
            {
                return Ok(result);
                
            }
           
            
            return BadRequest(result);
        }
    }
}
