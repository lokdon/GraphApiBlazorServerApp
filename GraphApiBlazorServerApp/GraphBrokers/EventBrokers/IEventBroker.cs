using GraphApiBlazorServerApp.Models;
using Microsoft.Graph;

namespace GraphApiBlazorServerApp.GraphBrokers.EventBrokers
{
    public interface IEventBroker
    {
        public Task<Event> CreateEventAsync(MyEventModel model);

        public Task<List<MyEventModel>> GetAllEventsByCalendarIdAsync(string calendarId);
        public Task<Event> GetEventByEventIdAsync(string eventId, string calendarId);

        public Task<Event> UpdateEventByEventIdAsync(Event model);

    }
}
