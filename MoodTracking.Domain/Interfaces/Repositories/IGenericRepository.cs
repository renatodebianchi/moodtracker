using System.Linq; 
using System.Threading.Tasks; 
using System;
namespace Domain.Interfaces.Repositories 
{ 
    /// <summary>
    /// Interface genérica para repositórios de entidades.
    /// </summary>
    public interface IGenericRepository<T> 
    { 
        Task<T> GetByIdAsync(Guid id); 
        Task<IQueryable<T>> GetAllAsync(); 
        Task<Guid> AddAsync(T entity); 
        Task<Guid> UpdateAsync(T entity); 
        Task<Guid> UpdateAsync(T entity, bool force); 
        Task<Guid> DeleteAsync(Guid id); 
    } 
}
