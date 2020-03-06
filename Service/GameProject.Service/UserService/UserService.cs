using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GameProject.Common;
using GameProject.Data.Models.Users;
using GameProject.Service.Common;
using GameProject.Service.Common.UserService;
using GameProject.Service.Common.UserService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectGame.Data.Common.Repositories;

namespace GameProject.Service.UserService
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;
        private readonly ILogger<UserService> logger;

        public UserService(IUnitOfWork unitOfWork, UserManager<User> userManager, ILogger<UserService> logger, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<IEnumerable<UserModel>> GetUsersAsync()
        {
            var users =  await userManager.GetUsersInRoleAsync(GlobalConstants.Roles.Player);
            return users.Select(x => mapper.Map<UserModel>(x));
        }

        public async Task RemoveAsync(Guid id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user != null)
            {
                await userManager.DeleteAsync(user);
                await unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<OperationResult> CreateAsync(UserModel model)
        {
            var result = new OperationResult();
            try
            {
                var user = await userManager.Users.FirstOrDefaultAsync(x => x.UserName == model.UserName);

                if (user == null)
                {
                    var newUser = mapper.Map<User>(model);
                    newUser.Id = Guid.NewGuid();


                    var registerResult = await userManager.CreateAsync(newUser, model.Password);


                    if (registerResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newUser, GlobalConstants.Roles.Player);
                        result.Succeeded = true;
                    }
                }
                else
                {
                    result.Message = "Игрок с таким логином уже существует";
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception: {ex.GetType().ToString()}; Сообщение об ошибке: {ex.Message}; StackTrace: {ex.StackTrace}");
                result.Message = "Ошибка при добавлении игрока";
            }
            

            return result;
        }

        public async Task PrepeareUserForEditAsync(UserModel model)
        {
            try
            {
                var user = await userManager.FindByIdAsync(model.Id.ToString());
                mapper.Map(model, user);

                await userManager.UpdateAsync(user);
                await unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception: {ex.GetType()}; Сообщение об ошибке: {ex.Message}; StackTrace: {ex.StackTrace}");
            }
           
        }

        public async Task<UserModel> PrepeareUserForEditViewAsync(Guid? id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            return mapper.Map<UserModel>(user);
        }


        public async Task<UserInfoModel> DetailsAsync(Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new Exception("Нет такого пользователя");
            }
            var matches = from u in userManager.Users
                          join match in unitOfWork.Match.All() on u.Id equals match.UserId
                          join word in unitOfWork.Word.All() on match.WordId equals word.Id
                          where u.Id == userId
                          group match by match.Result
                into grp
                          select grp;

            var userInfoModel = new UserInfoModel { FullName = user.FullName };
            foreach (var match in matches)
            {
                if (match.Key)
                {
                    userInfoModel.CountOfWin = match.Count();
                }
                else
                {
                    userInfoModel.CountOfLose = match.Count();
                }
            }

            await unitOfWork.SaveChangesAsync();

            userInfoModel.GameCount = userInfoModel.CountOfWin + userInfoModel.CountOfLose;

            return userInfoModel;
        }

        public async Task<IEnumerable<UserInfoModel>> GetUsersRatingAsync()
        {
            var users = await userManager.GetUsersInRoleAsync(GlobalConstants.Roles.Player);
            return users.Select(x => mapper.Map<UserInfoModel>(x)).OrderByDescending(x=>x.Rating);
        }
    }
}
