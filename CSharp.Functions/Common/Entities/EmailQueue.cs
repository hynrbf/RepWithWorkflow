using Newtonsoft.Json;

namespace Common.Entities
{
    public class EmailQueue
    {
        [JsonProperty("id")] public string Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string CustomerName { get; set; }
    }
}