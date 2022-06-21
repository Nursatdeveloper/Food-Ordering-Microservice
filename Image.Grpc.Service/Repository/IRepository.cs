using System.Linq.Expressions;

namespace Image.Grpc.Service.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<T> CreateAsync(T entity);
        Task<T> GetAsync(Expression<Func<T, bool>> condition);
    }
}
