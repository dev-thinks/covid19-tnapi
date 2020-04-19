using System.Collections.Generic;
using Mapdata.Api.DbContexts;
using Mapdata.Api.Entities;
using Mapdata.Api.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Mapdata.Api.Business
{
    /// <summary>
    /// 
    /// </summary>
    public class CommentBusiness : IComment
    {
        private readonly TnDistrictContext _context;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public CommentBusiness(TnDistrictContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<CommentRequest>> GetComments()
        {
            var result = new List<CommentRequest>();
            
            var commentData = await _context.Comment.ToListAsync();
            
            commentData.ForEach(s =>
            {
                result.Add(new CommentRequest
                {
                    Name = s.Name,
                    Email = s.Email,
                    Comments = s.Feedback
                });
            });
            
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> SaveComment(CommentRequest model)
        {
            var comment = new Comment
            {
                Name = model.Name,
                Email = model.Email,
                Feedback = model.Comments
            };

            _context.Comment.Add(comment);

            var returnRows = await _context.SaveChangesAsync();

            return returnRows > 0;
        }
    }
}
