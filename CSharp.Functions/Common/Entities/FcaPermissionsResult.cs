namespace Common.Entities
{
    public class FcaPermissionsResult
    {
        public List<string> PermissionNames { get; set; } = new();
        public string? Raw { get; set; }
    }
}
