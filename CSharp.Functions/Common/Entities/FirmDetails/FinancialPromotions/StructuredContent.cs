namespace Common.Entities
{
    public class StructuredContent
    {
        public string? ScrapeRunId { get; set; }

        public TextContent? TextContent { get; set; }

        public List<DocumentContent>? Documents { get; set; }

        public List<ImageContent>? Images { get; set; }

        public List<VideoContent>? Videos { get; set; }
    }

    public class ImageContent
    {
        public string? Url { get; set; }
        public int? Height { get; set; }
        public int? Width { get; set; }
        public string? AltText { get; set; }
    }

    public class VideoContent
    {
        public string? Url { get; set; }
        public string? VideoType { get; set; }
    }

    public class TextContent
    {
        public string? Text { get; set; }
    }

    public class DocumentContent
    {
        public string? ContentType { get; set; }
        public string? Url { get; set; }
    }
}
