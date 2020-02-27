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
using ProjectGame.Data.Common.Repositories;

namespace GameProject.Service.UserService
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;

        public UserService(IUnitOfWork unitOfWork, UserManager<User> userManager, RoleManager<Role> roleManager, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.mapper = mapper;
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
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.UserName == model.UserName);
            var result = new OperationResult();
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

            return result;
        }

        public async Task PrepeareUserForEditAsync(UserModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id.ToString());

            user.UserName = model.UserName;
            user.FullName = model.FullName;

            await userManager.UpdateAsync(user);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<UserModel> PrepeareUserForEditViewAsync(Guid? id)
        {
            var user = (from u in userManager.Users
                where u.Id == id
                select new UserModel
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    FullName = u.FullName
                }).FirstOrDefault();
            return user;
          
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

            userInfoModel.GameCount = userInfoModel.CountOfWin + userInfoModel.CountOfLose;

            return userInfoModel;
        }
    }
}
