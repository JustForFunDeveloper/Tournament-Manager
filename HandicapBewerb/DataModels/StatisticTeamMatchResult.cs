namespace TournamentManager.DataModels.DbModels
{
    public class StatisticTeamMatchResult
    {
        public TeamMatchResult TeamMatchResult { get; set; }
        public SoloTeamMatchResult SoloTeamMatchResult { get; set; }

        public StatisticTeamMatchResult()
        {
        }
    }
}
