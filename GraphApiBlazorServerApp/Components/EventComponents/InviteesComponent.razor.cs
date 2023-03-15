using GraphApiBlazorServerApp.GraphBrokers.CalendarBrokers;
using GraphApiBlazorServerApp.GraphBrokers.EmailBrokers;
using GraphApiBlazorServerApp.GraphBrokers.EventBrokers;
using GraphApiBlazorServerApp.GraphBrokers.UserBrokers;
using GraphApiBlazorServerApp.Models;
using GraphApiBlazorServerApp.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.Graph;
using Microsoft.IdentityModel.Tokens;
using Radzen.Blazor;
using System.Collections.Generic;

namespace GraphApiBlazorServerApp.Components.EventComponents
{
    public partial class InviteesComponent:ComponentBase
    {
        [Inject]
        ICalendarBroker CalendarBroker { get; set; }

        [Inject]
        IEventBroker eventBroker { get; set; }

        [Inject]
        IUserGraphBroker UserGraphBroker { get; set; }

        [Inject]
        IEmailBroker emailBroker { get; set; }

        List<CustomCalendarDropDownData> calendarDropDownList;

        List<CustomEventDropDown> eventDropDownList;

        List<CustomUserDropDownData> userDropDownData;

        IEnumerable<UserBasicData> userList;
        RadzenDataGrid<UserBasicData>? grid;

        List<UserBasicData> tempUserList;

        List<UserBasicData> allTenantUserData;

        UserBasicData selectedUser = null;

        string selectedEventId =string.Empty;
        string selectedCalendarId = string.Empty;

        protected override void OnInitialized()
        {
            calendarDropDownList = new List<CustomCalendarDropDownData>();
            eventDropDownList = new List<CustomEventDropDown>();
            userDropDownData = new List<CustomUserDropDownData>();
            tempUserList = new List<UserBasicData>();
            allTenantUserData = new List<UserBasicData>();
        }

        protected override async Task OnInitializedAsync()
        {
            allTenantUserData = await UserGraphBroker.GetAllUsersInTenatAsync();
            CreateUserDropDown(allTenantUserData);

            var calendars = await CalendarBroker.GetMeCalendarAsync();
            CreateCalendarDropDown(calendars);
        }

        void CreateCalendarDropDown(List<MyCalendarModel> calendarList)
        {
            calendarDropDownList.Clear();
            foreach (var item in calendarList)
            {
                var custommodel = new CustomCalendarDropDownData();
                custommodel.Id = item.CalendarId;
                custommodel.Name = item.CalendarName;
                calendarDropDownList.Add(custommodel);
            }
        }


        void CreateEventDropDown(List<MyEventModel> eventList)
        {
            eventDropDownList = new List<CustomEventDropDown>();
            foreach (var item in eventList) 
            {
                var eventModel = new CustomEventDropDown();
                eventModel.Id = item.EventId;
                eventModel.Name = item.Subject;
                eventDropDownList.Add(eventModel);  
            }
        }

        void CreateUserDropDown(List<UserBasicData> userDataList)
        {
            userDropDownData = new List<CustomUserDropDownData>();
            foreach (var user in userDataList)
            {
                var userModel = new CustomUserDropDownData();
                userModel.Id = user.Id;
                userModel.Name = user.DisplayName;
                userDropDownData.Add(userModel);
            }
        }

        void OnAddButtonClicked()
        {
            if (!tempUserList.Contains(selectedUser))
            {
                tempUserList.Add(selectedUser);
            }

            userList = tempUserList;
            grid!.Reload();
            StateHasChanged();
        }

        void RemoveUserFromList(UserBasicData user)
        {
            tempUserList = userList.ToList();
            
            if (tempUserList != null) 
            {
                if (tempUserList.Contains(user)) 
                { 
                    tempUserList.Remove(user);
                }
            }

            userList = tempUserList.ToList();
            StateHasChanged(); 
        }

        async Task OnCalendarDropDownChanged(object value)
        {
            selectedCalendarId = (string)value;

            var events = await eventBroker.GetAllEventsByCalendarIdAsync(selectedCalendarId);
            CreateEventDropDown(events);
            await InvokeAsync(StateHasChanged);
        }

        void OnEventDropDownChanged(object value)
        {
            selectedEventId = (string)value;
        }
        void OnUserDropDownChanged(object value)
        {
            string userId = (string)value;
            selectedUser = allTenantUserData.Where(i => i.Id == userId).FirstOrDefault()!;
        }

        async Task SendMailToInviteesAsync()
        {

            if (userList.Count() == 0)
                return;
            
            //update event with invitees
             var myevent = await eventBroker.GetEventByEventIdAsync(selectedEventId, selectedCalendarId);
            CreateInviteesList(myevent);

            await eventBroker.UpdateEventByEventIdAsync(myevent);
            //send the mail to the invitees
            await emailBroker.SendEmailAsync(myevent.Subject,myevent.Body.Content,userList.ToList().Select(i=>i.Email).ToList(),string.Empty);
        }


        void CreateInviteesList(Event graphEvent)
        {
            var attendees = new List<Attendee>();
            foreach (var atte in userList)
            {
                var model = new Attendee();
                model.EmailAddress = new EmailAddress() { Address = atte.Email, Name = atte.DisplayName };
                model.Type = AttendeeType.Required;
                attendees.Add(model);
            }
            
            graphEvent.Attendees = attendees;
        }

    }
}
