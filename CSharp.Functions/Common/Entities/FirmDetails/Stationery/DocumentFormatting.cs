namespace Common.Entities
{
    public class DocumentFormatting
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Font { get; set; }
        public int Size { get; set; }
        public bool IsBold { get; set; }
        public bool IsItalic { get; set; }
        public bool IsUnderline { get; set; }
        public string? Alignment { get; set; }
        public string? TextCase { get; set; }
        public long UpdatedAt { get; set; }
    }
}