using Mapdata.Api.DbContexts;
using Mapdata.Api.Entities;
using Mapdata.Api.Models;
using System.Threading.Tasks;

namespace Mapdata.Api.Business
{
    /// <summary>
    /// 
    /// </summary>
    public class CommentBusiness
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
