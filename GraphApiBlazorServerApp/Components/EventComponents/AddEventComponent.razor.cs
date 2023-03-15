using FluentValidation;
using GraphApiBlazorServerApp.Components.ValidationComponents;
using GraphApiBlazorServerApp.GraphBrokers.CalendarBrokers;
using GraphApiBlazorServerApp.GraphBrokers.EventBrokers;
using GraphApiBlazorServerApp.GraphBrokers.OnlineMeetingBrokers;
using GraphApiBlazorServerApp.GraphBrokers.UserBrokers;
using GraphApiBlazorServerApp.Models;
using GraphApiBlazorServerApp.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Graph;
using Microsoft.Identity.Web.UI.Areas.MicrosoftIdentity.Pages.Account;
using Radzen;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GraphApiBlazorServerApp.Components.EventComponents;

public partial class AddEventComponent : ComponentBase
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
    IOnlineMeetingBroker meetingBroker { get; set; }

    [Inject]
    IHttpContextAccessor _contextAccessor { get; set; }

    [Inject]
    IValidator<MyEventModel> validator { get; set; }

    [Inject]
    NotificationService NotificationService { get; set; }

    ServerInputValidator? serverValidator;

    List<MyErrorModel> errorList { get; set; }
    Dictionary<string, string> errorDictionary { get; set; }

    bool isLoading = false;

    bool isGeneratingUrl =false;

    List<CustomCalendarDropDownData> calendarDropDownList;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        model = new MyEventModel();
        model.IsOnline=true;
        calendarDropDownList = new List<CustomCalendarDropDownData>
                                { 
                                    new CustomCalendarDropDownData() {Id="", Name="Select" } 
                                };

        errorList = new List<MyErrorModel>();
        errorDictionary = new Dictionary<string, string>();
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
        await InvokeAsync(() => StateHasChanged());

        var validationResults = await validator.ValidateAsync(model);

        if (validationResults.IsValid)
        {
            if (string.IsNullOrEmpty(model.EventId))
            {
                await CreateEventAsync();
                AddEventSuccessfulNotification();
                model = new MyEventModel();
            }
            else
            {
                await UpdateEventAsync();
            }
           
        }
        else
        {
            errorList = new List<MyErrorModel>();
            errorDictionary.Clear();
            foreach (var validationResult in validationResults.Errors)
            {
                var myErrorModel = new MyErrorModel();
                myErrorModel.FieldName = validationResult.PropertyName;
                myErrorModel.ErrorMessage = validationResult.ErrorMessage;

                errorList.Add(myErrorModel);

                if (!errorDictionary.ContainsKey(validationResult.PropertyName))
                {
                    var key = validationResult.PropertyName;
                    var error = validationResult.ErrorMessage;
                    errorDictionary[key] = error;
                }
            }
            serverValidator!.ValidateApiErrors(errorDictionary, model);
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

    void OnChangeMeetingCheckBox(bool value)
    {
        model.IsOnline = value;
        StateHasChanged();
    }

    void OnChange(object value)
    {
        model.CalendarId = (string)value;
    }

    async Task GenerateMeetingUrl()
    {
        isGeneratingUrl =true;
        await InvokeAsync(StateHasChanged);
        var onlineMeeting = new OnlineMeeting
        {
            StartDateTime = model.StartDateAndTime,
            EndDateTime = model.EndDateAndTime,
            Subject = model.Subject,
            LobbyBypassSettings = new LobbyBypassSettings
            {
                Scope = LobbyBypassScope.Everyone
            }
        };

        var meetingdetails =await meetingBroker.CreateOnlineMeeting(onlineMeeting);
        model.MeetingUrl = meetingdetails.JoinWebUrl;
        isGeneratingUrl = false;
        await InvokeAsync(StateHasChanged);
    }

    void AddEventSuccessfulNotification()
    {

        ShowNotification(new NotificationMessage
        {
            Severity = NotificationSeverity.Success,
            Summary = "Success",
            Detail = "Event added successfully",
            Duration = 4000,
        });
    }

    void ShowNotification(NotificationMessage message)
    {
        NotificationService.Notify(message);
    }
}













