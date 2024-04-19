using Newtonsoft.Json;

namespace Common.Entities
{
    public abstract class OnboardingUserBase : ChangeInfo
    {
        [JsonProperty("id")] public string? Id { get; set; }

        private string? _tempPassword;

        public string? TempPassword
        {
            get => _tempPassword;
            set
            {
                if (!string.IsNullOrEmpty(_tempPassword))
                {
                    return;
                }

                _tempPassword = value;
            }
        }

        private bool _isUserPasswordSet;

        public bool IsUserPasswordSet
        {
            get => _isUserPasswordSet;
            set
            {
                if (_isUserPasswordSet)
                {
                    return;
                }

                _isUserPasswordSet = value;
            }
        }

        private string? _companyNumber;

        public string? CompanyNumber
        {
            get => _companyNumber;
            set
            {
                if (!string.IsNullOrEmpty(_companyNumber))
                {
                    if (_companyNumber != AppConstants.NotApplicable)
                    {
                        return;
                    }

                    _companyNumber = value;
                    return;
                }

                _companyNumber = value;
            }
        }

        private string? _firmReferenceNumber;

        public string? FirmReferenceNumber
        {
            get => _firmReferenceNumber;
            set
            {
                if (!string.IsNullOrEmpty(_firmReferenceNumber))
                {
                    if (_firmReferenceNumber != AppConstants.NotApplicable)
                    {
                        return;
                    }

                    _firmReferenceNumber = value;
                    return;
                }

                _firmReferenceNumber = value;
            }
        }

        public string? CompanyName { get; set; }
        public string? CompanyAddress { get; set; }
        public bool IsCompanyNotApplicable { get; set; }
        public bool IsAuthorised { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public ContactNumber? ContactNumber { get; set; }
        public bool IsFinishedSignUp { get; set; }
    }
}