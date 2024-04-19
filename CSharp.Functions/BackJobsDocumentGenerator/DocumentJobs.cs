using Common.Infra;
using Common.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Common;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace BackJobsDocumentGenerator
{
    public class DocumentJobs
    {
        private static readonly SemaphoreSlim Semaphore = new(1, 1);
        private static bool _isAllTasksCompleted = true;

        private readonly ICustomerRepository _customerRepository;
        private readonly IDocGeneratorService _docGeneratorService;
        private readonly IBlobContainerService _blobContainerClientService;
        private readonly bool _isEnabled;

        private string _wwwRootDirectory;

        public DocumentJobs(ICustomerRepository customerRepository,
            IDocGeneratorService docGeneratorService,
            IBlobContainerService blobContainerClientService)
        {
            _customerRepository = customerRepository;
            _docGeneratorService = docGeneratorService;
            _blobContainerClientService = blobContainerClientService;
            _blobContainerClientService.Register();

            if (!bool.TryParse(Helpers.GetEnvironmentVariable("IsEnable"), out var isEnabled))
            {
                _isEnabled = false;
            }

            _isEnabled = isEnabled;
        }

        [FunctionName(nameof(DocumentJobs))]
        public async Task Run(
            [TimerTrigger("%ScheduleExpression%")] TimerInfo myTimer,
            ILogger log,
            ExecutionContext context)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0.0";
            log.LogInformation("Document generator function executed at {Now} with version {Version}", DateTime.Now,
                version);
            _wwwRootDirectory = context.FunctionAppDirectory;

            if (!_isEnabled)
            {
                log.LogWarning($"{nameof(DocumentJobs)}: Document Generation is DISABLED.");
                return;
            }

            await Semaphore.WaitAsync();

            if (!_isAllTasksCompleted)
            {
                Semaphore.Release();
                log.LogInformation("we will run again in the next round when all tasks are completed");
                return;
            }

            //we just maintain semaphore here because we know we'll add more document generation tasks here
            //again like before
            _isAllTasksCompleted = false;
            Semaphore.Release();
            var proposalTaskStatus = false;

            try
            {
                proposalTaskStatus = await GenerateProposalForCustomerAsync(log);
            }
            catch (DocumentNotFoundInDBException docEx)
            {
                log.LogError(docEx, "An error occurred");
            }
            finally
            {
                await Semaphore.WaitAsync();
                _isAllTasksCompleted = proposalTaskStatus;
                Semaphore.Release();
            }
        }

        private async Task<bool> GenerateProposalForCustomerAsync(ILogger log)
        {
            var customers = await _customerRepository.GetCustomersForProposalEmailAsync();
            var recipientsOfProposal = customers.ToList();

            if (!recipientsOfProposal.Any())
            {
                log.LogWarning($"{nameof(GenerateProposalForCustomerAsync)}: No recipients for 'Proposal'");
                return true;
            }

            foreach (var customer in recipientsOfProposal.Where(customer => !customer.IsLockProposalDocument))
            {
                log.LogInformation(
                    "{GenerateProposalForCustomerAsyncName}: Generating \'Proposal\' document for {CustomerEmail}",
                    nameof(GenerateProposalForCustomerAsync), customer.Email);
                await _docGeneratorService.GenerateDocumentAsync(customer, DocumentNames.Proposal.ToString(), log,
                    _blobContainerClientService.BlobContainerClient, _wwwRootDirectory);
            }

            return true;
        }

        private async Task<bool> GenerateDirectDebitMandateForCustomerAsync(ILogger log)
        {
            var customers = await _customerRepository.GetCustomersForDirectDebitEmailAsync();
            var recipientsOfDirectDebitMandate = customers.ToList();

            if (!recipientsOfDirectDebitMandate.Any())
            {
                log.LogWarning(
                    $"{nameof(GenerateDirectDebitMandateForCustomerAsync)}: No recipients for 'Direct Debit Mandate'");
                return true;
            }

            foreach (var customer in recipientsOfDirectDebitMandate)
            {
                //convert to compile-time constant message
                //https://softwareengineering.stackexchange.com/questions/312197/benefits-of-structured-logging-vs-basic-logging
                log.LogInformation(
                    "{GenerateDirectDebitMandateForCustomerAsyncName}: Generating \'Direct Debit Mandate\' document for {CustomerEmail}",
                    nameof(GenerateDirectDebitMandateForCustomerAsync), customer.Email);

                await _docGeneratorService.GenerateDocumentAsync(customer, DocumentNames.DirectDebitMandate.ToString(),
                    log, _blobContainerClientService.BlobContainerClient, _wwwRootDirectory);
            }

            return true;
        }
    }
}