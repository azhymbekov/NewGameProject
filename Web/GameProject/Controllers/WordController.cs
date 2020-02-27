using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameProject.Common;
using GameProject.Service.Common.UserService.Models;
using GameProject.Service.Common.WordService;
using GameProject.Service.Common.WordService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameProject.Controllers
{
    public class WordController : Controller
    {
        private readonly IWordService wordService;

        public WordController(IWordService wordService)
        {
            this.wordService = wordService;
        }

        public IActionResult Index()
        {
            var words = wordService.GetWords() ?? new List<WordModel>();
            return View(words);
        }

        [HttpGet]
        [Authorize(Roles = GlobalConstants.Roles.Administrator)]
        public async Task<IActionResult> Create(Guid? wordId)
        {
            if (wordId.HasValue)
            {
                var userModel = await wordService.PrepeareWordForEditView(wordId);
                ViewBag.IsForEdit = true;
                return this.View(userModel);
            }
            return View("Create");
        }

        [HttpPost]
        [Authorize(Roles = GlobalConstants.Roles.Administrator)]
        public async Task<IActionResult> Create(WordModel model)
        {
            await wordService.SaveAsync(model);
           
            return RedirectToAction("Index", "Word");
        }
       

        [Authorize(Roles = GlobalConstants.Roles.Administrator)]
        public async Task<IActionResult> Remove(Guid id)
        {
            await wordService.RemoveAsync(id);
            return RedirectToAction("Index", "Word");
        }
    }
}
