using Domain.Interfaces.Repositories; 
using System.Threading.Tasks; 
using Domain.Interfaces.Services; 
using System.Linq; 
namespace Application.Services 
{ 
    public abstract class GenericService<TSource> : IGenericService<TSource> 
    { 
        protected IGenericRepository<TSource> Repository {get; private set;} 
        public GenericService(IGenericRepository<TSource> sourceRepository) 
        { 
            Repository = sourceRepository; 
        } 
        public async Task<TSource> GetByIdAsync(Guid id)  
        { 
            var result = await Repository.GetByIdAsync(id); 
            return result; 
        } 
        public async Task<IQueryable<TSource>> GetAllAsync() 
        { 
            var result = await Repository.GetAllAsync(); 
            return result; 
        }
        public async Task<Guid> AddAsync(TSource entity)  
        { 
            return await Repository.AddAsync(entity); 
        } 
        public async Task<Guid> UpdateAsync(TSource entity) 
        { 
            return await Repository.UpdateAsync(entity); 
        } 
        public async Task<Guid> DeleteAsync(Guid id) 
        { 
            return await Repository.DeleteAsync(id); 
        } 
    } 
} 
