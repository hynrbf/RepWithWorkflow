namespace Common.Entities
{
    public abstract class ChangeInfo
    {
        public string? ChangedBy { get; set; }
        public long ChangedOn { get; set; }
        public string? IpAddress { get; set; }
    }

    public enum ChangeSource
    {
        ComplyDexWeb,
        JobSignUp,
        JobDataInitializer
    }
}