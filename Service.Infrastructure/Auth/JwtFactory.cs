using Microsoft.Extensions.Options;
using Service.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Service.Infrastructure.Auth
{
    internal sealed class JwtFactory : IJwtFactory
    {
        private readonly IJwtTokenHandler _jwtTokenHandler;
        private readonly JwtIssuerOptions _jwtOptions;

        public JwtFactory(IJwtTokenHandler jwtTokenHandler, IOptions<JwtIssuerOptions> jwtOptions)
        {
            _jwtTokenHandler = jwtTokenHandler;
            _jwtOptions = jwtOptions.Value;

            ThrowIfInvalidOptions(_jwtOptions);
        }

        public async Task<JwtAccessToken> GenerateEncodedToken(IEnumerable<JwtTokenClaim> userClaims, string refreshToken)
        {
            var claims = new List<Claim>
            {
                new Claim("rol", "sc_ms_api_access"),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(),
                    ClaimValueTypes.Integer64)
            };

            claims.AddRange(userClaims.Select(userClaim => new Claim(userClaim.Name, userClaim.Value)));

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                _jwtOptions.Issuer,
                //_jwtOptions.Audience,
                null,
                claims.ToArray(),
                _jwtOptions.NotBefore,
                _jwtOptions.Expiration,
                _jwtOptions.SigningCredentials);

            return new JwtAccessToken(_jwtTokenHandler.WriteToken(jwt), (int)_jwtOptions.ValidFor.TotalSeconds, refreshToken);
        }

        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() -
                                 new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                .TotalSeconds);

        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }
    }
}
