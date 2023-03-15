using GraphApiBlazorServerApp.Models;
using Microsoft.Graph;

namespace GraphApiBlazorServerApp.GraphBrokers.UserBrokers
{
    public interface IUserGraphBroker
    {
        public Task<User> AddUserModelAsync(AddUserModel model);

        public Task<SubscribedSku> GetLicensesAsync();

        public Task AddLicenseToUserAsync(string userId, Guid? skuId);

        public Task<bool> CheckIfUserAlreadyExistsAsync(string email);

        public Task<User> GetUserByIdAsync(string id);

        public Task<User> GetUserByEmailAsync(string email);

        public Task<List<UserBasicData>> GetAllUsersInTenatAsync();



    }
}
