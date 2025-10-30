using Rex.Application.DTOs.JWT;
using Rex.Application.Utilities;

namespace Rex.Application.Interfaces;

public interface IRemoveUserService
{
    Task<ResultT<ResponseDto>> ProcessRemoval(CancellationToken cancellationToken);

}