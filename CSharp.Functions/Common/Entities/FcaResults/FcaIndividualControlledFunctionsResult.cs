using Newtonsoft.Json;

namespace Common.Entities
{
    public class FcaIndividualControlledFunctionsResult : FcaCompanyResultBase
    {
        public List<FcaControlledFunctionsData>? Data { get; set; } = new();
    }

    public class FcaControlledFunctionsData
    {
        [JsonProperty("Current")] public object CurrentControlledFunctions { get; set; } = new();

        [JsonProperty("Previous")] public object PreviousControlledFunctions { get; set; } = new();
    }
}