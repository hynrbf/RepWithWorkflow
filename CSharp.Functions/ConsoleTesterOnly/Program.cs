using Common.Infra;

Console.WriteLine("This is for quick testing only");

//var queryStringBase64 = Helpers.EncodeToBase64("email=hynrbf@gmail.com");
//Console.WriteLine(queryStringBase64);

//Testing Customer Repo
//protected RepositoryBase(string containerKey = null, string partitionKey = "/id")
//{
//    DatabaseName = "complydexdbinstance"; //Environment.GetEnvironmentVariable("Database", EnvironmentVariableTarget.Process);
//    ContainerName = "customer"; //Environment.GetEnvironmentVariable(containerKey ?? nameof(ContainerName),
//                                //  EnvironmentVariableTarget.Process);

//    if (_lazyCosmosClient?.Value != null)
//    {
//        return;
//    }

//    var account = "https://complydexdb.documents.azure.com:443/"; //Environment.GetEnvironmentVariable("CosmosDbAccount", EnvironmentVariableTarget.Process);
//    var key = "6aNmZkcXGtZQ9LpjOTn2cImhHBqkDKNa5YOpsyQWR0Oye5775VmbMoIjXPMpgae6UzQN96QHWLJJACDbNFPBMw=="; // Environment.GetEnvironmentVariable("CosmosDbKey", EnvironmentVariableTarget.Process);
//    _lazyCosmosClient = new Lazy<CosmosClient>(GetCosmosClientInstance(account, key));

//    //// Containers are already created in Terraform, so we dont need this one
//    //// Wait to complete for now
//    //CreateContainerIfNotExist().Wait();
//}

//ICustomerRepository c = new CustomerRepository();
//var customers = await c.GetCustomersAsync();

//foreach (var customer in customers)
//{
//    var tt = $"'{customer.TempPassword}'";
//    Console.WriteLine(tt);
//}