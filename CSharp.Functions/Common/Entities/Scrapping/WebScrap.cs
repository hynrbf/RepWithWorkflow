using Newtonsoft.Json;

namespace Common.Entities
{
    public class WebScrap
    {
        [JsonProperty("id")] public string Id { get; set; } = "";
        public string? ParentId { get; set; }
        public MediaOutletType MediaType { get; set; }
        public string Url { get; set; } = "";
        public List<string> SubUrls { get; set; } = new();
        public string RootUrl { get; set; } = "";
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public List<WebScrapeRun> WebScrapeRuns { get; set; } = new();

        public DateTime? LastScrapeDate { get; set; }
    }

    public class WebScrapeRun
    {
        [JsonProperty("id")] public string Id { get; set; } = "";
        public int Version { get; set; }
        public DateTime ScrapeDate { get; set; }
        public string? HtmlContent { get; set; }
    }
}