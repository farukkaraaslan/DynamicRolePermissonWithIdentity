using Business.Abstract;
using Business.Dto;
using Core.Entities.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
 
    public class RolesController : ControllerBase
    {
        private IRoleService _roleService;

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
                ? Ok(result.Data)
                : BadRequest(result.Message);
        }
    }
}
