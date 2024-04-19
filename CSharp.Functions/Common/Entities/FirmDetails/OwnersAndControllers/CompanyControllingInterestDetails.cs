namespace Common.Entities
{
    public class CompanyControllingInterestDetails : CompanyDetails
    {
        public double? PercentageOfCapital { get; set; }
        public double? PercentageOfVotingRights { get; set; }
    }
}
