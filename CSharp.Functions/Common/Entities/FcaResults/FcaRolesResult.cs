namespace Common.Entities
{
    public class FcaRolesResult
    {
        public string? IndividualRefNo { get; set; }
        public List<FcaIndividualRole> RoleNames { get; set; } = new();
    }

    public class FcaIndividualRole
    {
        public string? Id { get; set; }
        public string? CustomerEngagementMethod { get; set; }
        public string? EndDate { get; set; }
        public string? EffectiveDate { get; set; }
        public string? Restriction { get; set; }
        public string? FirmName { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
        public bool IsCurrent { get; set; }
    }
}
