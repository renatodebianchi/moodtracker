using Application.Requests; 
namespace Application.Requests.Queries { 
    public abstract class BaseQueryRequest<T> : BaseRequest<T> 
    { 
        public BaseQueryRequest(): base() 
        {} 
    } 
} 
