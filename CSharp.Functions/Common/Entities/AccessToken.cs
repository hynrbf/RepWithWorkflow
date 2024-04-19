using Newtonsoft.Json;

namespace Common.Entities
{
    public class AccessToken
    {
        [JsonProperty("access_token")] public string Value { get; set; }
        [JsonProperty("id_token")] public string IdToken { get; set; }
    }
}
