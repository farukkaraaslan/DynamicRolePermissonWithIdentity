using Business.Abstract;
using Business.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto model)
        {
            var result = await _userService.CreateAsync(model, model.Password);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest();
        }
        [HttpPost("changePasword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var result = await _userService.ChangePasswordAsync(changePasswordDto);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest();
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UserUpdateDto model)
        {
            var result = await _userService.UpdateAsync(model);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest();
        }
        [HttpGet()]
        public  IActionResult GetAll()
        {
            var result =  _userService.GetUsersAsync();
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest();
        }
        [HttpGet("username")]
        public async Task<IActionResult> GetByUserName([FromQuery(Name = "username")] string userName)
        {
            var resut = await _userService.GetByUserNameAsync(userName);
            if (resut.Success)
            {
                return Ok(resut);
            }
            return BadRequest();
        }
        [HttpGet("email")]
        public async Task<IActionResult> GetByEmail([FromQuery(Name = "email")] string email)
        {
            var resut = await _userService.GetByEmailAsync(email);
            if (resut.Success)
            {
                return Ok(resut);
            }
            return BadRequest();
        }
        [HttpGet("id")]
        public async Task<IActionResult> GetById( string id)
        {
            var resut = await _userService.GetByIdAsync(id);
            if (resut.Success)
            {
                return Ok(resut);
            }
            return BadRequest();
        }

        [HttpDelete("id")]
        public async Task<IActionResult> Delete(string id)
        {
            var resut = await _userService.DeleteAsync(id);
            if (resut.Success)
            {
                return Ok(resut);
            }
            return BadRequest();
        }
    }
}
