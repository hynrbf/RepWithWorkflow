namespace Common.Entities
{
    public class MediaMarketingOutlet
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public bool Archived { get; set; }
        public string Platform { get; set; }
        public long CreatedAt { get; set; } = DateHelper.GetCurrentDateTimeInEpoch();
    }
}