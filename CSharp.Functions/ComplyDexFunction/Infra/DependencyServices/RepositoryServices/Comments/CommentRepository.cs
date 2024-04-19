using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Entities;
using Common.Infra;
using Microsoft.Azure.Cosmos;

namespace Api.Infra
{
    public class CommentRepository : RepositoryBase, ICommentRepository
    {
        private const string CommentsContainer = "CommentsContainer";

        private readonly Container _container;

        public CommentRepository() : base(CommentsContainer)
            => _container = Client.GetContainer(DatabaseName, ContainerName);

        public async Task<IEnumerable<Comment>> GetCommentsAsync()
        {
            var query = _container.GetItemQueryIterator<Comment>(
               new QueryDefinition($"SELECT * FROM c"));
            var results = new List<Comment>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<IEnumerable<Comment>> GetCommentsAsync(string contentId)
        {
            if (string.IsNullOrEmpty(contentId))
            {
                throw new NullReferenceException(
                    $"The content id should have a value in {nameof(CommentRepository)}.{nameof(GetCommentsAsync)}");
            }

            var query = _container.GetItemQueryIterator<Comment>(
                new QueryDefinition($"SELECT * FROM c WHERE c.ContentId = '{contentId}'"));
            var results = new List<Comment>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<Comment> AddOrEditCommentAsync(Comment comment)
        {
            var commentModelResponse =
                await _container.UpsertItemAsync(comment, new PartitionKey(comment.Id));
            return commentModelResponse.Resource;
        }

        public async Task<bool> DeleteCommentByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new NullReferenceException(
                    $"The id should have a value in {nameof(CommentRepository)}.{nameof(DeleteCommentByIdAsync)}");
            }

            var itemResponse = await _container.DeleteItemAsync<Comment>(id, new PartitionKey(id));
            return itemResponse.StatusCode == System.Net.HttpStatusCode.NoContent;
        }
    }
}