using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Data.Models.Users;

namespace GameProject.Data.Models.Game
{
    public class Match : BaseModel
    {
        public Guid UserId { get; set; }

        public Guid WordId { get; set; }

        public bool Result { get; set; }

        public virtual User User { get; set; }

        public virtual Word Word { get; set; }
    }
}
