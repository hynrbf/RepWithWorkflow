namespace Common.Entities;

public class IcoDetails
{
    public string? RegistrationReference { get; set; }
    public long? DateRegistered { get; set; }
    public long? RegistrationExpires { get; set; }
    public string? PaymentTier { get; set; }
    public IcoAddress? Address { get; set; }
    public string? DataController { get; set; }
    public string? OtherNames { get; set; }
    public IcoDataProtectionOfficer? DataProtectionOfficer { get; set; }
}