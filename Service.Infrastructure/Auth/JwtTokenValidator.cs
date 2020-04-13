using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Service.Infrastructure.Auth
{
    internal sealed class JwtTokenValidator : IJwtTokenValidator
    {
        private readonly IJwtTokenHandler _jwtTokenHandler;

        public JwtTokenValidator(IJwtTokenHandler jwtTokenHandler)
        {
            _jwtTokenHandler = jwtTokenHandler;
        }

        public ClaimsPrincipal GetPrincipalFromToken(string token, string signingKey)
        {
            return _jwtTokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                ValidateLifetime = false // we check expired tokens here
            });
        }

        public bool IsValidTokenForRefresh(ClaimsPrincipal claims)
        {
            var policyClaim = claims.Claims.First(c => c.Type == "rol");

            return policyClaim.Value == "sc_ms_api_access";
        }
    }
}
