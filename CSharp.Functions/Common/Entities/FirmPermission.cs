using Newtonsoft.Json;

namespace Common.Entities
{
    public class BaseFirmPermission
    {
        [JsonProperty("id")] public string Id { get; set; }
        public string? CategoryName { get; set; }
        public string? PermissionName { get; set; }
        public string? PermissionDisplayText { get; set; }
        public bool IsForOthers { get; set; }
    }

    public class FirmPermission : BaseFirmPermission
    {
        public List<string?>? CustomerTypes { get; set; } = new();
        public List<string?>? InvestmentTypes { get; set; } = new();
        public List<string?>? Limitations { get; set; } = new();
    }
}
