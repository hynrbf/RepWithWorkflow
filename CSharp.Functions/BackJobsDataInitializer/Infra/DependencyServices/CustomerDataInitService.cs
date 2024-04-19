using Common;
using Common.Entities;
using Common.Infra;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackJobsDataInitializer.Infra
{
    public class CustomerDataInitService : ICustomerDataInitService
    {
        private readonly IFcaService _fcaService;
        private readonly ICompaniesHouseService _companiesHouseService;
        private readonly IServiceMapper _serviceMapper;
        private readonly IIcoService _icoService;
        private readonly IWebScrapeService _webScrapeService;
        private readonly IFcaPermissionsRepository _fcaPermissionsRepository;
        private readonly IFcaRoleRepository _fcaRoleRepository;
        private readonly ILogger<CustomerDataInitService> _logger;

        private string _azureStorageBaseUrl;
        private string _azureStorageConnectionString;
        private string _blobStorageContainerName;

        public CustomerDataInitService(IFcaService fcaService,
            ICompaniesHouseService companiesHouseService,
            IIcoService icoService,
            IWebScrapeService webScrapeService,
            IFcaPermissionsRepository fcaPermissionsRepository,
            IFcaRoleRepository fcaRoleRepository,
            ILogger<CustomerDataInitService> logger)
        {
            _fcaService = fcaService;
            _companiesHouseService = companiesHouseService;
            _icoService = icoService;
            _serviceMapper = new ServiceMapper();
            _webScrapeService = webScrapeService;
            _fcaPermissionsRepository = fcaPermissionsRepository;
            _fcaRoleRepository = fcaRoleRepository;
            _logger = logger;
        }

        public async Task<DataProtectionLicense> GetDataProtectionLicenseAsync(string companyName)
        {
            if (string.IsNullOrEmpty(_azureStorageBaseUrl) || string.IsNullOrEmpty(_blobStorageContainerName))
            {
                throw new NullReferenceException("Please register the storage base url and container name first");
            }

            var icoSearchInput = new IcoSearchInput { CompanyName = companyName };
            var icoDetails = await _icoService.SearchDetailsAsync(icoSearchInput);

            if (!string.IsNullOrEmpty(icoDetails.RegistrationReference))
            {
                _icoService.Register(_azureStorageConnectionString, _blobStorageContainerName);
                await _icoService.SaveRegistrationCertificate(icoDetails.RegistrationReference);
            }

            var result = _serviceMapper.Instance.Map<DataProtectionLicense>(icoDetails);
            result.DocumentUrl =
                $"{_azureStorageBaseUrl}{_blobStorageContainerName}/DocStore/ICO/{result.LicenseNumber}/Registration-Certificate_{result.LicenseNumber}.pdf";

            var dataProtectionOfficer = result.DataProtectionOfficer;

            if (dataProtectionOfficer is not { IsOrganisation: true } ||
                string.IsNullOrEmpty(dataProtectionOfficer.CompanyName))
            {
                return result;
            }

            var matchedFirms = await _fcaService.SearchMatchedFirmsAsync(dataProtectionOfficer.CompanyName, true);
            var firmDetails = matchedFirms.FirstOrDefault(f =>
                f.CompanyName.Equals(dataProtectionOfficer.CompanyName,
                    StringComparison.InvariantCultureIgnoreCase));

            if (firmDetails == null)
            {
                return result;
            }

            dataProtectionOfficer.CompanyNumber = firmDetails.CompanyNumber;
            dataProtectionOfficer.FirmReferenceNo = firmDetails.FirmReferenceNo;

            return result;
        }

        public async Task<List<string>> GetTradingNamesAsync(string firmRefNo)
        {
            var tradingNamesBase = await _fcaService.GetTradingNamesAsync(firmRefNo);
            var tradingNames = tradingNamesBase.ToList();
            return !tradingNames.Any() ? new List<string>() : tradingNames;
        }

        public async Task<List<MediaMarketingOutlet>> GetMediaMarketingOutletsAsync(string customerId, string firmRefNo)
        {
            var mediaMarketingOutlets = new List<MediaMarketingOutlet>();
            const string addressType = ""; // Get all regardless of type
            var fcaAddressesBase = await _fcaService.GetFirmAddressDetailsAsync(firmRefNo, addressType);
            var fcaAddresses = fcaAddressesBase.ToList();
            if (!fcaAddresses.Any())
            {
                return mediaMarketingOutlets;
            }

            mediaMarketingOutlets = fcaAddresses.Where(fa => !string.IsNullOrEmpty(fa.WebsiteAddress)).Select(fa =>
                new MediaMarketingOutlet
                {
                    Id = Guid.NewGuid().ToString(),
                    Url = FormatUrl(fa.WebsiteAddress),
                    Name = "Default Website",
                    Archived = false,
                    Platform = "website"
                }).ToList();

            var mediaMarketingOutlet = mediaMarketingOutlets.FirstOrDefault();
            if (mediaMarketingOutlet != null)
            {
                await _webScrapeService.RegisterMedia(customerId, mediaMarketingOutlet.Id, mediaMarketingOutlet.Url);
            }

            return mediaMarketingOutlets;

            static string FormatUrl(string url) =>
                !string.IsNullOrEmpty(url) && !Uri.IsWellFormedUriString(url, UriKind.Absolute)
                    ? $"https://{url}"
                    : url;
        }

        public async Task<List<CorporateController>> GetCorporateControllersAsync(string companyNumber)
        {
            var corporateControllers = new List<CorporateController>();
            var corporateControllersBase =
                await _companiesHouseService.GetCorporateControllersRecursiveAsync(companyNumber);

            if (!corporateControllersBase.Any())
            {
                return corporateControllers;
            }

            corporateControllers = _serviceMapper.Instance.Map<List<CorporateController>>(corporateControllersBase)
                .ToList();
            var firmDataSetterTasks = corporateControllers
                .Select(SetCompanyFirmDataAsync).ToList();

            await Task.WhenAll(firmDataSetterTasks);

            return corporateControllers;

            async Task SetCompanyFirmDataAsync(CorporateController corporateController)
            {
                var matchedFirms = (await _fcaService.SearchMatchedFirmsAsync(corporateController.CompanyName ?? "",
                    true,
                    corporateController.CompanyNumber ?? "", corporateController.RegisteredAddress ?? "")).ToList();

                if (!matchedFirms.Any())
                {
                    return;
                }

                var firm = matchedFirms.First();
                corporateController.FirmReferenceNumber = firm.FirmReferenceNo;
                var addressDetails = await GetTradingAddressAsync(firm.FirmReferenceNo);

                if (addressDetails == null)
                {
                    return;
                }

                corporateController.TradingAddress = addressDetails.Address;
                corporateController.ContactNumber = new ContactNumber
                {
                    Number = addressDetails.PhoneNumber?.Replace("+44", "")
                };
            }
        }

        public async Task<List<IndividualController>> GetIndividualControllersAsync(string companyNumber)
        {
            var individualControllers = new List<IndividualController>();
            var individualControllersBase =
                await _companiesHouseService.GetIndividualControllersAsync(companyNumber);
            if (!individualControllersBase.Any())
            {
                return individualControllers;
            }

            individualControllers = _serviceMapper.Instance.Map<List<IndividualController>>(individualControllersBase)
                .ToList();
            return individualControllers;
        }

        public async Task<List<AppointedRepresentative>> GetAppointedRepresentativesAsync(string firmRefNo)
        {
            var appointedRepresentatives = new List<AppointedRepresentative>();

            var taskFull = _fcaService.GetAppointedRepresentativesAsync(firmRefNo, "Full");
            var taskIntroducer = _fcaService.GetAppointedRepresentativesAsync(firmRefNo, "Introducer");

            await Task.WhenAll(taskFull, taskIntroducer);

            var fcaAppointedRepresentativesFull = await taskFull;
            var fcaAppointedRepresentatives = fcaAppointedRepresentativesFull.Concat(await taskIntroducer).ToList();

            if (!fcaAppointedRepresentatives.Any())
            {
                return appointedRepresentatives;
            }

            appointedRepresentatives =
                _serviceMapper.Instance.Map<List<AppointedRepresentative>>(fcaAppointedRepresentatives);

            var dataSetterTasks = new List<Task>();
            foreach (var ar in appointedRepresentatives)
            {
                dataSetterTasks.Add(SetCompanyFirmData(ar));
            }

            await Task.WhenAll(dataSetterTasks);

            return appointedRepresentatives;

            async Task SetCompanyFirmData(AppointedRepresentative ar)
            {
                if (ar.Name == null)
                {
                    return;
                }

                var companies = await _companiesHouseService.SearchCompaniesWithDetailsAsync(ar.Name);

                var company = companies.FirstOrDefault(c =>
                    c.CompanyName.Equals(ar.Name, StringComparison.InvariantCultureIgnoreCase));

                if (company != null)
                {
                    ar.IsCompany = true;
                    ar.CompanyNumber = company.CompanyNumber;
                }


                var dataInitBuilder = new CustomerDataInitBuilder(ar, this, _logger);
                await dataInitBuilder
                    .InitRegisteredAddress()
                    .InitTradingAddress()
                    .InitTradingNames()
                    .BuildAppointedRepresentativeAsync();
            }
        }

        public async Task<AddressDetails> GetTradingAddressAsync(string firmRefNo)
        {
            const string addressType = "PPOB"; // Principal Point of Business
            var fcaAddresses = await _fcaService.GetFirmAddressDetailsAsync(firmRefNo, addressType);
            var fcaAddress = fcaAddresses.FirstOrDefault();
            if (fcaAddress == null)
            {
                return null;
            }

            var address = new StringBuilder();
            if (!string.IsNullOrEmpty(fcaAddress.Line1))
            {
                address.Append(fcaAddress.Line1);
            }

            if (!string.IsNullOrEmpty(fcaAddress.Line2))
            {
                address.Append($", {fcaAddress.Line2}");
            }

            if (!string.IsNullOrEmpty(fcaAddress.Line3))
            {
                address.Append($", {fcaAddress.Line3}");
            }

            if (!string.IsNullOrEmpty(fcaAddress.Line4))
            {
                address.Append($", {fcaAddress.Line4}");
            }

            if (!string.IsNullOrEmpty(fcaAddress.Town))
            {
                address.Append($", {fcaAddress.Town}");
            }

            if (!string.IsNullOrEmpty(fcaAddress.Country))
            {
                address.Append($", {fcaAddress.Country}");
            }

            if (!string.IsNullOrEmpty(fcaAddress.Postcode))
            {
                address.Append($", {fcaAddress.Postcode}");
            }

            var addressDetails = new AddressDetails
            {
                PhoneNumber = fcaAddress.PhoneNumber,
                WebsiteAddress = fcaAddress.WebsiteAddress,
                Address = address.ToString()
            };

            return addressDetails;
        }

        public async Task<string> GetRegisteredAddressAsync(string companyNumber)
        {
            var foundCompany = await _companiesHouseService.GetCompanyProfileAsync(companyNumber);

            if (foundCompany?.RegisteredOfficeAddress == null)
            {
                return null;
            }

            var foundAddress = foundCompany.RegisteredOfficeAddress;
            var address = new StringBuilder();

            if (!string.IsNullOrEmpty(foundAddress.AddressLine1))
            {
                address.Append(foundAddress.AddressLine1);
            }

            if (!string.IsNullOrEmpty(foundAddress.AddressLine2))
            {
                address.Append($", {foundAddress.AddressLine2}");
            }

            if (!string.IsNullOrEmpty(foundAddress.AddressLine3))
            {
                address.Append($", {foundAddress.AddressLine3}");
            }

            if (!string.IsNullOrEmpty(foundAddress.AddressLine4))
            {
                address.Append($", {foundAddress.AddressLine4}");
            }

            if (!string.IsNullOrEmpty(foundAddress.Locality))
            {
                address.Append($", {foundAddress.Locality}");
            }

            if (!string.IsNullOrEmpty(foundAddress.Country))
            {
                address.Append($", {foundAddress.Country}");
            }

            if (!string.IsNullOrEmpty(foundAddress.PostalCode))
            {
                address.Append($", {foundAddress.PostalCode}");
            }

            return address.ToString();
        }

        public void Register(string azureStorageBaseUrl, string azureStorageConnectionString,
            string blobStorageContainerName)
        {
            _azureStorageBaseUrl = azureStorageBaseUrl;
            _azureStorageConnectionString = azureStorageConnectionString;
            _blobStorageContainerName = blobStorageContainerName;
        }

        public async Task<List<Employee>> ComputeEmployeeAsync(Employee firstEmployee, string firmRefNo,
            string companyNo)
        {
            var results = (await _fcaService.GetFirmIndividualsAsync(firmRefNo)).ToList();
            var baseRoles = (await _fcaRoleRepository.GetFcaRolesAsync()).ToList();

            if (!results.Any())
            {
                return new List<Employee> { firstEmployee };
            }

            var employees = new List<Employee>();

            foreach (var item in results)
            {
                if (string.IsNullOrEmpty(item?.Name))
                {
                    continue;
                }

                //e.g. Ian Andrew Gray
                var fullName = item.Name.Split(" ");
                var lastIndex = fullName.Length - 1;
                var lastName = fullName[lastIndex];
                var firstName = "";

                for (var i = 0; i < lastIndex; i++)
                {
                    if (i > 0)
                    {
                        firstName += $" {fullName[i]}";
                        continue;
                    }

                    firstName += fullName[i];
                }

                var newEmployee = new Employee
                {
                    FirstName = firstName,
                    LastName = lastName,
                    CompanyNo = companyNo,
                    Id = Guid.NewGuid().ToString(),
                    EmploymentStatus = item.CurrentRoles.Any() ? "Active" : "Resigned"
                };
                SetIndividualRoles(newEmployee, item, baseRoles);
                employees.Add(newEmployee);
            }

            employees.Add(firstEmployee);
            return employees;
        }

        public async Task<List<Employee>> ComputeEmployeeAsync(string firmRefNo, string companyNo)
        {
            var results = (await _fcaService.GetFirmIndividualsAsync(firmRefNo)).ToList();
            var baseRoles = (await _fcaRoleRepository.GetFcaRolesAsync()).ToList();

            if (!results.Any())
            {
                return new List<Employee>();
            }

            var employees = new List<Employee>();

            foreach (var item in results)
            {
                if (string.IsNullOrEmpty(item?.Name))
                {
                    continue;
                }

                //e.g. Ian Andrew Gray
                var fullName = item.Name.Split(" ");
                var lastIndex = fullName.Length - 1;
                var lastName = fullName[lastIndex];
                var firstName = "";

                for (var i = 0; i < lastIndex - 1; i++)
                {
                    if (i > 0)
                    {
                        firstName += $" {fullName[i]}";
                        continue;
                    }

                    firstName += fullName[i];
                }

                var newEmployee = new Employee
                {
                    FirstName = firstName,
                    LastName = lastName,
                    CompanyNo = companyNo,
                    EmploymentStatus = item.CurrentRoles.Any() ? "Active" : "Resigned"
                };
                SetIndividualRoles(newEmployee, item, baseRoles);
                employees.Add(newEmployee);
            }

            return employees;
        }

        public async Task<List<CustomerPermission>> GetFirmFcaPermissionsAsync(string firmRefNo)
        {
            var customerPermissions = new List<CustomerPermission>();

            if (string.IsNullOrEmpty(firmRefNo))
            {
                return customerPermissions;
            }

            var definedPermissions = await _fcaPermissionsRepository.GetAllPermissionsAsync();
            var fcaPermissions = await _fcaService.SearchFirmPermissionsAsync(firmRefNo);
            var clientMoney = await _fcaService.SearchFirmClientMoneyPermissionAsync(firmRefNo);

            if (!string.IsNullOrEmpty(clientMoney))
            {
                fcaPermissions.PermissionNames.Add(clientMoney);
            }

            customerPermissions.AddRange(from permission in definedPermissions
                let state = GetPermissionState(permission, fcaPermissions)
                select new CustomerPermission
                {
                    Id = permission.Id,
                    PermissionGroupName = permission.PermissionGroupName,
                    CategoryName = permission.CategoryName,
                    SubPermissionName = permission.SubPermissionName,
                    SubPermissionDisplayText = permission.SubPermissionDisplayText,
                    State = state,
                    HasPendingApplication = state == "Added",
                    IsModified = false
                });

            return customerPermissions;
        }

        private static string GetPermissionState(FcaPermission fcaPermission, FcaPermissionsResult fcaPermissionResult)
        {
            var permissionGroupName = fcaPermission.PermissionGroupName;
            var subPermissionName = fcaPermission.SubPermissionName;
            var permissionNames = fcaPermissionResult.PermissionNames;
            var found = permissionNames.FirstOrDefault(name =>
                name.ToLower() == permissionGroupName.ToLower() || name.ToLower() == subPermissionName.ToLower());
            return found != null ? "Added" : "Removed";
        }

        private static void SetIndividualRoles(Employee employee, FcaIndividual fcaIndividual,
            IEnumerable<FcaRole> baseRoles)
        {
            var employeeBaseRoles = baseRoles.Where(baseRole =>
                fcaIndividual.CurrentRoles.Any(cr => cr.Name != null && cr.Name.Equals(baseRole.Name))).ToList();
            var employeeRoles = employeeBaseRoles.OrderBy(o => o.Order).Select(ebr =>
                new Role { IsFcaAuthorised = true, Name = ebr.DisplayText, State = "Added" }).ToList();

            if (employeeRoles.Any())
            {
                employee.PrimaryRole = employeeRoles.First();
            }

            if (employeeRoles.Any() && employeeRoles.Count > 1)
            {
                employee.OtherRoles = employeeRoles.Skip(1).ToList();
            }
        }
    }
}