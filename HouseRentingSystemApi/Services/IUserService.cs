using System.Collections.Generic;
using System.Threading.Tasks;

namespace HouseRentingSystemApi.Services
{
    public interface IUserService
    {
        Task AssignRoleAsync(string userId, string role);
        Task<IList<string>> GetUserRolesAsync(string userId);
    }
}
