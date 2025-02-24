using AuthApi.Models;

namespace AuthApi.Repositories
{
    public interface IUserRepository : IGenericRepository<ApplicationUser>
    {
        Task<ApplicationUser> GetByEmailAsync(string email);
    }
}
