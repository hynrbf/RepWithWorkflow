using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BackJobsDataInitializer.Infra;
using Common;
using Common.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace BackJobsDataInitializer
{
    public class JobDataInitializer
    {
        private static readonly SemaphoreSlim Semaphore = new(1, 1);
        private static bool _isAllTasksCompleted = true;

        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerDataInitService _dataInitService;
        private readonly IOrganizationalStructureRepository _organizationalStructureRepository;
        private readonly IAppointedRepresentativeRepository _appointedRepresentativeRepository;
        private readonly bool _isEnabled;

        private ILogger _logger;

        public JobDataInitializer(ICustomerRepository customerRepository, ICustomerDataInitService dataInitService,
            IOrganizationalStructureRepository organizationalStructureRepository,
            IAppointedRepresentativeRepository appointedRepresentativeRepository)
        {
            _customerRepository = customerRepository;
            _dataInitService = dataInitService;
            _organizationalStructureRepository = organizationalStructureRepository;
            _appointedRepresentativeRepository = appointedRepresentativeRepository;

            var azureStorageUrl =
                Environment.GetEnvironmentVariable("AzureStorageBaseUrl", EnvironmentVariableTarget.Process);
            var azureStorageConnectionString =
                Environment.GetEnvironmentVariable("AzureStorageConnectionString", EnvironmentVariableTarget.Process);
            var containerName =
                Environment.GetEnvironmentVariable("BlobStorageContainerName", EnvironmentVariableTarget.Process);
            _dataInitService.Register(
                azureStorageUrl ??
                throw new NullReferenceException($"Base Url must exist in {nameof(BackJobsDataInitializer)}"),
                azureStorageConnectionString ??
                throw new NullReferenceException(
                    $"Azure Storage Connection String must exist in {nameof(BackJobsDataInitializer)}"),
                containerName ??
                throw new NullReferenceException($"Container Name must exist in {nameof(BackJobsDataInitializer)}"));

            if (!bool.TryParse(Helpers.GetEnvironmentVariable("IsEnable"), out var isEnabled))
            {
                _isEnabled = false;
            }

            _isEnabled = isEnabled;
        }

        [FunctionName(nameof(JobDataInitializer))]
        public async Task Run(
            [TimerTrigger("%ScheduleExpression%")] TimerInfo myTimer,
            ILogger log,
            ExecutionContext context)
        {
            _logger = log;

            var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0.0";
            log.LogInformation("Customer Data Initializer function executed at {Now} with version {Version}", DateTime.Now,
                version);

            if (!_isEnabled)
            {
                log.LogWarning($"{nameof(JobDataInitializer)}: Customer Data Initializer is DISABLED.");
                return;
            }

            await Semaphore.WaitAsync();

            if (!_isAllTasksCompleted)
            {
                Semaphore.Release();
                log.LogInformation("we will run again in the next round when all tasks are completed");
                return;
            }

            _isAllTasksCompleted = false;
            Semaphore.Release();
            var dataInitTaskStatus = false;
            var customersInProgress = new List<Customer>();

            try
            {
                var customersList = (await _customerRepository.GetCustomersForDataInitAsync()).ToList();
                var customerUpdateTasks = new List<Task<Customer>>();

                foreach (var customer in customersList)
                {
                    SetChangeInfo(customer);
                    customer.IsInProgressDataInitializing = true;
                    customerUpdateTasks.Add(_customerRepository.SaveCustomerAsync(customer));
                }

                await Task.WhenAll(customerUpdateTasks);
                var customerInitUpdateTasks = customersList.Select(InitUpdateCustomerAsync).ToList();

                customersInProgress = (await Task.WhenAll(customerInitUpdateTasks)).ToList();
                dataInitTaskStatus = true;
            }
            finally
            {
                if (customersInProgress.Any())
                {
                    try
                    {
                        var customerUpdateTasks = new List<Task<Customer>>();

                        foreach (var customer in customersInProgress)
                        {
                            SetChangeInfo(customer);
                            customer.IsInProgressDataInitializing = false;
                            customerUpdateTasks.Add(_customerRepository.SaveCustomerAsync(customer));
                        }

                        await Task.WhenAll(customerUpdateTasks);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Error in resetting in progress of customer: {ExMessage}", ex.Message);
                    }
                }

                await Semaphore.WaitAsync();
                _isAllTasksCompleted = dataInitTaskStatus;
                Semaphore.Release();
            }
        }

        private async Task<Customer> InitUpdateCustomerAsync(Customer customer)
        {
            var saveTasks = new List<Task>();
            var customerDataInitBuilder = new CustomerDataInitBuilder(customer, _dataInitService, _logger);
            var initializedCustomer = await customerDataInitBuilder
                .InitRegisteredAddress()
                .InitTradingAddress()
                .InitTradingNames()
                .InitMediaMarketingOutlet()
                .InitCorporateControllers()
                .InitIndividualControllers()
                .InitAppointedRepresentatives()
                .InitDataProtectionLicense()
                .InitOrganizationalEmployees()
                .InitCustomerPermissions()
                .BuildCustomerAsync();

            if (customerDataInitBuilder.Employees.Any())
            {
                saveTasks.Add(
                    _organizationalStructureRepository.SaveBulkEmployeesAsync(customerDataInitBuilder.Employees));
            }

            if (customerDataInitBuilder.AppointedRepresentatives.Any())
            {
                customerDataInitBuilder.AppointedRepresentatives.ForEach(ar => ar.CustomerId = customer.Id);
                saveTasks.Add(_appointedRepresentativeRepository.SaveBulkAppointedRepresentativesAsync(
                    customerDataInitBuilder
                        .AppointedRepresentatives));
            }

            SetChangeInfo(initializedCustomer);
            saveTasks.Add(_customerRepository.SaveCustomerAsync(initializedCustomer));

            await Task.WhenAll(saveTasks);
            return initializedCustomer;
        }

        private static void SetChangeInfo(ChangeInfo customer)
        {
            customer.ChangedBy = ChangeSource.JobDataInitializer.ToString();
            customer.ChangedOn = DateHelper.GetCurrentDateTimeInEpoch();
            customer.IpAddress = GetIpAddress();
        }

        private static string GetIpAddress()
        {
            var hostName = Dns.GetHostName();
            var ipAddresses = Dns.GetHostAddresses(hostName);
            var ipAddress = ipAddresses.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            return ipAddress != null ? ipAddress.ToString() : "";
        }
    }
}