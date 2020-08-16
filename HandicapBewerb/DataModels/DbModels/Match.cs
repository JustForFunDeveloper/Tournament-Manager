using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TournamentManager.DataModels.DbModels
{
    public class Match
    {
        public int MatchId { get; set; }
        public DateTime Date { get; set; }

        [NotMapped]
        public bool MatchToDelete { get; set; }

        public ICollection<MatchResult> MatchResults{ get; set; }
        public ICollection<UserMatch> UserMatches { get; set; }
    }
}
