using System.Collections.Generic;
using System.Threading.Tasks;

namespace DALtravelagency.Interfaces
{
    /// <summary>
    /// Базовый generic-репозиторий для простого CRUD.
    /// </summary>
    public interface IBaseStorage<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(long id);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(long id);
    }
}
