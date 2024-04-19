using Common.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Api
{
    public partial class Endpoints
    {
        //All about Emailing Service

        [FunctionName(nameof(SendEmailAsync))]
        public async Task<IActionResult> SendEmailAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var emailMessage = JsonConvert.DeserializeObject<EmailMessage>(requestBody);
            var recipientEmail = emailMessage.Recipients.FirstOrDefault();
            var isSuccess = _emailService.SendEmail(recipientEmail, emailMessage.Subject, emailMessage.Body);
            return new OkObjectResult(isSuccess);
        }
    }
}