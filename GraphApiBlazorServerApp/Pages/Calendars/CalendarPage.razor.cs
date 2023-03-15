using FluentValidation;
using GraphApiBlazorServerApp.Components.ValidationComponents;
using GraphApiBlazorServerApp.GraphBrokers.CalendarBrokers;
using GraphApiBlazorServerApp.GraphBrokers.EmailBrokers;
using GraphApiBlazorServerApp.GraphBrokers.UserBrokers;
using GraphApiBlazorServerApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI.Areas.MicrosoftIdentity.Pages.Account;
using Radzen;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;


namespace GraphApiBlazorServerApp.Pages.Calendars
{
    public partial class CalendarPage : ComponentBase
    {
        MyCalendarModel model;

        [Inject]
        NotificationService NotificationService { get; set; }

        [Inject]
        IUserGraphBroker GraphBroker { get; set; }

        [Inject]
        ICalendarBroker CalendarBroker { get; set; }

        [Inject]
        IValidator<MyCalendarModel> validator { get; set; }

        //[Inject]
        //IValidator<AddUserModel> validator { get; set; }

        string domain = null;

        ServerInputValidator? serverValidator;

        List<MyErrorModel> errorList { get; set; }

        bool isLoading = false;

        Dictionary<string, string> errorDictionary { get; set; }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            model = new MyCalendarModel();
         
            errorList = new List<MyErrorModel>();
            errorDictionary = new Dictionary<string, string>();
        }
        void OnInvalidSubmit(FormInvalidSubmitEventArgs args)
        {
            var data = args;
        }

        async Task SubmitAsync(MyCalendarModel arg)
        {
            isLoading = true;
            await InvokeAsync(() => StateHasChanged());
            
            var validationResults = await validator.ValidateAsync(arg);

            if (validationResults.IsValid)
            {
                var calendar = await CalendarBroker.CreateMeCalendarAsync(arg.CalendarName);
                if (calendar != null)
                {
                    CalendarAddedNotification();
                    model = new MyCalendarModel();
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

        void Cancel()
        {
            //
        }

        void ShowUserAlreadyExists()
        {
            ShowNotification(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = "Duplicate",
                Detail = "User already exists with email",
                Duration = 4000,
            });
        }


        void CalendarAddedNotification()
        {

            ShowNotification(new NotificationMessage
            {
                Severity = NotificationSeverity.Success,
                Summary = "Success",
                Detail = "Calendar Created Successfully",
                Duration = 4000,
            });
        }

        void ShowNotification(NotificationMessage message)
        {
            NotificationService.Notify(message);
        }

        void ShowExceptionNotification()
        {
            ShowNotification(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = "Failure",
                Detail = "Some Exception Occurred",
                Duration = 4000,
            });
        }
    }
}

