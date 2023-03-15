using GraphApiBlazorServerApp.GraphBrokers.UserBrokers;
using GraphApiBlazorServerApp.Models;
using Microsoft.Graph;
using Microsoft.Identity.Web;

namespace GraphApiBlazorServerApp.GraphBrokers.CalendarBrokers
{
    [AuthorizeForScopes(Scopes =new string[] { "Calendars.Read", "Calendars.ReadWrite" })]
    public class CalendarBroker : ICalendarBroker
    {

        private readonly GraphServiceClient _graphServiceClient;
        MicrosoftIdentityConsentAndConditionalAccessHandler _consentHandler;
        private readonly IUserGraphBroker _userBroker;
        public CalendarBroker(GraphServiceClient graphServiceClient,
                    MicrosoftIdentityConsentAndConditionalAccessHandler consentHandler,
                    IUserGraphBroker userBroker)
        {
            _graphServiceClient = graphServiceClient;
            _consentHandler = consentHandler;
            _userBroker = userBroker;

        }


        public async Task<Calendar> CreateCalendarAsync(string name,string userId)
        {
            var requestBody = new Calendar
            {
                Name = name,
            };

            var result =await _graphServiceClient.Users[userId].Calendars.Request().AddAsync(requestBody);
            return result;
        }

        public async Task<Calendar> CreateMeCalendarAsync(string name)
        {
            var requestBody = new Calendar
            {
                Name = name,
            };

            var result = await _graphServiceClient.Me.Calendars.Request().AddAsync(requestBody);
            return result;
        }

        public async Task<List<MyCalendarModel>> GetCalendarByUserIdAsync(string userId)
        {
            IUserCalendarsCollectionPage calendarsCollections;
            var CalendarList = new List<MyCalendarModel>();
            var userModel = await _userBroker.GetUserByIdAsync(userId);
            string owner = userModel.Mail;
            try
            {
                calendarsCollections = await _graphServiceClient.Users[userId].Calendars.Request().GetAsync();
                foreach (var calendar in calendarsCollections)
                {
                    var myCalendar = new MyCalendarModel();
                    myCalendar.CalendarId= calendar.Id;
                    myCalendar.CalendarName = calendar.Name;
                    myCalendar.Owner = owner;
                    CalendarList.Add(myCalendar);
               }
            }
            catch(Exception ex)
            {
                _consentHandler.HandleException(ex);    
            }

           
            return CalendarList;
        }


        public async Task<List<MyCalendarModel>> GetMeCalendarAsync()
        {
            IUserCalendarsCollectionPage calendarsCollections;
            var CalendarList = new List<MyCalendarModel>();
            try
            {
                calendarsCollections = await _graphServiceClient.Me.Calendars.Request().GetAsync();
                foreach (var calendar in calendarsCollections)
                {
                    var myCalendar = new MyCalendarModel();
                    myCalendar.CalendarId = calendar.Id;
                    myCalendar.CalendarName = calendar.Name;
                    myCalendar.Owner = calendar.Owner.Address;
                    CalendarList.Add(myCalendar);
                }
            }
            catch (Exception ex)
            {
                _consentHandler.HandleException(ex);
            }


            return CalendarList;
        }

        public async Task<List<MyCalendarModel>> GetCalendarByUserEmailAsync(string email)
        {
            IUserCalendarsCollectionPage calendarsCollections;
            var CalendarList = new List<MyCalendarModel>();
            //var userModel = await _userBroker.GetUserByEmailAsync(email);
            //string owner = userModel.Mail;
            try
            {
                calendarsCollections = await _graphServiceClient.Me.Calendars.Request().GetAsync();
                foreach (var calendar in calendarsCollections)
                {
                    var myCalendar = new MyCalendarModel();
                    myCalendar.CalendarId = calendar.Id;
                    myCalendar.CalendarName = calendar.Name;
                    myCalendar.Owner = email;
                    CalendarList.Add(myCalendar);
                }
            }
            catch (Exception ex)
            {
                _consentHandler.HandleException(ex);
            }


            return CalendarList;
        }
    }
}
