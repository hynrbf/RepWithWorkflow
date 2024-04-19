using Newtonsoft.Json;

namespace Common.Entities
{
    public class Comment
    {
        [JsonProperty("id")] public string Id { get; set; }
        public string? ParentId { get; set; }
        public string? ContentId { get; set; }
        public string? Content { get; set; }
        public string? ContentType { get; set; }
        public long CreatedAt { get; set; }
        public string? Email { get; set; }
        public string? CommentText { get; set; }
        public string? PostId { get; set; }
        public string? PostType { get; set; }
        public string? CommentType { get; set; }
        public bool IsPublic { get; set; }
    }
}
