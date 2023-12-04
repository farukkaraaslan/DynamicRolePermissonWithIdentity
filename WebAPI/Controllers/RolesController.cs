using Business.Abstract;
using Business.Dto;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] RoleRequestDto roleDto)
        {
            var result = await _roleService.CreateRoleAsync(roleDto);

            return result.Success
                ? Ok(result.Message)
                : BadRequest(result.Message);
        }

        [HttpGet()]
        public IActionResult GetRoles()
        {
            var result = _roleService.GetRoles();

            return result.Success
                ? Ok(result)
                : BadRequest(result.Message);
        }
        [HttpPut()]
        public async Task<IActionResult> UpdateRoleClaims([FromBody] RoleUpdateDto roleUpdateDto)
        {
            var updateResult = await _roleService.UpdateRoleClaimsAsync(roleUpdateDto, roleUpdateDto.Claims);

            if (updateResult.Success)
            {
                return Ok(new { Message = "Role claims updated successfully." });
            }

            return BadRequest(new { Error = updateResult.Message });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _roleService.GetByIdAsync(id.ToString());
            return result.Success
            ? Ok(result)
            : BadRequest(result.Message);
        }
    }
}
