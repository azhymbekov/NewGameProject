using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameProject.Data.Models.Game;
using GameProject.Data.Models.Users;
using ProjectGame.Data.Common.Repositories;

namespace GameProject.Data.Repository
{
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
         private AppDbContext db;

         private IRepository<Word> wordService;
         private IRepository<Match> gameService;

         public UnitOfWork(AppDbContext db)
         {
             this.db = db;
         }


        public IRepository<Word> Word
        {
            get { return wordService ?? (wordService = new EfRepository<Word>(db)); }
        }

        public IRepository<Match> Match
        {
            get { return gameService ?? (gameService = new EfRepository<Match>(db)); }
        }
        

        public Task<int> SaveChangesAsync() => this.db.SaveChangesAsync();

        public void Dispose() => this.db.Dispose();

       
    }
}
