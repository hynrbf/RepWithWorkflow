namespace Common.Entities
{
    public class SoleTraderDetails
    {
        public long? DateOfBirthInEpoch
        {
            get
            {
                //DOB can be null if not sole trader
                if (string.IsNullOrEmpty(DateOfBirth))
                {
                    return 0;
                }

                //e.g. save into db '2008-12-24'
                return !DateTime.TryParse(DateOfBirth, out var parsedDate) ? 0 : parsedDate.ConvertToEpoch();
            }
        }

        public string? HomeAddress { get; set; }
        public string? DateOfBirth { get; set; }
        public string? ResidenceDate { get; set; }
    }
}