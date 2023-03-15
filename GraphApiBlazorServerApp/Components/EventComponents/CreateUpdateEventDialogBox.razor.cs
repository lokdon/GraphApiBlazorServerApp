using GraphApiBlazorServerApp.Components.ValidationComponents;
using GraphApiBlazorServerApp.GraphBrokers.CalendarBrokers;
using GraphApiBlazorServerApp.GraphBrokers.EventBrokers;
using GraphApiBlazorServerApp.GraphBrokers.UserBrokers;
using GraphApiBlazorServerApp.Models;
using GraphApiBlazorServerApp.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Graph;
using Microsoft.Identity.Web.UI.Areas.MicrosoftIdentity.Pages.Account;
using Radzen;
using System.Drawing;
using System.Reflection;

namespace GraphApiBlazorServerApp.Components.EventComponents;

public partial class CreateUpdateEventDialogBox : ComponentBase
{
    MyEventModel model;

    [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; }

    [Parameter]
    public string EventId { get; set; }

    [Inject]
    DialogService dialogService { get; set; }

    [Inject]
    NotificationService notificationService { get; set; }

    [Inject]
    IEventBroker EventBroker { get; set; }

    [Inject]
    ICalendarBroker CalendarBroker { get; set; }

    [Inject]
    IUserGraphBroker UserBroker { get; set; }

    [Inject]
    IHttpContextAccessor _contextAccessor { get; set; }

    ServerInputValidator? serverValidator;

    List<ErrorModel> errorList { get; set; }

    Dictionary<string, string> ErrorDictionary { get; set; }

    bool isLoading = false;

    List<CustomCalendarDropDownData> calendarDropDownList;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        model = new MyEventModel();
        calendarDropDownList = new List<CustomCalendarDropDownData>();
    }


    protected override async Task OnInitializedAsync()
    {
        var authState = await authenticationStateTask;
        var user = authState.User;
        var calendatList = await CalendarBroker.GetMeCalendarAsync();

        foreach (var item in calendatList)
        {
            var custommodel = new CustomCalendarDropDownData();
            custommodel.Id = item.CalendarId;
            custommodel.Name = item.CalendarName;
            calendarDropDownList.Add(custommodel);
        }

        // var graphUser = await UserBroker.GetUserByEmailAsync(user.Identity?.Name!);

        //fetch the data from grpahapi
        if (!string.IsNullOrEmpty(EventId))
        {

        }
    }
    void OnInvalidSubmit()
    {
    }

    void Cancel()
    {
        dialogService.Close();
        //Close(null);
    }

    async Task SubmitAsync(MyEventModel model)
    {
        isLoading = true;
        if (string.IsNullOrEmpty(model.EventId))
        {
            await CreateEventAsync();
            dialogService.Close();
            //await LinkDescAddNotifier.NotifyChange(model);
        }
        else
        {
            await UpdateEventAsync();
            //await LinkDescAddNotifier.NotifyChange(model);
        }
        isLoading = false;
        await InvokeAsync(() => StateHasChanged());

    }

    async Task CreateEventAsync()
    {
        await EventBroker.CreateEventAsync(model);
    }

    async Task UpdateEventAsync()
    {

    }

    void OnChange(object value)
    {
        model.CalendarId = (string)value;
    }
}













