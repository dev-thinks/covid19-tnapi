namespace Service.Infrastructure.Models
{
    /// <summary>
    /// Jwt Access Token
    /// </summary>
    public sealed class UserAccessToken
    {
        /// <summary>
        /// Json web token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Refresh token
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Token expires in
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Remote ip address from where request is initiated
        /// </summary>
        public string RemoteIpAddress { get; set; }

        public string Aoid { get; set; }

        public string Ooid { get; set; }
        public string Iid { get; set; }
        public bool Impersonate { get; set; }

        public UserAccessToken()
        {

        }

        public UserAccessToken(string token, string refreshToken, int expiresIn, string remoteIpAddress, string aoid, string ooid, string iid, bool impersoante)
        {
            Token = token;
            RefreshToken = refreshToken;
            ExpiresIn = expiresIn;
            RemoteIpAddress = remoteIpAddress;
            Aoid = aoid;
            Ooid = ooid;
        }
    }

    /// <summary>
    /// Jwt Access Token
    /// </summary>
    public sealed class JwtAccessToken
    {
        /// <summary>
        /// Json web token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Refresh token
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Token expires in
        /// </summary>
        public int ExpiresIn { get; set; }

        public JwtAccessToken(string token, int expiresIn, string refreshToken)
        {
            Token = token;
            RefreshToken = refreshToken;
            ExpiresIn = expiresIn;
        }

        public JwtAccessToken()
        {
            
        }
    }

    /// <summary>
    /// Jwt user claim
    /// </summary>
    public sealed class JwtTokenClaim
    {
        /// <summary>
        /// Claim name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Claim value
        /// </summary>
        public string Value { get; set; }

        public JwtTokenClaim(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }

    public sealed class JwtRefreshTokenRequest
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
