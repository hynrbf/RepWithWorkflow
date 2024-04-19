using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using Common;
using Common.Infra;
using Common.Entities;

namespace ApiSpecialCase
{
    public class Endpoints
    {
        private readonly IMeetingRequestRepository _meetingRequestRepository;
        private readonly string _azureAccountName;

        public Endpoints(IMeetingRequestRepository meetingRequestRepository)
        {
            _meetingRequestRepository = meetingRequestRepository;

            _azureAccountName = Environment.GetEnvironmentVariable("AzureAccount", EnvironmentVariableTarget.Process);
        }

        [FunctionName(nameof(GetHeartBeat))]
        public IActionResult GetHeartBeat(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0.0";
            var workingUrl = $"{req.Scheme}://{req.Host}";
            var workingEnvironmentApi = "";

            if (workingUrl.ToLower().Contains("localhost"))
            {
                workingEnvironmentApi = "dev";
            }
            else if (workingUrl.ToLower().Contains("az-func-api-spc-sun.azurewebsites.net"))
            {
                workingEnvironmentApi = "staging";
            }
            else if (workingUrl.ToLower().Contains("az-func-api-spc-sun-live.azurewebsites.net"))
            {
                workingEnvironmentApi = "LIVE";
            }

            if (string.IsNullOrEmpty(workingEnvironmentApi))
            {
                throw new NullReferenceException("The working environment should have a value");
            }

            var dbName = Environment.GetEnvironmentVariable("Database", EnvironmentVariableTarget.Process);

            if (string.IsNullOrEmpty(dbName))
            {
                throw new NullReferenceException("The dbName should have a value");
            }

            return new OkObjectResult(
                $"Hello I'm alive. My api version is {version}. Hosted in '{_azureAccountName}'. It is connected to '{workingEnvironmentApi}' api and connected to '{dbName}' db.");
        }

        [FunctionName(nameof(CalendlyCallbackAsync))]
        public async Task<IActionResult> CalendlyCallbackAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var webhookPayload = JsonConvert.DeserializeObject<CalendlyWebhookPayloadResultK>(requestBody);

            if (webhookPayload?.Payload?.ScheduledEvent?.Created == null ||
                string.IsNullOrEmpty(webhookPayload.Payload?.Email))
            {
                return new OkResult();
            }

            var scheduledEventUrl = webhookPayload.Payload.EventUrl;
            var scheduledEventId = GetUrlLastSegment(scheduledEventUrl);
            var eventTypeId = GetUrlLastSegment(webhookPayload.Payload.ScheduledEvent.EventTypeUrl);

            var meetingRequest = new MeetingRequests
            {
                Id = scheduledEventId,
                EventTypeId = eventTypeId,
                EventUrl = scheduledEventUrl,
                CancelUrl = webhookPayload.Payload.cancelUrl,
                EventName = webhookPayload.Payload.ScheduledEvent.Name,
                Status = webhookPayload.Payload.Status,
                Created = webhookPayload.Created,
                StartInEpoch = DateHelper.ConvertDateTimeToEpoch(webhookPayload.Payload.ScheduledEvent.StartTime),
                EndInEpoch = DateHelper.ConvertDateTimeToEpoch(webhookPayload.Payload.ScheduledEvent.EndTime),
                LocationType = webhookPayload.Payload.ScheduledEvent.Location?.Type ?? "",
                LocationOrContactNo = webhookPayload.Payload.ScheduledEvent.Location?.Detail ?? "",
                TimeZone = webhookPayload.Payload.TimeZone,
                EmailUsedForCalendly = webhookPayload.Payload.Email,
                DisplayName = webhookPayload.Payload.DisplayName
            };

            var message = meetingRequest.Status == "active" ? "Creating" : "Canceling";
            log.LogWarning(
                $"{message} meeting with {meetingRequest.DisplayName} ({meetingRequest.EmailUsedForCalendly}) ...");
            var result = await _meetingRequestRepository.SaveMeetingRequestAsync(meetingRequest);
            return new OkObjectResult(result);
        }

        private static string GetUrlLastSegment(string url)
        {
            var startIndex = url.LastIndexOf('/') + 1;
            var length = url.Length - startIndex;
            var result = url.Substring(startIndex, length);
            return result;
        }
    }
}