namespace Common.Entities
{
    public class ContactNumber
    {
        public string? DialCode { get; set; }
        public string? CountryCode { get; set; }
        public string? Country { get; set; }

        private string? _number;
        public string? Number
        {
            get => _number;
            set
            {
                //This is a dirty hack because the front end is sending underscrore and component is hard to change
                _number = value?.Replace("_", "");
            }
        }
    }
}
