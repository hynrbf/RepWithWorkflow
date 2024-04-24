namespace Common.Entities
{
    public class Provider
    {
        public string? Label { get; set; }
        public string? Value { get; set; }
        public List<Provider>? Items { get; set; }
    }
}