using Common.Entities;
using Microsoft.Azure.Cosmos;

namespace Common.Infra
{
    public class MeetingRequestRepository : RepositoryBase, IMeetingRequestRepository
    {
        private const string MeetingRequestContainer = "MeetingRequestContainer";

        private readonly Container _container;

        public MeetingRequestRepository() : base(MeetingRequestContainer)
            => _container = Client.GetContainer(DatabaseName, ContainerName);

        public async Task<MeetingRequests> SaveMeetingRequestAsync(MeetingRequests meetingRequest)
        {
            var meetingRequestsModelResponse =
                await _container.UpsertItemAsync(meetingRequest, new PartitionKey(meetingRequest.Id));
            return meetingRequestsModelResponse.Resource;
        }

        public async Task<bool> CheckAndUpdateMeetingRequestIfFoundAsync(string calendlyEmail, string customerEmail,
            string eventTypeId, long startDateTimeEpoch)
        {
            var found = await GetMeetingRequestAsync(calendlyEmail, eventTypeId, startDateTimeEpoch);

            if (found == null)
            {
                return false;
            }

            // map email
            found.Email = customerEmail;
            var updated = await SaveMeetingRequestAsync(found);
            return updated != null;
        }

        private async Task<MeetingRequests?> GetMeetingRequestAsync(string calendlyEmail, string eventTypeId,
            long startDateTimeEpoch)
        {
            var whereClause =
                $"c.EmailUsedForCalendly = '{calendlyEmail}' AND c.EventTypeId = '{eventTypeId}' AND c.StartInEpoch = {startDateTimeEpoch}";
            var query = _container.GetItemQueryIterator<MeetingRequests>(
                new QueryDefinition($"SELECT * FROM c WHERE {whereClause}"));
            var results = new List<MeetingRequests>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results.FirstOrDefault();
        }
    }
}