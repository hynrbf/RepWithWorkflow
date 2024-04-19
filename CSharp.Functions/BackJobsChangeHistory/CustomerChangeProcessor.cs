using Common;
using Common.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace BackJobsChangeHistory
{
    public class CustomerChangeProcessor
    {
        private readonly ICustomerReplicationRepository _customerReplicationRepository;
        private readonly ICustomerFcaReplicationRepository _customerFcaReplicationRepository;

        public CustomerChangeProcessor(ICustomerReplicationRepository customerReplicationRepository,
            ICustomerFcaReplicationRepository customerFcaReplicationRepository)
        {
            _customerReplicationRepository = customerReplicationRepository;
            _customerFcaReplicationRepository = customerFcaReplicationRepository;
        }

        [FunctionName("CustomerChangeProcessor")]
        public async Task Run([CosmosDBTrigger(
                databaseName: "%Database%",
                containerName: "customer",
                Connection = "CosmosDbConnectionString",
                LeaseContainerName = "customerLease",
                CreateLeaseContainerIfNotExists = true)]
            IReadOnlyList<Customer> customers,
            ILogger log)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0.0";
            log.LogInformation("CustomerChangeProcessor function executed at {Now} with version {Version}",
                DateTime.Now, version);

            log.LogInformation($"Received {customers.Count} changed customers");

            if (!bool.TryParse(Helpers.GetEnvironmentVariable("IsEnable"), out var isEnabled))
            {
                isEnabled = false;
            }

            if (!isEnabled)
            {
                log.LogWarning($"{nameof(BackJobsChangeHistory)}: is DISABLED.");
                return;
            }

            var saveTasks = new List<Task>();

            foreach (var customer in customers)
            {
                customer.Id = Guid.NewGuid().ToString();

                if (customer.ChangedBy == ChangeSource.JobDataInitializer.ToString())
                {
                    saveTasks.Add(_customerFcaReplicationRepository.SaveCustomerFcaReplicaAsync(customer));
                }

                saveTasks.Add(_customerReplicationRepository.SaveCustomerReplicaAsync(customer));
            }

            await Task.WhenAll(saveTasks);
        }
    }
}