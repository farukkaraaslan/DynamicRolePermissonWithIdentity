using Business.Abstract;
using Business.Dto.Role;
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

        [HttpGet()]
        public async Task<IActionResult> GetAll()
        {
            var result = await _roleService.GetAll();

            return result.Success
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _roleService.GetByIdAsync(id.ToString());
            return result.Success
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoleRequestDto roleDto)
        {
            var result = await _roleService.CreateAsync(roleDto);

            return result.Success
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] RoleRequestDto roleRequestDto)
        {
            var result = await _roleService.UpdateAsync(id, roleRequestDto);

            return result.Success
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _roleService.DeleteAsync(id);

            return result.Success
                ? Ok(result)
                : BadRequest(result);
        }
    }
}
