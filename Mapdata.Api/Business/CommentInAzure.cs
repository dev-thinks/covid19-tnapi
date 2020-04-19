using System.Collections.Generic;
using System.Threading.Tasks;
using Mapdata.Api.Models;
using Service.Infrastructure.AzureTable;

namespace Mapdata.Api.Business
{
    /// <summary>
    /// 
    /// </summary>
    public class CommentInAzure : IComment
    {
        private readonly IAzureTableStorage<CommentRequest> _repository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        public CommentInAzure(IAzureTableStorage<CommentRequest> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<CommentRequest>> GetComments()
        {
            return await _repository.GetList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> SaveComment(CommentRequest model)
        {
            model.PartitionKey = "MapApp";
            model.RowKey = model.Email;
            
            await _repository.Insert(model);

            return true;
        }
    }
}