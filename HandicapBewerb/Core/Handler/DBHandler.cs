using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TournamentManager.Core.Data;
using TournamentManager.DataModels.DbModels;
using TournamentManager.Views.UserControls;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace TournamentManager.Core.Handler
{
    public static class DbHandler
    {
        public static void Migrate()
        {

            using (var db = new ApplicationDbContext())
            {
                db.Database.Migrate();

                //TestData(db);
            }
        }

        public static void AddUser(string name)
        {
            using (var db = new ApplicationDbContext())
            {
                var user = new User()
                {
                    Name = name,
                    Rounds = new List<Round>(),
                    UserMatches = new List<UserMatch>()
                };

                db.Users.Add(user);
                db.SaveChanges();
            }
        }

        public static void UpdateUser(User user)
        {
            using (var db = new ApplicationDbContext())
            {
                db.Users.Update(user);
                db.SaveChanges();
            }
        }

        public static void DeleteUsers(List<User> users)
        {
            using (var db = new ApplicationDbContext())
            {
                foreach (var user in users)
                {
                    if (user.UsersToDelete)
                    {
                        var queryable = db.Users.Include(u => u.Rounds).Single(u => u.UserId.Equals(user.UserId));
                        db.Rounds.RemoveRange(queryable.Rounds);
                        db.Users.Remove(queryable);
                        db.SaveChanges();

                        if (user.IsSelected)
                        {
                            ApplicationData.CurrentSelectedUser.Remove(user.UserId);
                            var index = ApplicationData.UserDataControls.FindIndex(u => u.UserId.Equals(user.UserId));
                            ApplicationData.UserDataControls.RemoveAt(index);
                        }
                    }
                }
            }
        }

        public static List<User> GetUsers()
        {
            List<User> users;
            using (var db = new ApplicationDbContext())
            {
               users = new List<User>(db.Users);
            }

            return users;
        }

        public static List<User> GetUsersIncludingRounds()
        {
            List<User> users;
            using (var db = new ApplicationDbContext())
            {
                users = new List<User>(db.Users.Include(u=>u.Rounds));
            }

            return users;
        }

        [CanBeNull]
        public static List<Round> GetUserRounds(User user)
        {
            if (user == null)
            {
                return null;
            }

            List<Round> rounds;
            using (var db = new ApplicationDbContext())
            {
                rounds = db.Users.Include(u=>u.Rounds).Single(u => u.UserId.Equals(user.UserId)).Rounds.ToList();
            }

            return rounds;
        }

        public static List<Round> GetLastThreeRoundsFromUserOrderedByDate(int userId)
        {
            List<Round> rounds; 
            using (var db = new ApplicationDbContext())
            {
                rounds = db.Users.Include(u=>u.Rounds).First(u => u.UserId == userId).Rounds.OrderByDescending(r => r.Date)
                    .Take(3).ToList();
            }

            return rounds;
        }

        public static void RemoveRounds(List<Round> roundsToDelete, User user)
        {
            using (var db = new ApplicationDbContext())
            {
                var singleUser = db.Users.Single(u => u.UserId.Equals(user.UserId));
                var queryableRounds = db.Rounds.Where(r => EF.Property<int>(r, "UserId") == user.UserId).Where(r => roundsToDelete.Contains(r));

                foreach (var round in queryableRounds)
                {
                    singleUser.Rounds.Remove(round);
                    db.Rounds.Remove(round);
                }

                db.SaveChanges();
            }
        }

        public static void SaveMatch(List<UserDataControl> userDataControls)
        {
            using (var db = new ApplicationDbContext())
            {
                List<MatchResult> matchResults = new List<MatchResult>();
                foreach (var userDataControl in userDataControls)
                {
                    var round = new Round()
                    {
                        Date = DateTime.Now,
                        Points = userDataControl.CurrentRound
                    };
                    var user = db.Users
                        .Include(u => u.Rounds)
                        .Include(u => u.UserMatches)
                        .Single(u => u.UserId.Equals(userDataControl.UserId));
                    user.Rounds.Add(round);

                    Double.TryParse(userDataControl.Result, out double parsedResult);
                    Int32.TryParse(userDataControl.Position, out int parsedPosition);

                    if (Double.IsNaN(parsedPosition))
                    {
                        parsedPosition = -9999;
                    }

                    matchResults.Add(new MatchResult()
                    {
                        Result = parsedResult,
                        Position = parsedPosition,
                        UserName = user.Name,
                        Round = userDataControl.CurrentRound
                    });
                }

                var match = new Match()
                {
                    Date = DateTime.Now,
                    MatchResults = matchResults
                };

                // Needed for many to many relationship
                foreach (var userDataControl in userDataControls)
                {
                    var user = db.Users
                        .Include(u => u.Rounds)
                        .Include(u => u.UserMatches)
                        .Single(u => u.UserId.Equals(userDataControl.UserId));

                    var userMatch = new UserMatch()
                    {
                        Match = match,
                        User = user
                    };
                    user.UserMatches.Add(userMatch);
                }

                db.SaveChanges();
            }
        }

        public static void DeleteMatches(List<Match> matches)
        {
            using (var db = new ApplicationDbContext())
            {
                foreach (var match in matches)
                {
                    if (match.MatchToDelete)
                    {
                        var queryable = db.Matches.Include(m=>m.MatchResults).Single(m => m.MatchId.Equals(match.MatchId));
                        db.MatchResults.RemoveRange(queryable.MatchResults);
                        db.Matches.Remove(queryable);
                        db.SaveChanges();
                    }
                }
            }
        }

        public static List<MatchResult> GetMatchResults(Match match)
        {
            if (match == null)
            {
                return null;
            }

            List<MatchResult> matchResults;
            using (var db = new ApplicationDbContext())
            {
                matchResults = db.Matches.Include(m=>m.MatchResults).Single(m => m.MatchId.Equals(match.MatchId)).MatchResults.ToList();
            }

            return matchResults;
        }

        public static List<Match> GetMatchesIncludingResults()
        {
            List<Match> matches;
            using (var db = new ApplicationDbContext())
            {
                matches = new List<Match>(db.Matches.Include(m=>m.MatchResults));
            }

            return matches;
        }

        public static List<Match> GetAllMatchesFromUser(User user)
        {
            using (var db = new ApplicationDbContext())
            {
                var myUser = db.Users.Include(m => m.UserMatches)
                    .ThenInclude(m => m.Match)
                    .ThenInclude(m => m.MatchResults)
                    .Single(u => u.UserId.Equals(user.UserId));
                return new List<Match>(myUser.UserMatches.Select(userMatch => userMatch.Match));
            }
        }

        public static void SaveTeamMatch(List<TeamUserDataControl> teamUserDataControls)
        {
            using (var db = new ApplicationDbContext())
            {
                List<TeamMatchResult> teamMatchResults = new List<TeamMatchResult>();
                List<SoloTeamMatchResult> soloTeamMatchResults = new List<SoloTeamMatchResult>();
                foreach (var teamUserDataControl in teamUserDataControls)
                {
                    StringBuilder userNamesBuilder = new StringBuilder();
                    foreach (var userDataControl in teamUserDataControl.UserDataControls)
                    {
                        var round = new Round()
                        {
                            Date = DateTime.Now,
                            Points = userDataControl.CurrentRound
                        };
                        var user = db.Users
                            .Include(u => u.Rounds)
                            .Include(u => u.UserTeamMatches)
                            .Single(u => u.UserId.Equals(userDataControl.UserId));
                        user.Rounds.Add(round);

                        Double.TryParse(userDataControl.Result, out double parsedResult);
                        Int32.TryParse(userDataControl.Position, out int parsedPosition);

                        if (Double.IsNaN(parsedPosition))
                        {
                            parsedPosition = -9999;
                        }

                        soloTeamMatchResults.Add(new SoloTeamMatchResult()
                        {
                            Result = parsedResult,
                            Position = parsedPosition,
                            UserName = user.Name,
                            Round = userDataControl.CurrentRound
                        });

                        userNamesBuilder.Append(userDataControl.UserName).Append(";");
                    }

                    Double.TryParse(teamUserDataControl.TeamResult, out double parsedTeamResult);
                    Int32.TryParse(teamUserDataControl.TeamPosition, out int parsedTeamPosition);

                    if (Double.IsNaN(parsedTeamPosition))
                    {
                        parsedTeamPosition = -9999;
                    }

                    teamMatchResults.Add(new TeamMatchResult()
                    {
                        TeamName = teamUserDataControl.TeamName,
                        UserNames = userNamesBuilder.ToString(),
                        Result = parsedTeamResult,
                        Position = parsedTeamPosition
                    });
                }

                var teamMatch = new TeamMatch()
                {
                    Date = DateTime.Now,
                    SoloTeamMatchResults = soloTeamMatchResults,
                    TeamMatchResults = teamMatchResults
                };

                foreach (var teamUserDataControl in teamUserDataControls)
                {
                    // Needed for many to many relationship
                    foreach (var userDataControl in teamUserDataControl.UserDataControls)
                    {
                        var user = db.Users
                            .Include(u => u.Rounds)
                            .Include(u => u.UserTeamMatches)
                            .Single(u => u.UserId.Equals(userDataControl.UserId));

                        var userTeamMatch = new UserTeamMatch()
                        {
                            TeamMatch = teamMatch,
                            User = user
                        };
                        user.UserTeamMatches.Add(userTeamMatch);
                    }
                }

                db.SaveChanges();
            }
        }

        public static void DeleteTeamMatches(List<TeamMatch> teamMatches)
        {
            using (var db = new ApplicationDbContext())
            {
                foreach (var teamMatch in teamMatches)
                {
                    if (teamMatch.TeamMatchToDelete)
                    {
                        var queryable = db.TeamMatches
                            .Include(m => m.TeamMatchResults)
                            .Include(t => t.SoloTeamMatchResults)
                            .Single(m => m.TeamMatchId.Equals(teamMatch.TeamMatchId));
                        db.TeamMatchResults.RemoveRange(queryable.TeamMatchResults);
                        db.SoloTeamMatchResults.RemoveRange(queryable.SoloTeamMatchResults);
                        db.TeamMatches.Remove(queryable);
                        db.SaveChanges();
                    }
                }
            }
        }

        public static List<SoloTeamMatchResult> GetSoloTeamMatchResults(TeamMatch teamMatch)
        {
            if (teamMatch == null)
            {
                return null;
            }

            List<SoloTeamMatchResult> teamMatchResults;
            using (var db = new ApplicationDbContext())
            {
                teamMatchResults = db.TeamMatches
                    .Include(m => m.SoloTeamMatchResults)
                    .Single(m => m.TeamMatchId.Equals(teamMatch.TeamMatchId))
                    .SoloTeamMatchResults.ToList();
            }

            return teamMatchResults;
        }

        public static List<TeamMatchResult> GetTeamMatchResults(TeamMatch teamMatch)
        {
            if (teamMatch == null)
            {
                return null;
            }

            List<TeamMatchResult> teamMatchResults;
            using (var db = new ApplicationDbContext())
            {
                teamMatchResults = db.TeamMatches
                    .Include(m => m.TeamMatchResults)
                    .Single(m => m.TeamMatchId.Equals(teamMatch.TeamMatchId))
                    .TeamMatchResults.ToList();
            }

            return teamMatchResults;
        }

        public static List<TeamMatch> GetTeamMatchesIncludingResults()
        {
            List<TeamMatch> matches;
            using (var db = new ApplicationDbContext())
            {
                matches = new List<TeamMatch>(db.TeamMatches
                    .Include(m => m.SoloTeamMatchResults)
                    .Include(m => m.TeamMatchResults));
            }

            return matches;
        }
    }
}
