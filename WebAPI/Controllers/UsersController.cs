using Business.Abstract;
using Business.Dto;
using Microsoft.AspNetCore.Http;
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

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto model)
        {
            var result = await _userService.Add(model, model.Password);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest();
        }
        [HttpGet]
        public  IActionResult GetAll()
        {
            var result =  _userService.GetUsers();
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest();
        }
    }
}
