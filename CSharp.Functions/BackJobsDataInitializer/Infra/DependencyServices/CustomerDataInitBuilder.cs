using System;
using Common.Entities;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;

namespace BackJobsDataInitializer.Infra
{
    public class CustomerDataInitBuilder
    {
        private const string NotApplicableId = "Not Applicable";

        private readonly CustomerBase _customer;
        private readonly ICustomerDataInitService _dataInitService;
        private readonly List<Task> _initTasks = new(); // To control non-null tasks
        private readonly ILogger _logger;

        private Task<string> _registeredAddressInitTask;
        private Task<AddressDetails> _tradingAddressInitTask;
        private Task<List<string>> _tradingNamesInitTask;
        private Task<List<MediaMarketingOutlet>> _mediaMarketingOutletsInitTask;
        private Task<List<CorporateController>> _corporateControllersInitTask;
        private Task<List<IndividualController>> _individualControllersInitTask;
        private Task<List<AppointedRepresentative>> _appointedRepresentativesInitTask;
        private Task<DataProtectionLicense> _dataProtectionLicenseInitTask;
        private Task<List<Employee>> _organizationalEmployeesTask;
        private Task<List<CustomerPermission>> _customerFcaPermissionsTask;

        public List<Employee> Employees { get; private set; } = new();
        public List<AppointedRepresentative> AppointedRepresentatives { get; private set; } = new();
        private readonly string _initType;

        public CustomerDataInitBuilder(CustomerBase customer, ICustomerDataInitService dataInitService, ILogger logger)
        {
            _customer = customer;
            _initType = _customer switch
            {
                Customer => "Customer",
                AppointedRepresentative => "AR",
                _ => nameof(_customer)
            };

            _dataInitService = dataInitService;
            _logger = logger;
        }

        public CustomerDataInitBuilder InitRegisteredAddress()
        {
            var companyNumber = _customer?.CompanyNumber ?? "";

            if (string.IsNullOrEmpty(companyNumber) ||
                companyNumber.Equals(NotApplicableId, StringComparison.InvariantCultureIgnoreCase))
            {
                return this;
            }

            _registeredAddressInitTask = _dataInitService.GetRegisteredAddressAsync(companyNumber);
            _logger.LogInformation("{InitType}: Initializing Registered Address for Company Number {CompanyNumber}",
                _initType, companyNumber);
            _initTasks.Add(_registeredAddressInitTask);
            return this;
        }

        public CustomerDataInitBuilder InitTradingAddress()
        {
            var firmRefNumber = _customer?.FirmReferenceNumber ?? "";

            if (string.IsNullOrEmpty(firmRefNumber) ||
                firmRefNumber.Equals(NotApplicableId, StringComparison.InvariantCultureIgnoreCase))
            {
                return this;
            }

            _tradingAddressInitTask = _dataInitService.GetTradingAddressAsync(firmRefNumber);
            _logger.LogInformation("{InitType}: Initializing Trading Address for Firm Reference Number {FirmRefNumber}",
                _initType, firmRefNumber);
            _initTasks.Add(_tradingAddressInitTask);
            return this;
        }

        public CustomerDataInitBuilder InitDataProtectionLicense()
        {
            var companyName = _customer?.CompanyName ?? "";

            if (string.IsNullOrEmpty(companyName) ||
                companyName.Equals(NotApplicableId, StringComparison.InvariantCultureIgnoreCase))
            {
                return this;
            }

            _dataProtectionLicenseInitTask = _dataInitService.GetDataProtectionLicenseAsync(companyName);
            _logger.LogInformation("{InitType}: Initializing Data Protection License for Company Name {CompanyName}",
                _initType, companyName);
            _initTasks.Add(_dataProtectionLicenseInitTask);
            return this;
        }

        public CustomerDataInitBuilder InitTradingNames()
        {
            var firmRefNumber = _customer?.FirmReferenceNumber ?? "";

            if (string.IsNullOrEmpty(firmRefNumber) ||
                firmRefNumber.Equals(NotApplicableId, StringComparison.InvariantCultureIgnoreCase))
            {
                return this;
            }

            _tradingNamesInitTask = _dataInitService.GetTradingNamesAsync(firmRefNumber);
            _logger.LogInformation("{InitType}: Initializing Trading Names for Firm Reference Number {FirmRefNumber}",
                _initType, firmRefNumber);
            _initTasks.Add(_tradingNamesInitTask);
            return this;
        }

        public CustomerDataInitBuilder InitMediaMarketingOutlet()
        {
            var firmRefNumber = _customer?.FirmReferenceNumber ?? "";

            if (string.IsNullOrEmpty(firmRefNumber) ||
                firmRefNumber.Equals(NotApplicableId, StringComparison.InvariantCultureIgnoreCase))
            {
                return this;
            }

            _mediaMarketingOutletsInitTask =
                _dataInitService.GetMediaMarketingOutletsAsync(_customer?.Id ?? "", firmRefNumber);
            _logger.LogInformation(
                "{InitType}: Initializing Media Marketing Outlets for Firm Reference Number {FirmRefNumber}", _initType,
                firmRefNumber);
            _initTasks.Add(_mediaMarketingOutletsInitTask);
            return this;
        }

        public CustomerDataInitBuilder InitAppointedRepresentatives()
        {
            var firmRefNumber = _customer?.FirmReferenceNumber ?? "";

            if (string.IsNullOrEmpty(firmRefNumber) ||
                firmRefNumber.Equals(NotApplicableId, StringComparison.InvariantCultureIgnoreCase))
            {
                return this;
            }

            _appointedRepresentativesInitTask = _dataInitService.GetAppointedRepresentativesAsync(firmRefNumber);
            _logger.LogInformation(
                "{InitType}: Initializing Appointed Representatives for Firm Reference Number {{firmRefNumber}}",
                _initType);
            _initTasks.Add(_appointedRepresentativesInitTask);
            return this;
        }

        public CustomerDataInitBuilder InitCorporateControllers()
        {
            var companyNumber = _customer?.CompanyNumber ?? "";

            if (string.IsNullOrEmpty(companyNumber) ||
                companyNumber.Equals(NotApplicableId, StringComparison.InvariantCultureIgnoreCase))
            {
                return this;
            }

            _corporateControllersInitTask = _dataInitService.GetCorporateControllersAsync(companyNumber);
            _logger.LogInformation("{InitType}: Initializing Corporate Controllers for Company Number {CompanyNumber}",
                _initType, companyNumber);
            _initTasks.Add(_corporateControllersInitTask);
            return this;
        }

        public CustomerDataInitBuilder InitIndividualControllers()
        {
            var companyNumber = _customer?.CompanyNumber ?? "";

            if (string.IsNullOrEmpty(companyNumber) ||
                companyNumber.Equals(NotApplicableId, StringComparison.InvariantCultureIgnoreCase))
            {
                return this;
            }

            _individualControllersInitTask = _dataInitService.GetIndividualControllersAsync(companyNumber);
            _logger.LogInformation(
                "{InitType}: Initializing Corporate Controllers for Company Number {{companyNumber}}", _initType);
            _initTasks.Add(_individualControllersInitTask);
            return this;
        }

        public CustomerDataInitBuilder InitCustomerPermissions()
        {
            var firmRefNumber = _customer?.FirmReferenceNumber ?? "";

            if (string.IsNullOrEmpty(firmRefNumber) ||
                firmRefNumber.Equals(NotApplicableId, StringComparison.InvariantCultureIgnoreCase))
            {
                return this;
            }

            _customerFcaPermissionsTask = _dataInitService.GetFirmFcaPermissionsAsync(firmRefNumber);
            _logger.LogInformation(
                "{InitType}: Initializing FCA Permissions for Company with Firm Reference Number {FirmRefNumber}",
                _initType, firmRefNumber);
            _initTasks.Add(_customerFcaPermissionsTask);
            return this;
        }

        public CustomerDataInitBuilder InitOrganizationalEmployees()
        {
            var companyNumber = _customer?.CompanyNumber ?? "";
            var firmRefNumber = _customer?.FirmReferenceNumber ?? "";

            if (string.IsNullOrEmpty(companyNumber) ||
                companyNumber.Equals(NotApplicableId, StringComparison.InvariantCultureIgnoreCase) ||
                string.IsNullOrEmpty(firmRefNumber) ||
                firmRefNumber.Equals(NotApplicableId, StringComparison.InvariantCultureIgnoreCase))
            {
                return this;
            }

            _logger.LogInformation(
                "{InitType}: Initializing organizational employees for Company Number {CompanyNumber}", _initType,
                companyNumber);
            var firstEmployeeDetails = _customer?.FirmRepresentativeDetail;

            if (firstEmployeeDetails != null)
            {
                _organizationalEmployeesTask = _dataInitService.ComputeEmployeeAsync(new Employee
                {
                    Email = firstEmployeeDetails.EmailAddress,
                    FirstName = firstEmployeeDetails.Forename,
                    LastName = firstEmployeeDetails.Surname,
                    ContactNumber = firstEmployeeDetails.ContactNumber,
                    CompanyNo = companyNumber,
                    Id = Guid.NewGuid().ToString()
                }, firmRefNumber, companyNumber);
                _initTasks.Add(_organizationalEmployeesTask);
                return this;
            }

            _organizationalEmployeesTask = _dataInitService.ComputeEmployeeAsync(firmRefNumber, companyNumber);
            _initTasks.Add(_organizationalEmployeesTask);
            return this;
        }

        public async Task<Customer> BuildCustomerAsync()
        {
            if (_customer is not Customer customer)
            {
                return new Customer();
            }

            if (!_initTasks.Any())
            {
                return customer;
            }

            await Task.WhenAll(_initTasks);

            _logger.LogInformation("Tasks execution were completed");
            customer.FirmDetail ??= InitFirmDetail<FirmDetail>();

            if (_registeredAddressInitTask?.Result != null && customer.FirmDetail != null)
            {
                customer.FirmDetail.RegisteredAddress = _registeredAddressInitTask.Result;
            }

            if (_tradingAddressInitTask?.Result != null && customer.FirmDetail != null)
            {
                customer.FirmDetail.TradingAddress = _tradingAddressInitTask?.Result.Address;
                customer.FirmDetail.Website = _tradingAddressInitTask?.Result.WebsiteAddress;
                customer.FirmDetail.ContactNumber = new ContactNumber
                {
                    //we default to UK first
                    CountryCode = "gb",
                    Country = "United Kingdom",
                    DialCode = "+44",
                    Number = _tradingAddressInitTask?.Result.PhoneNumber
                };
            }

            var tradingNames = _tradingNamesInitTask?.Result ?? new List<string>();

            if (tradingNames.Any() && customer.FirmDetail != null)
            {
                customer.FirmDetail.TradingNames = tradingNames;
            }

            var mediaMarketingOutlets = _mediaMarketingOutletsInitTask?.Result ?? new List<MediaMarketingOutlet>();

            if (mediaMarketingOutlets.Any())
            {
                customer.MediaMarketingOutlets = mediaMarketingOutlets;
            }

            AppointedRepresentatives = _appointedRepresentativesInitTask?.Result ?? new List<AppointedRepresentative>();

            var individualControllers = _individualControllersInitTask?.Result ?? new List<IndividualController>();

            if (individualControllers.Any())
            {
                customer.IndividualControllers = individualControllers;
            }

            var corporateControllers = _corporateControllersInitTask?.Result ?? new List<CorporateController>();

            if (corporateControllers.Any())
            {
                customer.CorporateControllers = corporateControllers;
            }

            var dataProtectionLicense = _dataProtectionLicenseInitTask?.Result ?? new DataProtectionLicense();
            if (!string.IsNullOrEmpty(dataProtectionLicense.LicenseNumber))
            {
                customer.DataProtectionLicense = dataProtectionLicense;
            }

            var customerFcaPermissions = _customerFcaPermissionsTask?.Result ?? new List<CustomerPermission>();
            if (customerFcaPermissions.Any())
            {
                customer.CurrentFcaPermissions = customerFcaPermissions;
                customer.CustomerPermissions = customerFcaPermissions;
            }


            Employees = _organizationalEmployeesTask?.Result ?? new List<Employee>();

            return customer;
        }

        public async Task<AppointedRepresentative> BuildAppointedRepresentativeAsync()
        {
            if (_customer is not AppointedRepresentative appointedRepresentative)
            {
                return new AppointedRepresentative();
            }

            if (!_initTasks.Any())
            {
                return appointedRepresentative;
            }

            await Task.WhenAll(_initTasks);

            _logger.LogInformation("Tasks execution were completed");

            //TODO: Duplicate, may need to delete
            appointedRepresentative.FirmDetail ??= InitFirmDetail<ARFirmDetail>();

            if (_registeredAddressInitTask?.Result != null && appointedRepresentative.FirmDetail != null)
            {
                appointedRepresentative.RegisteredAddress = _registeredAddressInitTask.Result;
                appointedRepresentative.FirmDetail.RegisteredAddress = _registeredAddressInitTask.Result;
            }

            if (_tradingAddressInitTask?.Result != null && appointedRepresentative.FirmDetail != null)
            {
                var contactNumber = new ContactNumber
                {
                    //we default to UK first
                    CountryCode = "gb",
                    Country = "United Kingdom",
                    DialCode = "+44",
                    Number = _tradingAddressInitTask?.Result.PhoneNumber
                };

                appointedRepresentative.TradingAddress = _tradingAddressInitTask?.Result.Address;
                appointedRepresentative.Website = _tradingAddressInitTask?.Result.WebsiteAddress;
                appointedRepresentative.ContactNumber = contactNumber;

                //TODO: Duplicate, may need to delete
                appointedRepresentative.FirmDetail.TradingAddress = _tradingAddressInitTask?.Result.Address;
                appointedRepresentative.FirmDetail.Website = _tradingAddressInitTask?.Result.WebsiteAddress;
                appointedRepresentative.FirmDetail.ContactNumber = contactNumber;
            }

            var tradingNames = _tradingNamesInitTask?.Result ?? new List<string>();

            if (tradingNames.Any() && appointedRepresentative.FirmDetail != null)
            {
                appointedRepresentative.TradingNames = tradingNames;
                appointedRepresentative.FirmDetail.TradingNames = tradingNames;
            }

            var mediaMarketingOutlets = _mediaMarketingOutletsInitTask?.Result ?? new List<MediaMarketingOutlet>();

            if (mediaMarketingOutlets.Any())
            {
                appointedRepresentative.MediaMarketingOutlets = mediaMarketingOutlets;
            }

            var individualControllers = _individualControllersInitTask?.Result ?? new List<IndividualController>();

            if (individualControllers.Any())
            {
                appointedRepresentative.IndividualControllers = individualControllers;
            }

            var corporateControllers = _corporateControllersInitTask?.Result ?? new List<CorporateController>();

            if (corporateControllers.Any())
            {
                appointedRepresentative.CorporateControllers = corporateControllers;
            }

            var dataProtectionLicense = _dataProtectionLicenseInitTask?.Result ?? new DataProtectionLicense();
            if (!string.IsNullOrEmpty(dataProtectionLicense.LicenseNumber))
            {
                appointedRepresentative.DataProtectionLicense = dataProtectionLicense;
            }

            Employees = _organizationalEmployeesTask?.Result ?? new List<Employee>();

            return appointedRepresentative;
        }

        private T InitFirmDetail<T>() where T : FirmDetailBase, new()
        {
            return new T
            {
                FirmName = _customer.CompanyName,
                CompanyNumber = _customer.CompanyNumber ?? AppConstants.NotApplicable,
                FirmReferenceNumber = _customer.FirmReferenceNumber ?? AppConstants.NotApplicable,
                EmailAddress = _customer.Email
            };
        }
    }
}