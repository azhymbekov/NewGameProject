using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameProject.Service.Common.UserService.Models;

namespace GameProject.Service.Common.UserService
{
    public interface IUserService
    {
        Task<IEnumerable<UserModel>> GetUsersAsync();

        Task RemoveAsync(Guid id);

        Task<OperationResult> CreateAsync(UserModel model);

        Task PrepeareUserForEditAsync(UserModel model);

        Task<UserModel> PrepeareUserForEditViewAsync(Guid? id);

        Task<UserInfoModel> DetailsAsync(Guid userId);
    }
}
