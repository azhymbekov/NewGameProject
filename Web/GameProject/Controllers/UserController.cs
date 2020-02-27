using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameProject.Common;
using GameProject.Service.Common.UserService;
using GameProject.Service.Common.UserService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameProject.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await userService.GetUsersAsync() ?? new List<UserModel>();
            return View(user);
        }

        [HttpGet]
        [Authorize(Roles = GlobalConstants.Roles.Administrator)]
        public async Task<IActionResult> Create(Guid? userId)
        {
            if (userId.HasValue)
            {
                var userModel = await userService.PrepeareUserForEditViewAsync(userId);
                ViewBag.IsForEdit = true;
                return this.View(userModel);
            }
            else
            {
                ViewBag.IsForEdit = false;
                return View();
            }
        }


        [HttpGet]
        [Authorize(Roles = GlobalConstants.Roles.Administrator)]
        public async Task<IActionResult> Details(Guid userId)
        {
            var userStatistic = await userService.DetailsAsync(userId);
            return View(userStatistic);
        }

        [HttpPost]
        [Authorize(Roles = GlobalConstants.Roles.Administrator)]
        public async Task<IActionResult> Create(UserModel model)
        {
            if (model.Password != null)
            {
                var result = await userService.CreateAsync(model);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    ViewBag.Result = result.Message;
                    return this.View();
                }
            }
            else
            {
                await userService.PrepeareUserForEditAsync(model);
                return RedirectToAction("Index", "User");
            }
        }

        [Authorize(Roles = GlobalConstants.Roles.Administrator)]
        public async Task<IActionResult> Remove(Guid id)
        {
            await userService.RemoveAsync(id);
            return RedirectToAction("Index", "User");
        }
    }
}
