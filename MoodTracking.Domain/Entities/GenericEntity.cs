using System; 
using System.ComponentModel.DataAnnotations; 
using Domain.Interfaces.Entities; 
using Domain.Interfaces.Repositories; 
using Domain.Interfaces.Services; 
namespace Domain.Entities 
{ 
    public abstract class GenericEntity : IGenericEntity 
    { 
        [Key] 
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt {get; set;} 
        public DateTime? UpdatedAt {get; set;} 
    } 
} 
