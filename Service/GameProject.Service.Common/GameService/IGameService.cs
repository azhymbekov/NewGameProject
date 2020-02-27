using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GameProject.Service.Common.WordService.Models;

namespace GameProject.Service.Common.GameService
{
    public interface IGameService
    {
        //Получает рандом слова из БД
        IEnumerable<WordModel> GetRandomWords();

        //Пребразует слова в звездочки
        WordModel GetSecretWord(WordModel model);

        //Проверяет слово на наличие букв или слова 
        WordModel CheckWord(ref IEnumerable<WordModel> words, string secretText, string letterOrWord, Guid userId);
    }
}
