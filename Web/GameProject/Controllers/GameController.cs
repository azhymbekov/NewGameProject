using System;
using System.Collections.Generic;
using System.Linq;
using GameProject.Extensions;
using GameProject.Features;
using GameProject.Service.Common.GameService;
using GameProject.Service.Common.WordService.Models;
using Microsoft.AspNetCore.Mvc;

namespace GameProject.Controllers
{
    public class GameController : Controller
    {
        private readonly IGameService gameService;

        public GameController(IGameService gameService)
        {
            this.gameService = gameService;
        }

        public IActionResult Index()
        {
            var words = gameService.GetRandomWords();
            if (!words.Any())
            {
                return View("_ErrorView");
            }
            HttpContext.Session.SetObject(this.User.Identity.Name + "Word", words);
            var word = gameService.GetSecretWord(words.First());
            return View(word);
        }
        
        [HttpGet]
        public JsonResult CheckWord(string secretWord, string wordOrLetter)
        {
            var userId = User.GetUserId();
            try
            {
                //Использую сессии , что бы каждый раз не обращаться в БД
                var words = HttpContext.Session.GetObject<IEnumerable<WordModel>>(this.User.Identity.Name + "Word");
                var word = gameService.CheckWord(ref words, secretWord, wordOrLetter, userId);
                if (word == null)
                {
                    return Json(new
                    {
                        gameOver = true,
                        isWin = true
                    });
                }

                HttpContext.Session.SetObject(this.User.Identity.Name + "Word", words);

                return Json(new
                {
                    gameOver = false,
                    model = new
                    {
                        secretWord = word.SecretWord,
                        tryCount = word.TryCount,
                        question = word.Question
                    }
                });
            }
            catch (ArgumentOutOfRangeException)
            {
                return Json(new
                {
                    gameOver = true,
                    isWin = false
                });
            }
        }

        [HttpGet]
        public IActionResult Results(bool isWin)
        {
            var id = User.GetUserId();
            if (isWin)
            {
                ViewBag.Result = "Поздравляем! Вы победили!";
            }
            else
            {
                ViewBag.Result = "Печально! Вы проиграли!";
            }
            return View();
        }
    }
}