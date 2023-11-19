using Business.Abstract;
using Business.Dto;
using Core.Entities.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Authorize(Roles ="user")]
    [Route("api/[controller]")]
    [ApiController]
 
    public class RolesController : ControllerBase
    {
        private IRoleService roleService;

        public RolesController(IRoleService roleService)
        {
            this.roleService = roleService;
        }
        [HttpPost]
        public async Task<IActionResult> Create(RoleRequestDto userRole)
        {
            var result = await roleService.CreateRoleAsync(userRole);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var result = roleService.GetRole();
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest();
        }
    }
}
