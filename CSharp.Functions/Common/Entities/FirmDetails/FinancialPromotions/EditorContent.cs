namespace Common.Entities
{
    /// <summary>
    /// CKEditor properties
    /// </summary>
    public class EditorContent
    {
        public string? Content { get; set; }
        public string? RawContent { get; set; }
        public List<Suggestion> Suggestions { get; set; }
        public List<CommentThread> CommentThreads { get; set; }
    }

    public struct Suggestion
    {
        public string? Id { get; set; }
        public string? Type { get; set; }
        public string? AuthorId { get; set; }
        public long CreatedAt { get; set; }
        public bool HasComments { get; set; }
        public Dictionary<string, object>? Data { get; set; }
        public Dictionary<string, object>? Attributes { get; set; }
    }

    public struct CommentThread
    {
        public string? ThreadId { get; set; }
        public Context Context { get; set; }
        public long ResolvedAt { get; set; }
        public string? ResolvedBy { get; set; }
        public List<EditorComment>? Comments { get; set; }
        public Dictionary<string, object>? Attributes { get; set; }
    }

    public struct EditorComment
    {
        public string? CommentId { get; set; }
        public string AuthorId { get; set; }
        public long CreatedAt { get; set; }
    }

    public struct Context
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}