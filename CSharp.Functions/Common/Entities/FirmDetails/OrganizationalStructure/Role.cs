namespace Common.Entities
{
    public class Role
    {
        public string? Name { get; set; }
        public string? State { get; set; }
        public bool IsModified { get; set; }
        public bool IsFcaAuthorised { get; set; }
        public bool IsPending { get; set; }
    }
}
