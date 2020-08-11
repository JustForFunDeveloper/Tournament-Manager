namespace HandicapBewerb.DataModels.DbModels
{
    public class MatchResult
    {
        public int MatchResultId { get; set; }
        public double Result { get; set; }
        public double Round { get; set; }
        public int Position { get; set; }
        public string UserName { get; set; }
    }
}
