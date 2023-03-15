using GraphApiBlazorServerApp.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using System.Globalization;
using System.Reflection;
using System.Security.Claims;

namespace GraphApiBlazorServerApp.GraphBrokers.EventBrokers
{
    public class EventBroker :IEventBroker
    {
        private readonly GraphServiceClient _graphServiceClient;
        MicrosoftIdentityConsentAndConditionalAccessHandler _consentHandler;
        public EventBroker(GraphServiceClient graphServiceClient,
                    MicrosoftIdentityConsentAndConditionalAccessHandler consentHandler
                    )
        {
            _graphServiceClient = graphServiceClient;
            _consentHandler = consentHandler;
            
        }

        public async Task<Event> CreateEventAsync(MyEventModel model)
        {
            var requestBody = new Event
            {
                Subject = model.Subject,
                Body = new ItemBody
                {
                    ContentType = BodyType.Html,
                    Content = model.Content
                },
                Start = new DateTimeTimeZone
                {
                    DateTime = model.StartDateAndTime.ToString(),
                    TimeZone = model.TimeZone,
                },
                End = new DateTimeTimeZone
                {
                    DateTime = model.EndDateAndTime.ToString(),
                    TimeZone = model.TimeZone,
                },
                Location = new Location
                {
                    DisplayName = model.Location,
                },
                TransactionId =model.TransactionId,
            };

            try
            {
                var result = await _graphServiceClient
                               .Me
                               .Calendars[model.CalendarId]
                               .Events
                               .Request()
                               .AddAsync(requestBody);

                return result;
            }
            catch(Exception ex ) 
            { 
              _consentHandler.HandleException( ex );
            }
            return null;
        }

        public async Task<List<MyEventModel>> GetAllEventsByCalendarIdAsync(string calendarId)
        {

            var myEventList = new List<MyEventModel>();

            ICalendarEventsCollectionPage events = await _graphServiceClient
                                                   .Me.Calendars[calendarId]
                                                   .Events
                                                   .Request()
                                                   .GetAsync();


            foreach(var ev in events)
            {
                var tempEv = new MyEventModel();

                tempEv.EventId = ev.Id;
                tempEv.CalendarId = calendarId;
                tempEv.Subject = ev.Subject;
                tempEv.Content = ev.Body.Content;
                tempEv.StartDateAndTime = Convert.ToDateTime(ev.Start.DateTime);
                tempEv.EndDateAndTime = Convert.ToDateTime(ev.End.DateTime);
                myEventList.Add(tempEv);
            }

            return myEventList;
        }

        public async Task<Event> GetEventByEventIdAsync(string eventId, string calendarId)
        {
            var ev = await _graphServiceClient.Me.Events[eventId].Request().GetAsync();

            return ev;
        }

        public async Task<Event> UpdateEventByEventIdAsync(Event model)
        {
            var ev = await _graphServiceClient.Me.Events[model.Id].Request().UpdateAsync(model);
            return ev;
        }
    }
}
