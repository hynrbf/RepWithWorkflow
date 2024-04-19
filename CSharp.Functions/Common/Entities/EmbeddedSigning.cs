namespace Common.Entities
{
    public class EmbeddedSigning
    {
        public string? DocumentId { get; set; }
        public string? FieldInviteId { get; set; }
        public string? Link { get; set; }
        public long? Created { get; set; }
        public long? Updated { get; set; }
        public long? Expiry { get; set; }
        public string? SignedByColleagueEmail { get; set; }
    }
}
