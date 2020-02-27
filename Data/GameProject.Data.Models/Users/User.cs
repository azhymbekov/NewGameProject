using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Data.Models.Game;
using Microsoft.AspNetCore.Identity;

namespace GameProject.Data.Models.Users
{
    public class User : IdentityUser<Guid>
    {
        public string FullName { get; set; }

        public ICollection<Match> Matches { get; set; }
    }
}
