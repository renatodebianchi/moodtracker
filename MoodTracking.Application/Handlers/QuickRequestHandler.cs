using Application.Requests; 
using Application.Responses; 
using MediatR; 
namespace Application.Handlers 
{ 
    public abstract class QuickRequestHandler<TRequest> : IRequestHandler<TRequest, BaseResponse> 
        where TRequest : QuickRequest 
    { 
        public QuickRequestHandler() 
        { 
        } 
        public abstract Task<BaseResponse> Handle(TRequest request, CancellationToken cancellationToken); 
    } 
} 
