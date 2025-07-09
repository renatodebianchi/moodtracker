using Application.Responses; 
using AutoMapper; 
using MediatR; 
using System.Threading; 
using System.Threading.Tasks; 
namespace Application.Handlers 
{ 
    public abstract class BaseRequestHandler<TRequest,TResponse> : IRequestHandler<TRequest, TResponse> 
        where TRequest : IRequest<TResponse> 
        where TResponse : BaseResponse, new() 
    { 
        public BaseRequestHandler() 
        { 
        } 
        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken); 
    } 
} 
