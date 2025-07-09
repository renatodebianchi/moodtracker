using Domain.Entities; 
using Microsoft.EntityFrameworkCore; 
using Microsoft.EntityFrameworkCore.Metadata.Builders; 
namespace Infra.Data.EntitiesConfiguration 
{ 
    public abstract class GenericEntityConfiguration<TSource> : IEntityTypeConfiguration<TSource> where TSource : GenericEntity 
    { 
        public virtual void Configure(EntityTypeBuilder<TSource> builder) 
        { 
            builder.HasKey(reference => reference.Id); 
        } 
    } 
} 
