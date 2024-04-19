﻿using Newtonsoft.Json;

namespace Common.Entities
{
    public class GetAddressActual
    {
        [JsonProperty("postcode")] public string? Postcode { get; set; }
        [JsonProperty("latitude")] public double Latitude { get; set; }
        [JsonProperty("longitude")] public double Longitude { get; set; }
        [JsonProperty("formatted_address")] public List<string> FormattedAddress { get; set; } = new List<string>();
        [JsonProperty("thoroughfare")] public string? Thoroughfare { get; set; }
        [JsonProperty("building_name")] public string? BuildingName { get; set; }
        [JsonProperty("sub_building_name")] public string? SubBuildingName { get; set; }
        [JsonProperty("sub_building_number")] public string? SubBuildingNumber { get; set; }
        [JsonProperty("building_number")] public string? BuildingNumber { get; set; }
        [JsonProperty("line_1")] public string? Line1 { get; set; }
        [JsonProperty("line_2")] public string? Line2 { get; set; }
        [JsonProperty("line_3")] public string? Line3 { get; set; }
        [JsonProperty("line_4")] public string? Line4 { get; set; }
        [JsonProperty("locality")] public string? Locality { get; set; }
        [JsonProperty("town_or_city")] public string? TownOrCity { get; set; }
        [JsonProperty("county")] public string? County { get; set; }
        [JsonProperty("district")] public string? District { get; set; }
        [JsonProperty("country")] public string? Country { get; set; }
        [JsonProperty("residential")] public bool IsResidential { get; set; }
    }
}
