using Microsoft.AspNetCore.Components;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using System.Net.NetworkInformation;

using System.Diagnostics;
using Radzen.Blazor;
using Radzen;
using GraphApiBlazorServerApp.Models;
using System.Runtime.InteropServices;

namespace GraphApiBlazorServerApp.Pages.Users
{
    public partial class UserPageByIQuerable : ComponentBase
    {

        [Inject]
        GraphServiceClient GraphServiceClient { get; set; }

        [Inject]
        MicrosoftIdentityConsentAndConditionalAccessHandler ConsentHandler { get; set; }

        [Inject] public DialogService DialogService { get; set; }

        IEnumerable<UserBasicData> userList;
        RadzenDataGrid<UserBasicData>? grid;

        string CategoryName = string.Empty;
        //int categoryID = 0;
        string pagingSummaryFormat = "Page {0} of {1} (total {2} records)";
        IEnumerable<int> pageSizeOptions = new int[] { 10, 20, 30 };
        bool showPagerSummary = true;

        IGraphServiceUsersCollectionPage users = null;


        protected override async Task OnInitializedAsync()
        {
            await InitializeUserDataAsync();
        }



        async Task InitializeUserDataAsync()
        {
            var tempList = new List<UserBasicData>();
            try
            {
                users = await GraphServiceClient.Users.Request().GetAsync();

                if (users != null)
                {

                    foreach (var user in users)
                    {
                        var tempuser = new UserBasicData();
                        tempuser.Id = user.Id;
                        tempuser.Email = user.Mail;
                        tempuser.UserPrincipalName = user.UserPrincipalName;
                        tempuser.DisplayName = user.DisplayName;

                        tempList.Add(tempuser);
                    }
                }
            }
            catch (Exception ex)
            {
                ConsentHandler.HandleException(ex);
            }

            userList = tempList;
        }

        async Task OpenAddUserDialogBoxAsync()
        {

        }

        async Task GetUserCountAsync()
        {


            var queryOptions = new List<QueryOption>()
                                {
                                new QueryOption("$count", "true")
                                };
            var users = await GraphServiceClient.Users
            .Request(queryOptions)
            .Header("ConsistencyLevel", "eventual")
            .Filter("endsWith(mail,'@5pwyw5.onmicrosoft.com')")
            .GetAsync();

        }

        void RowRender(RowRenderEventArgs<UserBasicData> args)
        {
            // args.Expandable = (args.Data.LinkModel.Id == rowIdToExpand);
            //args.Expandable = args.Data.ShipCountry == "France" || args.Data.ShipCountry == "Brazil";
        }

        void RowCollapse(UserBasicData model)
        {
           
        }
    }
}
