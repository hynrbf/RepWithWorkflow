using Newtonsoft.Json;

namespace Common.Entities
{
    //https://docs.signnow.com/docs/signnow/reference/operations/update-a-document-document-id-adds-fields-to-a-document
    //how do we make font size bigger?
    public class DocumentField
    {
        [JsonProperty("x")] public int X { get; set; }
        [JsonProperty("y")] public int Y { get; set; }
        [JsonProperty("width")] public int Width { get; set; }
        [JsonProperty("height")] public int Height { get; set; }
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("label")] public string Label { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("role")] public string Role { get; set; }
        [JsonProperty("page_number")] public int PageNumber { get; set; }
        [JsonProperty("required")] public bool IsRequired { get; set; }
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("element_id")] public string ElementId { get; set; }
        [JsonProperty("prefilled_text")] public string? Value { get; set; }
        [JsonProperty("font")] public string Font { get; set; }
        [JsonProperty("size")] public int FontSize { get; set; }
        [JsonProperty("max_chars")] public int MaxChars { get; set; }
        [JsonProperty("validator_id")] public string? ValidatorId { get; set; }
    }
}