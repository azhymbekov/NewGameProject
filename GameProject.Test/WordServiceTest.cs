using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using GameProject.Data.Models.Game;
using GameProject.Service.Automapper;
using GameProject.Service.Common.WordService;
using GameProject.Service.WordService;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ProjectGame.Data.Common.Repositories;
using Xunit;

namespace GameProject.Test
{
    public class WordServiceTest
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private IMapper _mapper;
        private Mock<IRepository<Word>> _mockWordRepository;
        private IWordService _wordService;

        public WordServiceTest()
        {
            _mockWordRepository = new Mock<IRepository<Word>>();
            _mockWordRepository.Setup(x => x.All()).Returns(ListOfWords());

            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(x => x.Word.All()).Returns(_mockWordRepository.Object.All());
            _mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapperProfile());
            }));

            _wordService = new WordService(_mockUnitOfWork.Object, _mapper);
        }

        

        [Fact]
        public void GetAllListOfWords()
        {
            //Act
            var list = _wordService.GetWords();

            //Assert
            Assert.Equal(ListOfWords().ToList().Count, list.Count());
        }

        public  IQueryable<Word> ListOfWords()
        {
            var words = new List<Word>
            {
                new Word{Id=Guid.NewGuid(),Question= "Кто такой Пушкин",SecretWord= "Писатель"},
                new Word{Id=Guid.NewGuid(),Question= "Как зовут Атамбаева",SecretWord= "Алмазбек"}, };
            return words.AsQueryable();
        }

    }
}
