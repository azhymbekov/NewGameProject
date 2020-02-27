using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using GameProject.Data.Models.Game;
using GameProject.Data.Models.Users;
using GameProject.Service.Common.UserService.Models;
using GameProject.Service.Common.WordService.Models;

namespace GameProject.Service.Automapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Word, WordModel>()
                .ForMember(x => x.TryCount,
                    opt => opt.MapFrom(x => x.SecretWord.Length - 2));
            CreateMap<WordModel, Word>();


            CreateMap<User, UserModel>();
            CreateMap<UserModel, User>();
        }
    }
}
