using AutoMapper;
using Common.Entities;
using System.Diagnostics;
using System.Text;

namespace Common.Infra
{
    public class ServiceMapper : IServiceMapper
    {
        public IMapper Instance
        {
            get
            {
                Debug.WriteLine("AutoMapper in Action.");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<FcaCompanyK, Company>()
                        .ForMember(dst => dst.CompanyName,
                            src => src.MapFrom(source => ExtractCompanyName(source.Name)))
                        .ForMember(dst => dst.FirmReferenceNo,
                            src => src.MapFrom(source => ComputeFirmReferenceNumber(source.ReferenceNo)))
                        .ForMember(dst => dst.Status, src => src.MapFrom(source => ComputeActualStatus(source.Status)))
                        .ForMember(dst => dst.Type, src => src.MapFrom(source => source.TypeOfBusiness))
                        .ForMember(dst => dst.Postcode, src => src.MapFrom(source => ExtractPostCode(source.Name)))
                        .ForMember(dst => dst.Address, src => src.MapFrom(source => source.CompanyAddress))
                        .ForMember(dst => dst.CompanyNumber, src => src.MapFrom(source => AppConstants.NotApplicable));

                    cfg.CreateMap<CompaniesHouseCompanyK, Company>()
                        .ForMember(dst => dst.CompanyName, src => src.MapFrom(source => source.Title))
                        .ForMember(dst => dst.CompanyNumber,
                            src => src.MapFrom(source => source.CompanyNumber ?? AppConstants.NotApplicable))
                        .ForMember(dst => dst.Postcode, src => src.MapFrom(source => source.AddressK.PostalCode))
                        .ForMember(dst => dst.Status, src => src.MapFrom(source => source.CompanyStatus))
                        .ForMember(dst => dst.Address, src => src.MapFrom(source => source.AddressSnippet))
                        .ForMember(dst => dst.Region, opt =>
                        {
                            opt.Condition(src => !string.IsNullOrEmpty(src.AddressK?.Region));
                            opt.MapFrom(source => source.AddressK.Region);
                        })
                        .ForMember(dst => dst.CompanyStatus, src => src.MapFrom(source => source.CompanyStatus))
                        .ForMember(dst => dst.FirmReferenceNo,
                            src => src.MapFrom(source => AppConstants.NotApplicable)); //this will get into FCA

                    cfg.CreateMap<NameElement, IndividualControllerDetails>()
                        .ForMember(dst => dst.Surname,
                            opt => opt.MapFrom(src => Helpers.CapitalizeOnlyFirstLetterOfWord(src.Surname ?? "")));

                    cfg.CreateMap<Controller, IndividualController>()
                        .ForMember(dst => dst.Detail,
                            src => src.MapFrom(source => GenerateIndividualControllerDetails(source)))
                        .ForMember(dst => dst.DirectorsAndDirectorship,
                            src => src.MapFrom(source => new List<CompanyOfficerAppointmentDetails>
                                { source.Directorships }));

                    cfg.CreateMap<Controller, CorporateController>()
                        .ForMember(dst => dst.CompanyName, src => src.MapFrom(source => source.Name))
                        .ForMember(dst => dst.RegisteredAddress, src => src.MapFrom(source => source.FullAddress))
                        .ForMember(dst => dst.CorporateOwners,
                            src => src.MapFrom(source => source.CorporateControllers.Count))
                        .ForMember(dst => dst.IndividualOwners,
                            src => src.MapFrom(source => source.IndividualControllers.Count))
                        .AddTransform<string>(s => string.IsNullOrEmpty(s) ? "" : s);

                    cfg.CreateMap<CompanyOfficer, Director>();

                    cfg.CreateMap<FcaAppointedRepresentative, AppointedRepresentative>()
                        .ForMember(dst => dst.FcaFirmRefNo, src => src.MapFrom(source => source.Frn))
                        .ForMember(dst => dst.Status, src => src.MapFrom(source => source.IsCurrentRepresentative 
                            ? ArStatus.Onboarding : ArStatus.Resigned));

                    cfg.CreateMap<IcoDetails, DataProtectionLicense>()
                        .ForMember(dst => dst.LicenseNumber, src => src.MapFrom(source => source.RegistrationReference))
                        .ForMember(dst => dst.EndDate, src => src.MapFrom(source => source.RegistrationExpires));

                    cfg.CreateMap<IcoDataProtectionOfficer, DataProtectionOfficer>()
                        .ForMember(dst => dst.Forename, src => src.MapFrom(source => source.Name));
                });

                return config.CreateMapper();
            }
        }

        #region COMPANY

        private static string ExtractCompanyName(string companyName)
        {
            if (string.IsNullOrEmpty(companyName))
            {
                return AppConstants.NotApplicable;
            }

            var firstIndexOfPostCode = companyName.IndexOf("(Post", StringComparison.Ordinal);
            return firstIndexOfPostCode == -1 ? companyName : companyName[..firstIndexOfPostCode];
        }

        private static string ComputeFirmReferenceNumber(string referenceNumber)
        {
            if (string.IsNullOrEmpty(referenceNumber) || referenceNumber.Trim().Length == 0)
            {
                return AppConstants.NotApplicable;
            }

            return referenceNumber;
        }

        private static string ComputeActualStatus(string actualStatus)
        {
            if (string.IsNullOrEmpty(actualStatus) || actualStatus.Trim().Length == 0)
            {
                return "Not Authorised";
            }

            return actualStatus;
        }

        private static string ExtractPostCode(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return "Not Available";
            }

            const string postcodeKeyWord = "Postcode: ";
            var hasPostCodeInName = name.Contains(postcodeKeyWord);

            if (!hasPostCodeInName)
            {
                return "Not Available";
            }

            var postCodePart = name[name.IndexOf(postcodeKeyWord, StringComparison.Ordinal)..];

            if (string.IsNullOrEmpty(postCodePart))
            {
                return "Not Available";
            }

            var postCodeLength = postCodePart.Length - postcodeKeyWord.Length - 1;
            return postCodePart.Substring(postcodeKeyWord.Length, postCodeLength);
        }

        private static IndividualControllerDetails GenerateIndividualControllerDetails(Controller controller)
        {
            var detail = new IndividualControllerDetails
            {
                Title = controller.NameElements?.Title,
                Forename = controller.NameElements?.Forename,
                Surname = controller.NameElements?.Surname,
                HomeAddress = BuildHomeAddress(controller.Address),
                PercentageOfCapital = controller.PercentageOfCapital,
                PercentageOfVotingRights = controller.PercentageOfVotingRights,
                DateOfBirth = controller.DateOfBirth,
                Nationalities = string.IsNullOrEmpty(controller.Nationality)
                    ? new List<string>()
                    : new List<string> { controller.Nationality }
            };
            
            return detail;
        }

        private static string BuildHomeAddress(Address? address)
        {
            if (address == null)
            {
                return "";
            }

            var fullAddress = new StringBuilder();

            if (!string.IsNullOrEmpty(address.Premises))
            {
                fullAddress.Append(address.Premises);
            }

            if (!string.IsNullOrEmpty(address.AddressLine1))
            {
                fullAddress.Append($", {address.AddressLine1}");
            }

            if (!string.IsNullOrEmpty(address.AddressLine2))
            {
                fullAddress.Append($", {address.AddressLine2}");
            }

            if (!string.IsNullOrEmpty(address.AddressLine3))
            {
                fullAddress.Append($", {address.AddressLine3}");
            }

            if (!string.IsNullOrEmpty(address.AddressLine4))
            {
                fullAddress.Append($", {address.AddressLine4}");
            }

            if (!string.IsNullOrEmpty(address.Locality))
            {
                fullAddress.Append($", {address.Locality}");
            }

            if (!string.IsNullOrEmpty(address.Region))
            {
                fullAddress.Append($", {address.Region}");
            }

            if (!string.IsNullOrEmpty(address.Country))
            {
                fullAddress.Append($", {address.Country}");
            }

            if (!string.IsNullOrEmpty(address.PostalCode))
            {
                fullAddress.Append($", {address.PostalCode}");
            }

            return fullAddress.ToString();
        }

        #endregion
    }
}