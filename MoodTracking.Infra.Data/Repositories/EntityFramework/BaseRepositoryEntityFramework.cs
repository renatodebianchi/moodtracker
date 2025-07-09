using System; 
using System.Threading.Tasks; 
using Domain.Interfaces.Entities; 
using Microsoft.EntityFrameworkCore; 
using System.Linq; 
using Domain.Extensions; 
using Infra.Data.Contexts; 
namespace Infra.Data.Repositories.EntityFramework 
{ 
    public class BaseRepositoryEntityFramework<TSource> : BaseRepository<TSource> where TSource : class, IGenericEntity 
    { 
        protected readonly ApplicationDbContext _context; 
        public BaseRepositoryEntityFramework(ApplicationDbContext context) 
        { 
            _context = context; 
        } 
        public override async Task<Guid> AddAsync(TSource entity) 
        { 
            try {
                if (entity == null) 
                    throw new ArgumentNullException(nameof(entity), "Entity cannot be null");

                if (entity.Id == Guid.Empty)
                    entity.Id = Guid.NewGuid();

                entity.CreatedAt = DateTime.UtcNow.ToLocalTime(); 
                await _context.AddAsync(entity); 
                await _context.SaveChangesAsync(); 
                return entity.Id; 
            } catch (Exception e) { 
                Console.WriteLine($"Insert record error: {e.Message}"); 
                throw; 
            } 
        } 
        public override async Task<Guid> DeleteAsync(Guid id) 
        { 
            try 
            { 
                var entity = await this.GetByIdAsync(id); 
                if (entity == null) 
                    throw new InvalidDataException($"No record found for Id: {id}"); 
                _context.Remove(entity); 
                await _context.SaveChangesAsync(); 
                return id; 
            } catch (Exception e) { 
                Console.WriteLine($"Delete record error: {e.Message}"); 
                throw; 
            } 
        } 
        public override async Task<IQueryable<TSource>> GetAllAsync() 
        { 
            await Task.Delay(1); 
            return _context.Set<TSource>(); 
        } 
        public override async Task<TSource> GetByIdAsync(Guid id) 
        { 
            return await _context.Set<TSource>().Where(obj => obj.Id == id).FirstOrDefaultAsync(); 
        } 
        public override async Task<Guid> UpdateAsync(TSource entity) 
        { 
            return await this.UpdateAsync(entity, false); 
        } 
        public override async Task<Guid> UpdateAsync(TSource entity, bool force) 
        { 
            try { 
                var updEntity = await this.GetByIdAsync(entity.Id); 
               if (updEntity == null) 
                   throw new InvalidDataException($"No record found for Id: {entity.Id}"); 
               if (!force) 
                   updEntity.SpreadNotNull(entity); 
               else 
                   updEntity.Spread(entity); 
               updEntity.UpdatedAt = DateTime.UtcNow.ToLocalTime(); 
               _context.Update(updEntity); 
               await _context.SaveChangesAsync(); 
               return updEntity.Id;  
            } catch (Exception e) { 
                Console.WriteLine($"Update record error: {e.Message}"); 
                throw; 
            } 
        } 
    } 
}
