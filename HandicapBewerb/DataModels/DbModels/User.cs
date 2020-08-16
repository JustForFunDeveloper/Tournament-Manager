using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TournamentManager.DataModels.DbModels
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }

        [NotMapped]
        public bool IsSelected { get; set; }
        [NotMapped]
        public bool UsersToDelete { get; set; }

        public ICollection<Round> Rounds { get; set; }
        public ICollection<UserMatch> UserMatches { get; set; }
    }
}