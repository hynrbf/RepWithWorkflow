using Newtonsoft.Json;

namespace Common.Entities
{
    public class HtmlContent
    {
        [JsonProperty("id")] public string Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string ContentType { get; set; }
        public string FileUrl { get; set; }
        public string Version { get; set; }
        public string Consultant { get; set; }
    }
}
