using Application.Requests; 
namespace Application.Requests.Commands { 
    public abstract class BaseCommandRequest<T, Model> : BaseRequest<T> 
    { 
        public Model model {get; set;} 
        public BaseCommandRequest(): base() 
        {} 
    } 
} 
