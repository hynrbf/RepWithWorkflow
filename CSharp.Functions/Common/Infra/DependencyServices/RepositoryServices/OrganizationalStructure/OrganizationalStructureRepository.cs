using Common.Entities;
using Microsoft.Azure.Cosmos;

namespace Common.Infra
{
    public class OrganizationalStructureRepository : RepositoryBase, IOrganizationalStructureRepository
    {
        private const string OrganizationalEmployeesContainer = "OrganizationalEmployeesContainer";

        private readonly Container _container;

        public OrganizationalStructureRepository() : base(OrganizationalEmployeesContainer)
            => _container = Client.GetContainer(DatabaseName, ContainerName);

        public async Task<IEnumerable<Employee>> GetEmployeesAsync()
        {
            const string queryText = $"SELECT * FROM c";
            var query = _container.GetItemQueryIterator<Employee>(new QueryDefinition(queryText));
            var results = new List<Employee>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync(string companyNumber)
        {
            if (string.IsNullOrEmpty(companyNumber))
            {
                throw new NullReferenceException(
                    $"The company no should have a value in {nameof(OrganizationalStructureRepository)}.{nameof(OrganizationalStructureRepository)}");
            }

            var queryText = $"SELECT * FROM c WHERE c.CompanyNo = '{companyNumber}'";
            var query = _container.GetItemQueryIterator<Employee>(new QueryDefinition(queryText));
            var results = new List<Employee>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesNotYetFinishedSignupAsync()
        {
            var query = _container.GetItemQueryIterator<Employee>(
                new QueryDefinition($"SELECT * FROM c WHERE c.IsFinishedSignUp = false"));
            var results = new List<Employee>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<Employee?> GetEmployeeByEmailAsync(string email)
        {
            var query = _container.GetItemQueryIterator<Employee>(
                new QueryDefinition($"SELECT * FROM c WHERE c.Email = '{email}'"));
            var results = new List<Employee>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results.FirstOrDefault();
        }

        public async Task<Employee> SaveOrUpdateEmployeeAsync(Employee employee)
        {
            if (employee == null)
            {
                throw new NullReferenceException(
                    $"The employee should not be null in {nameof(OrganizationalStructureRepository)}.{nameof(SaveOrUpdateEmployeeAsync)}");
            }

            if (string.IsNullOrEmpty(employee.CompanyNo))
            {
                throw new NullReferenceException(
                    $"The company no should have a value in {nameof(OrganizationalStructureRepository)}.{nameof(SaveOrUpdateEmployeeAsync)}");
            }

            if (string.IsNullOrEmpty(employee.Email))
            {
                throw new NullReferenceException(
                    $"The email should have value in {nameof(OrganizationalStructureRepository)}.{nameof(SaveOrUpdateEmployeeAsync)}");
            }

            var existingEmployee = await GetEmployeeByEmailAsync(employee.Email);

            if (existingEmployee != null)
            {
                employee.Id = existingEmployee.Id;
                employee.TempPassword = existingEmployee.TempPassword;
            }
            else
            {
                employee.TempPassword = PasswordGenerator.Generate(8, 1);
            }

            //This is a dirty hack because the front end is sending underscrore and component is hard to change
            if (!string.IsNullOrEmpty(employee?.ContactNumber?.Number))
            {
                employee.ContactNumber.Number = employee.ContactNumber.Number.Replace("_", "");
            }

            var schemaModelResponse =
                await _container.UpsertItemAsync(employee, new PartitionKey(employee.Id));
            return schemaModelResponse.Resource;
        }

        public Task SaveBulkEmployeesAsync(List<Employee> employees)
        {
            var tasks = (from employee in employees
                where !string.IsNullOrEmpty(employee.CompanyNo)
                select _container.UpsertItemAsync(employee, new PartitionKey(employee.Id))).ToList();
            return Task.WhenAll(tasks);
        }

        public async Task<bool> DeleteEmployeeAsync(string companyNo)
        {
            var existingRecords = (await GetEmployeesAsync(companyNo)).ToList();

            if (!existingRecords.Any())
            {
                return false;
            }

            var deleteTasks = existingRecords.Select(record =>
                _container.DeleteItemAsync<Customer>(record.Id, new PartitionKey(record.Id))).ToList();
            await Task.WhenAll(deleteTasks);
            return true;
        }
    }
}