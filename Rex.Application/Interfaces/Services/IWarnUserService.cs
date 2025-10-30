using Rex.Application.DTOs.JWT;
using Rex.Application.Utilities;

namespace Rex.Application.Interfaces;

public interface IWarnUserService
{
    Task<ResultT<ResponseDto>> ProcessWarning(CancellationToken cancellationToken);
}