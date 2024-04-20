
using ClaimIdentityUser.Server.Dtos;

namespace ClaimIdentityUser.Server.Interfaces
{
    public interface ITokenService
    {
         Task<string> GenerateToken(ClaimsDto user);
    }
}
