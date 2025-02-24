using System.Linq.Expressions;

namespace AuthApi.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(string id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
