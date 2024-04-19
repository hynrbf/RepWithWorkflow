namespace Common.Entities
{
    public class AddressInfo
    {
        public string? PreviousHomeAddress { get; set; }
        public bool IsPreviousHomeAddressChanged { get; set; }
        public long? PreviousHomeAddressResidenceDate { get; set; }
    }
}
