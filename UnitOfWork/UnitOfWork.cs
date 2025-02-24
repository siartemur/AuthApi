using AuthApi.Data;
using AuthApi.Repositories;
using System;
using System.Threading.Tasks;

namespace AuthApi.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IUserRepository Users { get; }

        public UnitOfWork(AppDbContext context, IUserRepository userRepository)
        {
            _context = context;
            Users = userRepository;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
