namespace PRN232.LMS.Repositories.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(
            string search = null,
            string sort = null,
            int page = 1,
            int pageSize = 10,
            string fields = null,
            string expand = null);

        Task<(IEnumerable<T> data, int totalCount)> GetAllWithCountAsync(
            string search = null,
            string sort = null,
            int page = 1,
            int pageSize = 10,
            string fields = null,
            string expand = null);

        Task<T> GetByIdAsync(int id, string expand = null);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
        Task<bool> SaveAsync();
    }
}
