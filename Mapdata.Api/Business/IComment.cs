using System.Collections.Generic;
using System.Threading.Tasks;
using Mapdata.Api.Models;

namespace Mapdata.Api.Business
{
    /// <summary>
    /// 
    /// </summary>
    public interface IComment
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<List<CommentRequest>> GetComments();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<bool> SaveComment(CommentRequest model);
    }
}