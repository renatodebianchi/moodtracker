using System; 
using System.Threading.Tasks; 
using Domain.Interfaces.Entities; 
using System.Reflection; 
using Domain.Attributes; 
using System.Linq; 
using Domain.Interfaces.Repositories; 
namespace Infra.Data.Repositories 
{ 
    public abstract class BaseRepository<TSource> : IGenericRepository<TSource> where TSource : IGenericEntity 
    { 
        public abstract Task<Guid> AddAsync(TSource entity); 
        public abstract Task<Guid> DeleteAsync(Guid id); 
        public abstract Task<IQueryable<TSource>> GetAllAsync(); 
        public abstract Task<TSource> GetByIdAsync(Guid id); 
        public abstract Task<Guid> UpdateAsync(TSource entity); 
        public abstract Task<Guid> UpdateAsync(TSource entity, bool force); 
        protected string GetSelectProperties() { 
            string columns = string.Empty; 
            foreach (PropertyInfo sourceProp in typeof(TSource).GetProperties()) 
            { 
                if (UseProperty(sourceProp)) { 
                    PropertyInfo targetProp = typeof(TSource).GetProperty(sourceProp.Name); 
                    try { 
                            columns += columns.Equals(string.Empty) ? "" : ","; 
                            columns += GetColumnName(sourceProp) + " as " + sourceProp.Name; 
                    } catch (Exception e) { 
                        Console.WriteLine("Nao foi possivel processar getSelectProperties: " + e.Message); 
                        return string.Empty; 
                    } 
                } 
            } 
            return columns; 
        } 
        protected string GetAllProperties(bool useAt) { 
            string columns = string.Empty; 
            foreach (PropertyInfo sourceProp in typeof(TSource).GetProperties()) 
            { 
                if (UseProperty(sourceProp)) { 
                    PropertyInfo targetProp = typeof(TSource).GetProperty(sourceProp.Name); 
                    try { 
                        if (!sourceProp.Name.Equals("id")) { 
                            columns += columns.Equals(string.Empty) ? "" : ","; 
                            columns += useAt ? "@" : ""; 
                            columns += useAt ? sourceProp.Name : GetColumnName(sourceProp); 
                        } 
                    } catch (Exception e) { 
                        Console.WriteLine("Nao foi possivel processar getAllProperties: " + e.Message); 
                        return string.Empty; 
                    } 
                } 
            } 
            return columns; 
        } 
        protected string GetSetProperties() { 
            string columns = string.Empty; 
            foreach (PropertyInfo sourceProp in typeof(TSource).GetProperties()) 
            { 
                if (UseProperty(sourceProp)) { 
                    PropertyInfo targetProp = typeof(TSource).GetProperty(sourceProp.Name); 
                    try { 
                        if (!sourceProp.Name.Equals("id")) { 
                            columns += columns.Equals(string.Empty) ? "" : ","; 
                            columns += GetColumnName(sourceProp) + " = @" + sourceProp.Name; 
                        } 
                    } catch (Exception e) { 
                        Console.WriteLine("Nao foi possivel processar getAllProperties: " + e.Message); 
                        return string.Empty; 
                    } 
                } 
            } 
            return columns; 
        } 
        protected bool UseProperty(PropertyInfo prop) { 
            bool use = true; 
            foreach (CustomAttributeData attr in prop.CustomAttributes) 
            { 
                if (attr.AttributeType.Equals(typeof(UseProperty))) 
                { 
                    use = (bool) attr.ConstructorArguments[0].Value; 
                } 
            } 
            return use; 
        } 
        protected string GetTableName(Type type) { 
            string tableName = type.Name; 
            foreach (CustomAttributeData attr in type.CustomAttributes) 
            { 
                if (attr.AttributeType.Equals(typeof(System.ComponentModel.DataAnnotations.Schema.TableAttribute))) 
                { 
                    tableName = attr.ConstructorArguments[0].Value.ToString(); 
                } 
            } 
            return tableName; 
        } 
        protected string GetColumnName(PropertyInfo prop) { 
            string columnName = prop.Name; 
            foreach (CustomAttributeData attr in prop.CustomAttributes) 
            { 
                if (attr.AttributeType.Equals(typeof(System.ComponentModel.DataAnnotations.Schema.ColumnAttribute))) 
                { 
                    columnName = attr.ConstructorArguments[0].Value.ToString(); 
                } 
            } 
            return columnName; 
        } 
    } 
}
