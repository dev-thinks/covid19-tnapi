using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace Service.Infrastructure.Auth
{
    public class CustomJwtBearerEvents : JwtBearerEvents
    {
        private readonly ITokenFactory _tokenFactory;

        public CustomJwtBearerEvents(ITokenFactory tokenFactory)
        {
            _tokenFactory = tokenFactory;
        }

        public override Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("x-service-token-expired", "true");
            }

            return Task.CompletedTask;
        }

        public override Task TokenValidated(TokenValidatedContext context)
        {
            if (context.SecurityToken is JwtSecurityToken accessToken)
            {
               var result = _tokenFactory.RefreshCacheToken(accessToken.RawData);

               if (!result)
               {
                    context.Fail("UnAuthorized. Session Jwt token not valid in Service store.");
               }
            }

            return Task.CompletedTask;
        }
    }
}
