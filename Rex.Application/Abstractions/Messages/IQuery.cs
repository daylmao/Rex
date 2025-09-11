using MediatR;
using Rex.Application.Utilities;

namespace Rex.Application.Abstractions.Messages;

public interface IQuery<TResponse>: IRequest<ResultT<TResponse>>;