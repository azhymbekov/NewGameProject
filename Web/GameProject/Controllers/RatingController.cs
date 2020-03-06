using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameProject.Common;
using GameProject.Service.Common.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : Controller
    {

        private readonly IUserService userService;

        public RatingController(IUserService userService)
        {
            this.userService = userService;
        }
        [HttpGet("GetRating")]
        public async Task<IActionResult> GetRating()
        {
            var users = await userService.GetUsersRatingAsync();
            return Ok(users);
        }
    }
}