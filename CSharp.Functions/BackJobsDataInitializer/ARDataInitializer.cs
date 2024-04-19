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
    public class ARDataInitializer
    {
        private static readonly SemaphoreSlim Semaphore = new(1, 1);
        private static bool _isAllTasksCompleted = true;

        private readonly ICustomerDataInitService _dataInitService;
        private readonly IAppointedRepresentativeRepository _appointedRepresentativeRepository;
        private readonly bool _isEnabled;

        private ILogger _logger;

        public ARDataInitializer(ICustomerDataInitService dataInitService,
            IAppointedRepresentativeRepository appointedRepresentativeRepository)
        {
            _dataInitService = dataInitService;
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

            if (!bool.TryParse(Helpers.GetEnvironmentVariable("IsEnableAR"), out var isEnabled))
            {
                _isEnabled = false;
            }

            _isEnabled = isEnabled;
        }

        [FunctionName(nameof(ARDataInitializer))]
        public async Task Run([TimerTrigger("%ScheduleExpressionAR%")] TimerInfo myTimer, ILogger log,
            ExecutionContext context)
        {
            _logger = log;

            var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0.0";
            log.LogInformation("AR Data Initializer function executed at {Now} with version {Version}", DateTime.Now,
                version);

            if (!_isEnabled)
            {
                log.LogWarning($"{nameof(ARDataInitializer)}: AR Data Initializer is DISABLED.");
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

            try
            {
                var appointedRepresentativeList =
                    (await _appointedRepresentativeRepository.GetAppointedRepresentativesForDataInitAsync()).ToList();

                var arUpdateTasks = new List<Task<AppointedRepresentative>>();
                foreach (var appointedRepresentative in appointedRepresentativeList)
                {
                    SetChangeInfo(appointedRepresentative);
                    appointedRepresentative.IsInProgressDataInitializing = true;
                    arUpdateTasks.Add(
                        _appointedRepresentativeRepository.SaveOrUpdateAppointedRepresentativeAsync(
                            appointedRepresentative));
                }

                await Task.WhenAll(arUpdateTasks);
                var arInitUpdateTasks =
                    appointedRepresentativeList.Select(InitUpdateAppointedRepresentativeAsync).ToList();

                await Task.WhenAll(arInitUpdateTasks);
                dataInitTaskStatus = true;
            }
            finally
            {
                await Semaphore.WaitAsync();
                _isAllTasksCompleted = dataInitTaskStatus;
                Semaphore.Release();
            }
        }

        private async Task<AppointedRepresentative> InitUpdateAppointedRepresentativeAsync(
            AppointedRepresentative appointedRepresentative)
        {
            var saveTasks = new List<Task>();
            var customerDataInitBuilder =
                new CustomerDataInitBuilder(appointedRepresentative, _dataInitService, _logger);
            var initializedAR = await customerDataInitBuilder
                .InitRegisteredAddress()
                .InitTradingAddress()
                .InitTradingNames()
                .InitMediaMarketingOutlet()
                .InitCorporateControllers()
                .InitIndividualControllers()
                .InitDataProtectionLicense()
                .BuildAppointedRepresentativeAsync();

            SetChangeInfo(initializedAR);
            initializedAR.IsInProgressDataInitializing = false;
            saveTasks.Add(_appointedRepresentativeRepository.SaveOrUpdateAppointedRepresentativeAsync(initializedAR));

            await Task.WhenAll(saveTasks);
            return initializedAR;
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