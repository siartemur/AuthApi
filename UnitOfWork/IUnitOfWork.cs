using System;
using System.Threading.Tasks;
using AuthApi.Repositories;

namespace AuthApi.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        Task<int> CompleteAsync();
    }
}
