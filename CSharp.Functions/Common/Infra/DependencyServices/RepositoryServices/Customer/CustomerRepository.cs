using Common.Entities;
using Microsoft.Azure.Cosmos;

namespace Common.Infra
{
    public class CustomerRepository : RepositoryBase, ICustomerRepository
    {
        private const string CustomerContainer = "CustomerContainer";

        private readonly Container _container;

        private bool _isDisabledCheckingForMultipleSignUpsForSingleCompany;

        public CustomerRepository() : base(CustomerContainer)
            => _container = Client.GetContainer(DatabaseName, ContainerName);

        public async Task<IEnumerable<Customer>> GetCustomersAsync()
        {
            var query = _container.GetItemQueryIterator<Customer>(new QueryDefinition("SELECT * FROM c"));
            var results = new List<Customer>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<Customer?> GetCustomerByEmailAsync(string email)
        {
            var query = _container.GetItemQueryIterator<Customer>(
                new QueryDefinition($"SELECT * FROM c WHERE c.Email = '{email}'"));
            var results = new List<Customer>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results.FirstOrDefault();
        }

        public async Task<Customer> SaveCustomerAsync(Customer customer)
        {
            if (string.IsNullOrEmpty(customer.Email))
            {
                throw new NullReferenceException(
                    $"The email should have value in {nameof(CustomerRepository)}.{nameof(SaveCustomerAsync)}");
            }

            var existingRecord = await GetCustomerByEmailAsync(customer.Email);

            if (existingRecord != null)
            {
                customer.Id = existingRecord.Id;
                customer.TempPassword = existingRecord.TempPassword;
            }
            else
            {
                customer.TempPassword = PasswordGenerator.Generate(8, 1);
            }

            //This is a dirty hack because the front end is sending underscrore and component is hard to change
            if (!string.IsNullOrEmpty(customer?.ContactNumber?.Number))
            {
                customer.ContactNumber.Number = customer.ContactNumber.Number.Replace("_", "");
            }

            var schemaModelResponse =
                await _container.UpsertItemAsync(customer, new PartitionKey(customer.Id));
            return schemaModelResponse.Resource;
        }

        public async Task<IEnumerable<Customer>> GetCustomersForProposalEmailAsync()
        {
            var query = _container.GetItemQueryIterator<Customer>(new QueryDefinition(
                "SELECT * FROM c WHERE c.IsFinishedSignUp = true AND c.IsProposalEmailSent = false AND c.IsInProgressDataInitializing = false"));
            var results = new List<Customer>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<IEnumerable<Customer>> GetCustomersForDataInitAsync()
        {
            var query = _container.GetItemQueryIterator<Customer>(new QueryDefinition(
                "SELECT * FROM c WHERE c.IsFinishedSignUp = true AND IS_NULL(c.FirmDetail) = true AND c.CompanyNumber != '' AND c.FirmReferenceNumber != '' AND c.IsInProgressDataInitializing = false AND c.IsInProgressProposal = false AND c.IsInProgressProposalFollowup = false AND c.IsInProgressDirectDebit = false AND c.IsInProgressDirectDebitFollowup = false AND c.IsGeneratingSigningLink = false AND c.IsGeneratingDirectDebitSigningLink = false"));
            var results = new List<Customer>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<IEnumerable<Customer>> GetCustomersNotSignedForProposalEmailAsync()
        {
            var query = _container.GetItemQueryIterator<Customer>(new QueryDefinition(
                "SELECT * FROM c WHERE c.IsFinishedSignUp = true AND c.IsProposalEmailSent = true AND c.IsProposalDocumentSigned = false AND c.IsProposalDocumentRejected = false AND c.IsInProgressDataInitializing = false"));
            var results = new List<Customer>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<IEnumerable<Customer>> GetCustomersForDirectDebitEmailAsync()
        {
            var query = _container.GetItemQueryIterator<Customer>(new QueryDefinition(
                "SELECT * FROM c WHERE c.IsFinishedSignUp = true AND c.IsProposalDocumentSigned = true AND c.IsDirectDebitEmailSent = false AND NOT IS_NULL(c.FirmRepresentativeDetail) AND c.IsInProgressDataInitializing = false"));
            var results = new List<Customer>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<IEnumerable<Customer>> GetCustomersForDirectDebitFollowupAsync()
        {
            var query = _container.GetItemQueryIterator<Customer>(new QueryDefinition(
                "SELECT * FROM c WHERE c.IsDirectDebitEmailSent = true AND c.IsDirectDebitDocumentSigned = false AND c.IsDirectDebitFollowupSent = false AND NOT IS_NULL(c.FirmRepresentativeDetail) AND c.IsInProgressDataInitializing = false"));
            var results = new List<Customer>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<IEnumerable<Customer>> GetCustomersNotSignedDirectDebitAsync()
        {
            var query = _container.GetItemQueryIterator<Customer>(new QueryDefinition(
                "SELECT * FROM c WHERE c.IsDirectDebitEmailSent = true AND c.IsDirectDebitDocumentSigned = false AND NOT IS_NULL(c.FirmRepresentativeDetail) AND c.IsInProgressDataInitializing = false"));
            var results = new List<Customer>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<bool> DeleteCustomerByIdEmailAsync(string id, string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new NullReferenceException(
                    $"The email should have value in {nameof(CustomerRepository)}.{nameof(DeleteCustomerByIdEmailAsync)}");
            }

            var existingRecord = await GetCustomerByEmailAsync(email);

            if (existingRecord == null)
            {
                return false;
            }

            var itemResponse = await _container.DeleteItemAsync<Customer>(id, new PartitionKey(id));
            return itemResponse.StatusCode == System.Net.HttpStatusCode.NoContent;
        }

        public async Task<IEnumerable<Customer>> GetColleaguesAsync(string email)
        {
            var customer = await GetCustomerByEmailAsync(email) ?? throw new Exception(
                $"Customer {email} not found!");

            var query = _container.GetItemQueryIterator<Customer>(new QueryDefinition(
                $"SELECT * FROM c WHERE c.id <> '{customer.Id}' and c.CompanyName = '{customer.CompanyName}' AND c.CompanyAddress = '{customer.CompanyAddress}' AND c.IsCompanyNotApplicable = false"));
            var results = new List<Customer>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<bool> CheckIfCompanyHasSignedProposalAlreadyAsync(string companyNumber)
        {
            if (_isDisabledCheckingForMultipleSignUpsForSingleCompany)
            {
                return false;
            }

            var queryStr =
                $"SELECT * FROM c WHERE c.CompanyNumber = '{companyNumber}' and c.IsProposalDocumentSigned = true";
            var query = _container.GetItemQueryIterator<Customer>(new QueryDefinition(queryStr));
            var results = new List<Customer>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results.Any();
        }

        public void Register(bool isDisabledCheckingForMultipleSignUpsForSingleCompany)
        {
            _isDisabledCheckingForMultipleSignUpsForSingleCompany =
                isDisabledCheckingForMultipleSignUpsForSingleCompany;
        }
    }
}