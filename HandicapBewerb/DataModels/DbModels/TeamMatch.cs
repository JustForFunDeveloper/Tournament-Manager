using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TournamentManager.DataModels.DbModels
{
    public class TeamMatch
    {
        public int TeamMatchId { get; set; }
        public DateTime Date { get; set; }

        [NotMapped]
        public bool TeamMatchToDelete { get; set; }

        public ICollection<TeamMatchResult> TeamMatchResults { get; set; }
        public ICollection<SoloTeamMatchResult> SoloTeamMatchResults { get; set; }
        public ICollection<UserTeamMatch> UserTeamMatches { get; set; }
    }

    public class SoloTeamMatchResult
    {
        public int SoloTeamMatchResultId { get; set; }
        public double Result { get; set; }
        public double Round { get; set; }
        public int Position { get; set; }
        public string UserName { get; set; }
    }

    public class UserTeamMatch
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int TeamMatchId { get; set; }
        public TeamMatch TeamMatch { get; set; }
    }

    public class TeamMatchResult
    {
        public int TeamMatchResultId { get; set; }
        public double Result { get; set; }
        public string TeamName { get; set; }
        public string UserNames { get; set; }
    }
}
