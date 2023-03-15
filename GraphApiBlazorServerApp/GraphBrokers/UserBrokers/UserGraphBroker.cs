using GraphApiBlazorServerApp.Models;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using System.Reflection;

namespace GraphApiBlazorServerApp.GraphBrokers.UserBrokers
{
    public class UserGraphBroker : IUserGraphBroker
    {
        private readonly GraphServiceClient _graphServiceClient;
        MicrosoftIdentityConsentAndConditionalAccessHandler _consentHandler;

        private readonly IConfiguration _configuration;
        public UserGraphBroker(GraphServiceClient graphServiceClient, 
                    MicrosoftIdentityConsentAndConditionalAccessHandler consentHandler,
                    IConfiguration configuration)
        {
            _graphServiceClient = graphServiceClient;
            _consentHandler = consentHandler;
            _configuration = configuration;
        }
        public async Task<User> AddUserModelAsync(AddUserModel model)
        {
            string domain = _configuration["AzureAd:Domain"]!.ToString();
            User user = null;
            try
            {
                var requestBody = new User
                {
                    AccountEnabled = true,
                    DisplayName = model.FirstName + " " + model.LastName,
                    MailNickname = model.UserName,
                    Mail = model.UserName +"@"+ domain,
                    UserPrincipalName = model.UserName + "@" + domain,
                    UsageLocation = "IN",
                    PasswordProfile = new PasswordProfile
                    {
                        ForceChangePasswordNextSignIn = true,
                        Password = "xWwvJ]6NMw+bWH-d",
                    },
                };
                user = await _graphServiceClient.Users.Request().AddAsync(requestBody);

            }
            catch (Exception ex)
            {
                
                _consentHandler.HandleException(ex);
            }

            return user;
        }
        public async Task<SubscribedSku> GetLicensesAsync()
        {
            var skuResult = await _graphServiceClient.SubscribedSkus.Request().GetAsync();
            return skuResult[0];
        }
        public async Task AddLicenseToUserAsync(string userId, Guid? skuId)
        {
            var licensesToAdd = new List<AssignedLicense>();
            var licensesToRemove = new List<Guid>();

            var license = new AssignedLicense()
            {
                SkuId = skuId,
            };

            licensesToAdd.Add(license);

            try
            {
                await _graphServiceClient.Users[userId]
                        .AssignLicense(licensesToAdd, licensesToRemove)
                        .Request()
                        .PostAsync();
            }
            catch (Exception ex)
            {
                _consentHandler.HandleException(ex);
            }


        }

        public async Task<bool> CheckIfUserAlreadyExistsAsync(string email)
        {
            try
            {
                var user = await _graphServiceClient
                        .Users[email]
                        .Request()
                        .GetAsync();

                if (user != null)
                    return true;
            }
            catch (Exception ex)
            {
                // ShowExceptionNotification();
                // ConsentHandler.HandleException(ex);
                return false;
            }

            return false;
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            try
            {
                var user = await _graphServiceClient
                        .Users[id]
                        .Request()
                        .GetAsync();

                if (user != null)
                    return user;
            }
            catch (Exception ex)
            {
                // ShowExceptionNotification();
                _consentHandler.HandleException(ex);

            }

            return null;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _graphServiceClient
                        .Users[email]
                        .Request()
                        .GetAsync();

                if (user != null)
                    return user;
            }
            catch (Exception ex)
            {
                // ShowExceptionNotification();
                _consentHandler.HandleException(ex);

            }

            return null;
        }


        public async Task<List<UserBasicData>> GetAllUsersInTenatAsync()
        {
            var tempList = new List<UserBasicData>();
            try
            {
                var users = await _graphServiceClient.Users.Request().GetAsync();

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
                _consentHandler.HandleException(ex);
            }

           return tempList;
           
        }
    }
}
