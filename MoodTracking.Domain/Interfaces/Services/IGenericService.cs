using System.Linq; 
using System.Threading.Tasks; 
using Domain.Interfaces.Repositories; 
namespace Domain.Interfaces.Services 
{ 
    public interface IGenericService<T> 
    { 
        Task<T> GetByIdAsync(Guid id); 
        Task<IQueryable<T>> GetAllAsync(); 
        Task<Guid> AddAsync(T entity); 
        Task<Guid> UpdateAsync(T entity); 
        Task<Guid> DeleteAsync(Guid id); 
    } 
} 
