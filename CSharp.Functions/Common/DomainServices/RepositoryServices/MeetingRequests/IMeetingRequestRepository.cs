using Common.Entities;

namespace Common
{
    public interface IMeetingRequestRepository
    {
        Task<MeetingRequests> SaveMeetingRequestAsync(MeetingRequests meetingRequest);

        Task<bool> CheckAndUpdateMeetingRequestIfFoundAsync(string calendlyEmail, string customerEmail,
            string eventTypeId, long startDateTimeEpoch);
    }
}