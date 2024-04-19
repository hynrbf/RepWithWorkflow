using Newtonsoft.Json;

namespace Common.Entities
{
    [Obsolete]
    //should be no postfix Model
    public abstract class SchemaModelBase
    {
        [JsonProperty("id")] public string Id { get; set; }

        //properties used only for strong typing
        public string Page { get; set; }
        public string FormNameKey { get; set; }
    }
}