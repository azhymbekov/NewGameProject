using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameProject.Data.Models.Game;

namespace GameProject.Test
{
    public class FakeInstances
    {
        public static IQueryable<Word> ListOfWords()
        {
            var words = new List<Word>
            {
                new Word{Id=Guid.NewGuid(),Question= "Кто такой Пушкин",SecretWord= "Писатель"},
                new Word{Id=Guid.NewGuid(),Question= "Как зовут Атамбаева",SecretWord= "Алмазбек"}, };
            return words.AsQueryable();
        }
    }
}
