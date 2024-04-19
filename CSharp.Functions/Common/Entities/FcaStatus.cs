﻿using Newtonsoft.Json;

namespace Common.Entities
{
    public class FcaStatus
    {
        [JsonProperty("id")] public string Id { get; set; }
        public string GeneralStatus { get; set; }
        public string ActualStatus { get; set; }
        public string ColorCoding { get; set; }
        public bool IsAuthorised { get; set; }
    }
}
