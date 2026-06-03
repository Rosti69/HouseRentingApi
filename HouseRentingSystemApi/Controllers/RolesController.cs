using HouseRentingSystemApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseRentingSystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;

        public RolesController(RoleManager<IdentityRole> roleManager, IRoleService roleService, IUserService userService)
        {
            _roleManager = roleManager;
            _roleService = roleService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            return Ok(roles);
        }

        public class AssignRoleModel
        {
            public string? UserId { get; set; }
            public string? Role { get; set; }
        }

        [Authorize]
        [HttpPost("assign")]
        public async Task<IActionResult> Assign([FromBody] AssignRoleModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.UserId) || string.IsNullOrWhiteSpace(model.Role))
            {
                return BadRequest();
            }

            await _roleService.AssignRoleAsync(model.UserId, model.Role);
            return Ok(new { model.UserId, model.Role });
        }

        public class CreateRoleModel
        {
            public string? Name { get; set; }
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateRoleModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Name))
            {
                return BadRequest();
            }

            if (!await _roleManager.RoleExistsAsync(model.Name))
            {
                await _roleManager.CreateAsync(new IdentityRole(model.Name));
            }

            return Ok(new { model.Name });
        }

        [HttpGet("user/{userId}/roles")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return BadRequest();
            var roles = await _userService.GetUserRolesAsync(userId);
            return Ok(roles);
        }
    }
}
