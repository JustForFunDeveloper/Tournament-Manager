using System;

namespace TournamentManager.DataModels.DbModels
{
    public class Round
    {
        public int RoundId { get; set; }
        public DateTime Date { get; set; }
        public double Points { get; set; }
    }
}
