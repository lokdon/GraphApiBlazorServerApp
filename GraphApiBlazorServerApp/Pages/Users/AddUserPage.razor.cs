using GraphApiBlazorServerApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI.Areas.MicrosoftIdentity.Pages.Account;
using Radzen;
using System.Diagnostics;

namespace GraphApiBlazorServerApp.Pages.Users
{
    public partial class AddUserPage :ComponentBase
    {
        AddUserModel model { get; set; }

        [Inject]
        NotificationService NotificationService { get; set; }

        [Inject]
        GraphServiceClient GraphServiceClient { get; set; }

        [Inject]
        MicrosoftIdentityConsentAndConditionalAccessHandler ConsentHandler { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            model = new AddUserModel();
           
        }

        async Task SubmitAsync(AddUserModel arg)
        {
            var x = model;
            
            try
            {
                var requestBody = new User
                {
                    AccountEnabled = true,
                    DisplayName = model.FirstName + " " + model.LastName,
                    MailNickname = model.Email.Split("@")[0],
                    Mail = model.Email,
                    UserPrincipalName = model.Email,
                    PasswordProfile = new PasswordProfile
                    {
                        ForceChangePasswordNextSignIn = true,
                        Password = "xWwvJ]6NMw+bWH-d",
                    },
                };
                var result = await GraphServiceClient.Users.Request().AddAsync(requestBody);
                AddUserNotification();
            }
            catch (Exception ex)
            {
                ShowExceptionNotification();
                ConsentHandler.HandleException(ex);
            }
            //  StateHasChanged();
        }

        void OnInvalidSubmit(FormInvalidSubmitEventArgs args)
        {
            var data = args;
        }

        void Cancel()
        {
            //
        }


        void AddUserNotification()
        {

            ShowNotification(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
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
