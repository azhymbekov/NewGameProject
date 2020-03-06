using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using GameProject.Data.Models.Game;
using GameProject.Data.Models.Users;
using GameProject.Service.Common.GameService;
using GameProject.Service.Common.WordService.Models;
using Microsoft.AspNetCore.Identity;
using ProjectGame.Data.Common.Repositories;

namespace GameProject.Service.GameService
{
    public class GameService : IGameService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;

        public GameService(
            UserManager<User> userManager,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        public IEnumerable<WordModel> GetRandomWords()
        {
            var words = this.mapper.ProjectTo<WordModel>(unitOfWork.Word.AllAsNoTracking());
            return GenerateRandomWords(words);
        }

        public WordModel GetSecretWord(WordModel model)
        {
            var secretWord = string.Join("", Enumerable.Repeat('*', model.SecretWord.Length).ToArray());
            var sb = new StringBuilder(secretWord);
            secretWord = sb.ToString();
            model.SecretWord = secretWord;
            return model;
        }

        public WordModel CheckWord(ref IEnumerable<WordModel> wordModels, string secretText, string letterOrWord, Guid userId)
        {
            var currentUser = userManager.Users.FirstOrDefault(x => x.Id == userId);
            var words = wordModels.ToList();
            if (words.Count > 0)
            {
                //слово которое нужно угадать
                var word = new WordModel { SecretWord = words[0].SecretWord, TryCount = words[0].TryCount, Question = words[0].Question, Id = words[0].Id};

                //Создается новый матч (игра)
                var match = new Match
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    WordId = word.Id,
                };

                word = CheckWordSecondary(word, secretText, letterOrWord);

                //если выиграл возвращает null
                if (word == null)
                {
                    //стирает угаданное слово из списка вопросов , что бы не повторялось 
                    words.RemoveAt(0);
                    match.Result = true;
                    unitOfWork.Match.Add(match);
                    currentUser.Rating++;
                    unitOfWork.SaveChangesAsync();

                    if (words.Count > 0)
                    {
                        wordModels = words;
                        return GetSecretWord(new WordModel{Question = wordModels.First().Question,SecretWord = wordModels.First().SecretWord, TryCount = wordModels.First().TryCount});
                    }

                    return null;
                }
                else
                {
                    if (word.IsLose)
                    {
                        //lose
                        match.Result = false;
                        currentUser.Rating--;
                        unitOfWork.Match.Add(match);
                        unitOfWork.SaveChangesAsync();
                        throw new ArgumentOutOfRangeException();
                    }
                    else
                    {
                        words[0].TryCount = word.TryCount;
                        return word;
                    }
                }
            }

            return null;
        }

        private IEnumerable<WordModel> GenerateRandomWords(IEnumerable<WordModel> words)
        {
            var wordList = new List<WordModel>(words);
            int count = wordList.Count;
            var stack = new Stack<WordModel>();
            var random = new Random();
            for (int i = 0; i < count; i++)
            {
                var element = wordList[random.Next(0, wordList.Count)];
                stack.Push(element);
                wordList.Remove(element);
            }

            return stack;
        }


        public WordModel CheckWordSecondary(WordModel wordModel, string secretText, string wordOrLetter)
        {
            if (wordOrLetter.Length > 1)
            {
                //new Match into Database
                if (string.Equals(wordModel.SecretWord, wordOrLetter, StringComparison.CurrentCultureIgnoreCase))
                {
                    //MatchResult = true
                    return null;
                    
                }
                else
                {
                    //MatchResult = false
                    wordModel.IsLose = true;
                    return wordModel;
                }
            }
            else
            {
                var targetLetter = wordOrLetter.ToUpper()[0];
                var secretWordSymbols = wordModel.SecretWord.ToUpper().ToCharArray(0, wordModel.SecretWord.Length);
                var result = new StringBuilder(secretText);
                var isFoundLetter = false;
                for (var i = 0; i != secretWordSymbols.Length; ++i)
                {
                    if (secretWordSymbols[i] == targetLetter)
                    {
                        result[i] = targetLetter;
                        isFoundLetter = true;
                    }
                }

                if (wordModel.SecretWord.ToUpper().Equals(result.ToString()))
                {
                    return null;
                }

                wordModel.SecretWord = result.ToString();

                if (!isFoundLetter)
                {
                    wordModel.TryCount--;
                    if (wordModel.TryCount == 0)
                    {
                        //MatchResult = false
                        wordModel.IsLose = true;
                        return wordModel;
                    }
                    else
                    {
                        return wordModel;
                    }
                }

                return wordModel;
            }
        }

    }
}
