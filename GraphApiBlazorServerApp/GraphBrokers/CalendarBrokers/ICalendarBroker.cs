using GraphApiBlazorServerApp.Models;
using Microsoft.Graph;

namespace GraphApiBlazorServerApp.GraphBrokers.CalendarBrokers
{
    public interface ICalendarBroker
    {
        Task<Calendar> CreateCalendarAsync(string name, string userId);

        Task<Calendar> CreateMeCalendarAsync(string name);

        Task<List<MyCalendarModel>> GetCalendarByUserIdAsync(string userId);

        Task<List<MyCalendarModel>> GetMeCalendarAsync();

        Task<List<MyCalendarModel>> GetCalendarByUserEmailAsync(string email);
    }
}
