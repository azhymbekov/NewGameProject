using GameProject.Data.Models.Game;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GameProject.Data.Models.Users;

namespace ProjectGame.Data.Common.Repositories
{
    public interface IUnitOfWork
    {
        IRepository<Word> Word { get; }
        IRepository<Match> Match { get; }

        Task<int> SaveChangesAsync();
    }
}
