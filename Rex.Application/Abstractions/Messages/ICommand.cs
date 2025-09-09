using MediatR;
using Rex.Application.Utilities;

namespace Rex.Application.Abstractions.Messages;

public interface ICommand : IRequest<Result>, IBaseCommand;

public interface ICommand<TResponse> : IRequest<ResultT<TResponse>>, IBaseCommand;

public interface IBaseCommand;