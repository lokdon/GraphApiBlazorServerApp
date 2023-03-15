using Microsoft.Graph;
using System.Transactions;

namespace GraphApiBlazorServerApp.Models
{
    public class MyEventModel
    {
        
        public string CalendarId { get; set; }  
        public string EventId { get; set; }

        public string Subject { get; set; }
        public string Content { get; set; }
        public DateTime? StartDateAndTime { get; set; }

        public DateTime? EndDateAndTime { get; set; }

        public string Location { get; set; }

        public bool IsOnline { get; set; }

        public string MeetingUrl { get;set; }   

        public List<Attendee> Attendees { get; set; }

        public string TransactionId { get; set; }

        public string TimeZone { get; set; }

        public MyEventModel()
        {
            Attendees= new List<Attendee>();
            TransactionId = Guid.NewGuid().ToString();
            TimeZone = "Pacific Standard Time";
        }

    }
}
