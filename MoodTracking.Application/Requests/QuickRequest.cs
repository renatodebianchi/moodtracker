using Application.Responses; 
namespace Application.Requests { 
    public abstract class QuickRequest : BaseRequest<BaseResponse> 
    { 
        public QuickRequest(): base() 
        {} 
    } 
} 
