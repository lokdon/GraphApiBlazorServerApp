using Microsoft.Graph;

namespace GraphApiBlazorServerApp.GraphBrokers.OnlineMeetingBrokers
{
    public interface IOnlineMeetingBroker
    {
        public Task<OnlineMeeting> CreateOnlineMeeting(OnlineMeeting onlineMeeting);
        public Task<OnlineMeeting> UpdateOnlineMeeting(OnlineMeeting onlineMeeting);
        public Task<OnlineMeeting> GetOnlineMeeting(string onlineMeetingId);
    }

}
