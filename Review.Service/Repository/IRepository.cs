using System.Linq.Expressions;

namespace Reviews.Service.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<T> CreateAsync(T entity);
        Task<T> GetAsync(Expression<Func<T, bool>> condition);

        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> condition);
    }
}
