using Newtonsoft.Json;

namespace Common.Entities
{
    public class FcaPermission
    {
        [JsonProperty("id")] public string Id { get; set; }
        public string PermissionGroupName { get; set; }
        public string CategoryName { get; set; }
        public string SubPermissionName { get; set; }
        public string SubPermissionDisplayText { get; set; }
    }
}
