namespace Image.Grpc.Service.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<T> CreateAsync(T entity);
    }
}
