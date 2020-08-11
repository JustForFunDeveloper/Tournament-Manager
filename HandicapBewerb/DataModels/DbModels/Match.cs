using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HandicapBewerb.DataModels.DbModels
{
    public class Match
    {
        public int MatchId { get; set; }
        public DateTime Date { get; set; }

        [NotMapped]
        public bool MatchToDelete { get; set; }

        public ICollection<MatchResult> MatchResults{ get; set; }
    }
}
