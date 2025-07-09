using System.Collections.Generic; 
using MediatR; 
namespace Application.Requests { 
    public abstract class BaseRequest<T> : IRequest<T> 
    { 
        public BaseRequest() 
        { 
        } 
        public Dictionary<string, string> Notifications; 
        public abstract bool IsValid(); 
    } 
} 
