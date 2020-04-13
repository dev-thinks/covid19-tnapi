using Microsoft.IdentityModel.Tokens;
using Service.Infrastructure.Models;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Service.Infrastructure.Auth
{
    public interface IJwtFactory
    {
        Task<JwtAccessToken> GenerateEncodedToken(IEnumerable<JwtTokenClaim> userClaims, string refreshToken);
    }

    public interface IJwtTokenHandler
    {
        string WriteToken(JwtSecurityToken jwt);

        ClaimsPrincipal ValidateToken(string token, TokenValidationParameters tokenValidationParameters);
    }

    public interface IJwtTokenValidator
    {
        ClaimsPrincipal GetPrincipalFromToken(string token, string signingKey);

        bool IsValidTokenForRefresh(ClaimsPrincipal claims);
    }

    public interface ITokenFactory
    {
        string GenerateToken(int size = 32);

        bool RefreshCacheToken(string token);
    }
}
