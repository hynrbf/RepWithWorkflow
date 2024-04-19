using Newtonsoft.Json;

namespace Common.Entities
{
    public class TextField
    {
        [JsonProperty("email")] public string Email { get; set; }
        [JsonProperty("data")] public string Data { get; set; }
    }
}
