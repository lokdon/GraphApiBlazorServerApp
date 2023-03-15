using Microsoft.Graph;
using Microsoft.Identity.Web;

namespace GraphApiBlazorServerApp.GraphBrokers.OnlineMeetingBrokers
{
    public class OnlineMeetingBroker: IOnlineMeetingBroker
    {
        private readonly GraphServiceClient _graphServiceClient;
        MicrosoftIdentityConsentAndConditionalAccessHandler _consentHandler;
        public OnlineMeetingBroker(GraphServiceClient graphServiceClient,
                    MicrosoftIdentityConsentAndConditionalAccessHandler consentHandler
                    )
        {
            _graphServiceClient = graphServiceClient;
            _consentHandler = consentHandler;

        }
        public async Task<OnlineMeeting> CreateOnlineMeeting(
     OnlineMeeting onlineMeeting)
        {
            return await _graphServiceClient.Me
                .OnlineMeetings
                .Request()
                .AddAsync(onlineMeeting);
        }

        public async Task<OnlineMeeting> UpdateOnlineMeeting(
            OnlineMeeting onlineMeeting)
        {
            return await _graphServiceClient.Me
                .OnlineMeetings[onlineMeeting.Id]
                .Request()
                .UpdateAsync(onlineMeeting);
        }

        public async Task<OnlineMeeting> GetOnlineMeeting(
            string onlineMeetingId)
        {
            return await _graphServiceClient.Me
                .OnlineMeetings[onlineMeetingId]
                .Request()
                .GetAsync();
        }

    }
}
