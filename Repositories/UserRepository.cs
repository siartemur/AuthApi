using AuthApi.Data;
using AuthApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Repositories
{
    public class UserRepository : GenericRepository<ApplicationUser>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<ApplicationUser> GetByEmailAsync(string email) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
}
