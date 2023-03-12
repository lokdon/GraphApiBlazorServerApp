using FluentValidation;
using GraphApiBlazorServerApp.Components.ValidationComponents;
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
using GraphApiBlazorServerApp.Models;

namespace GraphApiBlazorServerApp.Pages.Users
{
    public partial class AddUserPage :ComponentBase
    {
        AddUserModel model { get; set; }

        [Inject]
        NotificationService NotificationService { get; set; }

        [Inject]
        IConfiguration configuration { get; set; }

        [Inject]
        IUserGraphBroker GraphBroker { get; set; }

        [Inject]
        IEmailBroker EmailBroker { get; set; }

        [Inject]
        IValidator<AddUserModel> validator { get; set; }

        string domain = null;

        ServerInputValidator? serverValidator;

        List<MyErrorModel> errorList { get; set; }

        bool isLoading = false;

        Dictionary<string, string> errorDictionary { get; set; }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            model = new AddUserModel();
            domain ="@"+ configuration["AzureAd:Domain"]!.ToString();
            errorList = new List<MyErrorModel>();
            errorDictionary = new Dictionary<string, string>();
        }

        void OnInvalidSubmit(FormInvalidSubmitEventArgs args)
        {
            var data = args;
        }

        async Task SubmitAsync(AddUserModel arg)
        {

            await SendEmailToUserAsync("AdeleV@5pwyw5.onmicrosoft.com");


            return;




            isLoading = true;
            await InvokeAsync(() => StateHasChanged());

            arg.Email=arg.UserName + "@" + configuration["AzureAd:Domain"]!.ToString();
            var validationResults =await validator.ValidateAsync(arg);

            if (validationResults.IsValid)
            {
                var user = await GraphBroker.AddUserModelAsync(arg);
                var license = await GraphBroker.GetLicensesAsync();
                await GraphBroker.AddLicenseToUserAsync(user.Id, license.SkuId);

                await SendEmailToUserAsync(user.Mail);
                model = new AddUserModel();
                AddUserNotification();
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
        async Task SendEmailToUserAsync(string toAddress)
        {
            string subject = "On board Congratulation";
            string content = "Welcome to the fantastic jobs";
            string fromAddress = "lokesh@5pwyw5.onmicrosoft.com";

           await EmailBroker.SendEmailAsync(subject, content, toAddress, fromAddress);   
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


        void AddUserNotification()
        {

            ShowNotification(new NotificationMessage
            {
                Severity = NotificationSeverity.Success,
                Summary = "Success",
                Detail = "User added successfully",
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
