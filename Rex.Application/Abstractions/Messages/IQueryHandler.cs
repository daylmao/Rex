using MediatR;
using Rex.Application.Utilities;

namespace Rex.Application.Abstractions.Messages;

public interface IQueryHandler<in TQuery, TResponse> : 
    IRequestHandler<TQuery, ResultT<TResponse>>
    where TQuery : IQuery<TResponse>;