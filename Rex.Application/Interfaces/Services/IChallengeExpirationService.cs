using Rex.Application.DTOs.JWT;
using Rex.Application.Utilities;

namespace Rex.Application.Interfaces;

public interface IChallengeExpirationService
{
    Task<ResultT<ResponseDto>> MarkChallengeExpired(CancellationToken cancellationToken);
}