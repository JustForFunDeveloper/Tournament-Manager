using System;
using System.Collections.Generic;
using System.Linq;
using HandicapBewerb.Core.Data;
using HandicapBewerb.DataModels.DbModels;
using HandicapBewerb.Views.UserControls;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace HandicapBewerb.Core.Handler
{
    public static class DBHandler
    {
        public static void deleteUsers(List<User> users)
        {
            using (var db = new ApplicationDbContext())
            {
                foreach (var user in users)
                {
                    if (user.UsersToDelete)
                    {
                        var queryable = db.Users.Include("Rounds").Single(u => u.UserId.Equals(user.UserId));
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

        public static void Migrate()
        {

            using (var db = new ApplicationDbContext())
            {
                db.Database.Migrate();

                //TestData(db);
            }
        }

        private static void TestData(ApplicationDbContext db)
        {
            List<Round> andiRounds = new List<Round>()
            {
                new Round()
                {
                    Date = DateTime.Now,
                    Points = 10,
                },
                new Round()
                {
                    Date = DateTime.Now.AddHours(1),
                    Points = 11,
                },
                new Round()
                {
                    Date = DateTime.Now.AddHours(2),
                    Points = 12,
                }
            };

            List<Round> franzRounds = new List<Round>()
            {
                new Round()
                {
                    Date = DateTime.Now,
                    Points = 20,
                },
                new Round()
                {
                    Date = DateTime.Now.AddHours(1),
                    Points = 21,
                },
                new Round()
                {
                    Date = DateTime.Now.AddHours(2),
                    Points = 22,
                }
            };

            User andreas = new User()
            {
                Name = "Andreas",
                Rounds = andiRounds
            };

            User franz = new User()
            {
                Name = "Franz",
                Rounds = franzRounds
            };

            MatchResult andiMatchResult = new MatchResult()
            {
                Result = 112,
                UserName = andreas.Name
            };

            MatchResult franzMatchResult = new MatchResult()
            {
                Result = 212,
                UserName = franz.Name
            };

            Match match = new Match()
            {
                Date = DateTime.Now,
                MatchResults = new List<MatchResult>()
                {
                    andiMatchResult,
                    franzMatchResult
                }
            };

            db.Users.AddRange(new List<User>()
            {
                andreas,
                franz
            });

            db.Matches.Add(match);
            db.SaveChanges();
        }

        public static void AddUser(string name)
        {
            using (var db = new ApplicationDbContext())
            {
                var user = new User()
                {
                    Name = name,
                    Rounds = new List<Round>(),
                    Matches = new List<Match>()
                };

                db.Users.Add(user);
                db.SaveChanges();
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
                users = new List<User>(db.Users.Include("Rounds"));
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
                rounds = db.Users.Include("Rounds").Single(u => u.UserId.Equals(user.UserId)).Rounds.ToList();
            }

            return rounds;
        }

        public static List<Round> GetLastThreeRoundsFromUserOrderedByDate(int userId)
        {
            List<Round> rounds; 
            using (var db = new ApplicationDbContext())
            {
                rounds = db.Users.Include("Rounds").First(u => u.UserId == userId).Rounds.OrderByDescending(r => r.Date)
                    .Take(3).ToList();
            }

            return rounds;
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
                    var user = db.Users.Include("Rounds").Single(u => u.UserId.Equals(userDataControl.UserId));
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

                db.Matches.Add(match);
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

        public static List<MatchResult> GetMatchResults(Match match)
        {
            if (match == null)
            {
                return null;
            }

            List<MatchResult> matchResults;
            using (var db = new ApplicationDbContext())
            {
                matchResults = db.Matches.Include("MatchResults").Single(m => m.MatchId.Equals(match.MatchId)).MatchResults.ToList();
            }

            return matchResults;
        }

        public static List<Match> GetMatchesIncludingResults()
        {
            List<Match> matches;
            using (var db = new ApplicationDbContext())
            {
                matches = new List<Match>(db.Matches.Include("MatchResults"));
            }

            return matches;
        }

        public static void deleteMatches(List<Match> matches)
        {
            using (var db = new ApplicationDbContext())
            {
                foreach (var match in matches)
                {
                    if (match.MatchToDelete)
                    {
                        var queryable = db.Matches.Include("MatchResults").Single(m => m.MatchId.Equals(match.MatchId));
                        db.MatchResults.RemoveRange(queryable.MatchResults);
                        db.Matches.Remove(queryable);
                        db.SaveChanges();
                    }
                }
            }
        }
    }
}
