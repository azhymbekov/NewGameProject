using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameProject.Service.Common.WordService.Models;

namespace GameProject.Service.Common.WordService
{
    public interface IWordService
    {
        Task<WordModel> GetWordAsync();

        IEnumerable<WordModel> GetWords();

        Task RemoveAsync(Guid id);

        Task<OperationResult> SaveAsync(WordModel model);

        Task<WordModel> PrepeareWordForEditView(Guid? id);
    }
}
