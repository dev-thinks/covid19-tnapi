using Service.Infrastructure.Models;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace Service.Infrastructure.Auth
{
    internal sealed class TokenFactory : ITokenFactory
    {
        private readonly IConnectionMultiplexer _cache;
        private readonly IJwtTokenValidator _jwtTokenValidator;
        private readonly AppConfigurationOptions _configOptions;

        public TokenFactory(IConnectionMultiplexer cache, IJwtTokenValidator jwtTokenValidator,
            AppConfigurationOptions configOptions)
        {
            _cache = cache;
            _jwtTokenValidator = jwtTokenValidator;
            _configOptions = configOptions;
        }

        public string GenerateToken(int size = 32)
        {
            var randomNumber = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public bool RefreshCacheToken(string token)
        {
            var claimsPrincipal =
                _jwtTokenValidator.GetPrincipalFromToken(token, _configOptions.SecretKey);
            var key = claimsPrincipal.Claims.First(c => c.Type == "id")?.Value;

            IDatabase db = _cache.GetDatabase();
            if (db.KeyExists(key))
            {
                db.KeyExpire(key, new TimeSpan(0, _configOptions.CacheTimeout, 0));
                return true;
            }
            else
            {
                db.StringSet(key, token, TimeSpan.FromMinutes(_configOptions.CacheTimeout));
                return true;
            }
            //return false; --commented temp
        }
    }
}
