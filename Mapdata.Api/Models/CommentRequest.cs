using Newtonsoft.Json;
using Service.Infrastructure.AzureTable;

namespace Mapdata.Api.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class CommentRequest : AzureTableEntity
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
