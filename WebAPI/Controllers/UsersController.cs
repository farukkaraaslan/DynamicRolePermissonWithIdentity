using Business.Abstract;
using Business.Dto;
using Core.Utilities.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet()]
        public IActionResult GetAll()
        {
            var result = _userService.GetUsersAsync();
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("username")]
        public async Task<IActionResult> GetByUserName([FromQuery(Name = "username")] string userName)
        {
            var result = await _userService.GetByUserNameAsync(userName);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles([FromQuery(Name = "username")] string userName)
        {
            var result = await _userService.GetRoleAsync(userName);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("email")]
        public async Task<IActionResult> GetByEmail([FromQuery(Name = "email")] string email)
        {
            var result = await _userService.GetByEmailAsync(email);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _userService.GetByIdAsync(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto model)
        {
            var result = await _userService.CreateAsync(model, model.Password);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("changePasword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var result = await _userService.ChangePasswordAsync(changePasswordDto);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UserUpdateDto model)
        {
            var result = await _userService.UpdateAsync(model);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpDelete("id")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _userService.DeleteAsync(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
     
    }
}
