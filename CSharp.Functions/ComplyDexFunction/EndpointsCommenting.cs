using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Common.Entities;

namespace Api
{
    public partial class Endpoints
    {
        [FunctionName(nameof(AddOrEditCommentAsync))]
        public async Task<IActionResult> AddOrEditCommentAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var comment = JsonConvert.DeserializeObject<Comment>(requestBody);

            if (string.IsNullOrEmpty(comment.ContentId) || string.IsNullOrEmpty(comment.Email) ||
                string.IsNullOrEmpty(comment.CommentText))
            {
                throw new NullReferenceException("The comment must have the content id, owner email, and its text");
            }

            var response = await _commentRepository.AddOrEditCommentAsync(comment);
            return new OkObjectResult(response);
        }

        [FunctionName(nameof(DeleteCommentByIdAsync))]
        public async Task<bool> DeleteCommentByIdAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var commentId = await new StreamReader(req.Body).ReadToEndAsync();

            if (string.IsNullOrEmpty(commentId))
            {
                throw new NullReferenceException(
                    $"The comment id should have a value in {nameof(Endpoints)}.{nameof(DeleteCommentByIdAsync)}");
            }

            var isDeleted = await _commentRepository.DeleteCommentByIdAsync(commentId);
            return isDeleted;
        }

        [FunctionName(nameof(GetCommentsAsync))]
        public async Task<IActionResult> GetCommentsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var contentId = req.Query["contentId"].ToString();

            if (string.IsNullOrEmpty(contentId))
            {
                throw new NullReferenceException(
                    $"The content id should have a value in {nameof(Endpoints)}.{nameof(GetCommentsAsync)}");
            }

            var comments = await _commentRepository.GetCommentsAsync(contentId);
            return new OkObjectResult(comments);
        }
    }
}