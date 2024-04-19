namespace Common.Entities
{
    public class DataCollectionMethod
    {
        public string? Title { get; set; }

        public string? Text { get; set; }
    }

    public class DataCollectionPurpose
    {
        public string? Text { get; set; }
    }

    public class DataCollectionTiming
    {
        public string? Text { get; set; }
    }

    public class PersonalDataCategory
    {
        public string? Title { get; set; }

        public List<string>? Details { get; set; }
    }
}