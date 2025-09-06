using Learnova.Business.Abstraction;
using Learnova.Business.DTOs.Contract.Roles;
using Learnova.Business.Services.Interfaces;
using Learnova.Infrastructure.Data;
using Learnova.Infrastructure.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learnova.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController(IRoleService roleService) : ControllerBase
    {
        private readonly IRoleService _roleService = roleService;

        /// <summary>
        /// Get all roles
        /// </summary>
        [HttpGet("")]
        [HasPermission(Permissions.GetRoles)]
        public async Task<IActionResult> GetAll([FromQuery] bool includeDisabled, CancellationToken cancellationToken)
        {
            var roles = await _roleService.GetAllAsync(includeDisabled, cancellationToken);
            return Ok(roles);
        }

        /// <summary>
        /// Get a specific role by id
        /// </summary>
        [HttpGet("{id}")]
        [HasPermission(Permissions.GetRoles)]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            var result = await _roleService.GetAsync(id);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        /// <summary>
        /// Create a new role
        /// </summary>
        [HttpPost("")]
        [HasPermission(Permissions.AddRoles)]
        public async Task<IActionResult> Add([FromBody] RoleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _roleService.AddAsync(request);
            return result.IsSuccess ? CreatedAtAction(nameof(Get), new { result.Value.Id }, result.Value) : result.ToProblem();
        }

        /// <summary>
        /// Update a specific role by id
        /// </summary>
        [HttpPut("{id}")]
        [HasPermission(Permissions.UpdateRoles)]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] RoleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _roleService.UpdateAsync(id, request);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        /// <summary>
        /// Toggle status of a specific role by id
        /// </summary>
        [HttpPut("{id}/toggle-status")]
        [HasPermission(Permissions.UpdateRoles)]
        public async Task<IActionResult> ToggleStatus([FromRoute] string id)
        {
            var result = await _roleService.ToggleStatusAsync(id);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        /// <summary>
        /// Get all available permissions
        /// </summary>
        [HttpGet("permissions")]
        [HasPermission(Permissions.GetRoles)]
        public IActionResult GetAllPermissions()
        {
            var permissions = Permissions.GetAllPermissions().Where(p => !string.IsNullOrEmpty(p));
            return Ok(permissions);
        }
    }
}
