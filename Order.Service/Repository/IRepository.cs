using System.Linq.Expressions;

namespace Order.Service.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<T> CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<T> GetAsync(Expression<Func<T, bool>> condition);
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T,bool>> condition);
        Task<bool> Contains(int id);
    }
}