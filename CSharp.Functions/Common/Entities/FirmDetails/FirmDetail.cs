namespace Common.Entities
{
    public class FirmDetail: FirmDetailBase
    {
        public string? HeadOfficeAddress { get; set; }
        public bool IsHeadOfficeSameAsTradingAddress { get; set; }
    }
}
