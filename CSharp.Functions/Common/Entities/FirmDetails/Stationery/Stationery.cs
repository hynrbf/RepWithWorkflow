namespace Common.Entities
{
    public class Stationery
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Icon { get; set; }
        public ApprovalStatus Status { get; set; }
        public List<FileModel>? Files { get; set; }
        public long CreatedAt { get; set; }
        public long UpdatedAt { get; set; }
    }
}