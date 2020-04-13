using Newtonsoft.Json;

namespace Mapdata.Api.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class CommentRequest
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("message")]
        public string Comments { get; set; }
    }
}
