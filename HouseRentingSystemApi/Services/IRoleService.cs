using System.Collections.Generic;
using System.Threading.Tasks;

namespace HouseRentingSystemApi.Services
{
    public interface IRoleService
    {
        Task EnsureRolesAsync(params string[] roles);
        Task AssignRoleAsync(string userId, string role);
        Task<IList<string>> GetRolesAsync(string userId);
    }
}
