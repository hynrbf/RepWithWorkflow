namespace Common.Entities
{
    public class FileModel
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public int Size { get; set; }
        public string? Extension { get; set; }
        public string? Url { get; set; }
        public long CreatedAt { get; set; }
    }
}
