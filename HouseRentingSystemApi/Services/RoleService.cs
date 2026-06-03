using HouseRentingSystemApi.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseRentingSystemApi.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<RoleService> _logger;

        public RoleService(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, ILogger<RoleService> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task EnsureRolesAsync(params string[] roles)
        {
            foreach (var role in roles.Where(r => !string.IsNullOrWhiteSpace(r)))
            {
                if (await _roleManager.RoleExistsAsync(role) == false)
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(role));
                    if (!result.Succeeded)
                    {
                        _logger.LogWarning("Failed to create role {Role}: {Errors}", role, string.Join(',', result.Errors.Select(e => e.Description)));
                    }
                }
            }
        }

        public async Task AssignRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return;
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
            await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<IList<string>> GetRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new List<string>();
            return await _userManager.GetRolesAsync(user);
        }
    }
}
