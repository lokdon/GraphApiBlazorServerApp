using GraphApiBlazorServerApp.GraphBrokers.CalendarBrokers;
using GraphApiBlazorServerApp.GraphBrokers.EventBrokers;
using GraphApiBlazorServerApp.Models;
using GraphApiBlazorServerApp.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.Graph;
using Radzen;
using Radzen.Blazor;
using System.Reflection;

namespace GraphApiBlazorServerApp.Components.EventComponents
{
    public partial class EventDataGridComponent :ComponentBase
    {

        [Inject]
        public IEventBroker eventBroker { get; set; }

        [Inject]
        ICalendarBroker CalendarBroker { get; set; }

        
        IEnumerable<MyEventModel> eventList;
        RadzenDataGrid<MyEventModel>? grid;

        string CategoryName = string.Empty;
        //int categoryID = 0;
        string pagingSummaryFormat = "Page {0} of {1} (total {2} records)";
        IEnumerable<int> pageSizeOptions = new int[] { 10, 20, 30 };
        bool showPagerSummary = true;



        List<CustomCalendarDropDownData> calendarDropDownList;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            calendarDropDownList = new List<CustomCalendarDropDownData>();
        }

        protected override async Task OnInitializedAsync()
        {
            var calendatList = await CalendarBroker.GetMeCalendarAsync();

            foreach (var item in calendatList)
            {
                var custommodel = new CustomCalendarDropDownData();
                custommodel.Id = item.CalendarId;
                custommodel.Name = item.CalendarName;
                calendarDropDownList.Add(custommodel);
            }
        }



        [Inject]
        DialogService dialogService { get; set; }
        //async Task OpenEventDialogBoxAsync()
        //{
        //    await dialogService.OpenAsync<CreateUpdateEventDialogBox>("Event Detials",
        //                                              new Dictionary<string, object>()
        //                                              {

        //                                                   { "EventId", string.Empty }
        //                                              },
        //                                               new DialogOptions()
        //                                               {
        //                                                   Resizable = false,
        //                                                   Draggable = false
        //                                               });
        //}


        async Task OnChange(object value)
        {
            string calendarId = (string)value;

            eventList= await eventBroker.GetAllEventsByCalendarIdAsync(calendarId);

            await InvokeAsync(StateHasChanged);
        }
    }
}
