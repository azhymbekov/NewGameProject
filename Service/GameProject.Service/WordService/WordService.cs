using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GameProject.Data.Models.Game;
using GameProject.Service.Common;
using GameProject.Service.Common.WordService;
using GameProject.Service.Common.WordService.Models;
using Microsoft.EntityFrameworkCore;
using ProjectGame.Data.Common.Repositories;

namespace GameProject.Service.WordService
{
    public class WordService : IWordService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IMapper mapper;

        public WordService(IUnitOfWork unitOfWork,  IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<WordModel> GetWordAsync()
        {
            var word = await unitOfWork.Word.AllAsNoTracking().FirstOrDefaultAsync();
            if (word != null)
            {
                return this.mapper.Map<WordModel>(word);
            }

            return null;
        }

        public IEnumerable<WordModel> GetWords()
        {
            return this.mapper.ProjectTo<WordModel>(unitOfWork.Word.AllAsNoTracking());
        }

        public async Task RemoveAsync(Guid id)
        {
            var word = await unitOfWork.Word.GetByIdAsync(id);
            if (word != null)
            {
                unitOfWork.Word.Delete(word);
                await unitOfWork.Word.SaveChangesAsync();
            }
        }

        public async Task<OperationResult> SaveAsync(WordModel model)
        {
            var result = new OperationResult();

            var word = await unitOfWork.Word.All().FirstOrDefaultAsync(x => x.Id == model.Id);

            if (word == null)
            {
                var sword = await unitOfWork.Word.All().FirstOrDefaultAsync(x => x.SecretWord == model.SecretWord);
               
                if (sword == null)
                {
                    sword = mapper.Map<Word>(model);
                    model.Id = Guid.NewGuid();
                    unitOfWork.Word.Add(sword);
                }

                else
                {
                    result.Message = "error";
                }

            }
            else
            {
                word.SecretWord = model.SecretWord;
                word.Question = model.Question;
            }

            await unitOfWork.SaveChangesAsync();
            return result;
        }

        public async Task<WordModel> PrepeareWordForEditView(Guid? id)
        {
            return this.mapper.ProjectTo<WordModel>(unitOfWork.Word.AllAsNoTracking()).FirstOrDefault(x => x.Id == id);

            //var word = (from w in unitOfWork.Word.AllAsNoTracking()
            //    where w.Id == id
            //    select new WordModel
            //    {
            //        Id = w.Id,
            //        Question = w.Question,
            //        SecretWord = w.SecretWord
            //    }).FirstOrDefault();
            //return word;
        }
    }
}
