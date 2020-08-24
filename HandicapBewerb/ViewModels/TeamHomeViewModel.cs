using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using JetBrains.Annotations;
using TournamentManager.Core.Data;
using TournamentManager.Core.Handler;
using TournamentManager.DataModels.DbModels;
using TournamentManager.ViewModels.Handler;
using TournamentManager.Views.UserControls;

namespace TournamentManager.ViewModels
{
    class TeamHomeViewModel : INotifyPropertyChanged
    {
        // ReSharper disable once NotAccessedField.Local
        public event PropertyChangedEventHandler PropertyChanged;

        #region private Member

        private ICommand _newGame;
        private ICommand _addPlayers;
        private ICommand _saveAll;
        private ICommand _sortPositions;

        private readonly string DOUBLE_FORMAT = "##.#0";
        private readonly string DOUBLE_FORMAT_ROUNDS = "##.0";
        private readonly string DATE_FORMAT = "dd.MM.yy HH:mm";

        #endregion

        public bool IsProgressActive { get; set; }
        public bool IsAddPlayersEnabled { get; set; }
        public bool IsSaveAllEnabled { get; set; }
        public bool IsSortPositionsEnabled { get; set; }

        public ObservableCollection<TeamUserDataControl> TeamUserDataControls { get; set; }

        public TeamHomeViewModel()
        {
            Mediator.Register(MediatorGlobal.AddTeamPlayerViewClose, AddTeamPlayerViewClose);
            Mediator.Register(MediatorGlobal.RefreshView, OnRefreshView);
            TeamUserDataControls = new ObservableCollection<TeamUserDataControl>();

            IsAddPlayersEnabled = true;
            IsSaveAllEnabled = true;
            IsSortPositionsEnabled = true;
        }

        private void OnRefreshView(object obj)
        {
            var view = (MainViewModel.ApplicationViewEnum)obj;
            if (view.Equals(MainViewModel.ApplicationViewEnum.AddTeamPlayerView))
            {
                AddTeamPlayerViewClose(null);
            }
        }

        private void AddTeamPlayerViewClose(object obj)
        {
            try
            {
                if (ApplicationData.CreatedTeams != null)
                {
                    var teamUserDataControls = new List<TeamUserDataControl>();
                    foreach (var team in ApplicationData.CreatedTeams)
                    {
                        var teamUserDataControl = new TeamUserDataControl();
                        var userDataControls = InitUserDataControls(team);
                        teamUserDataControl.UserDataControls = userDataControls;
                        teamUserDataControl.TeamName = team.TeamName;
                        teamUserDataControl.RefreshResults();
                        teamUserDataControls.Add(teamUserDataControl);
                    }

                    TeamUserDataControls = new ObservableCollection<TeamUserDataControl>(teamUserDataControls);
                    ApplicationData.TeamUserDataControls = TeamUserDataControls.ToList();
                }
                else
                {
                    TeamUserDataControls.Clear();
                    if (ApplicationData.TeamUserDataControls != null)
                    {
                        ApplicationData.TeamUserDataControls.Clear();
                    }
                }
            }
            catch (Exception e)
            {
                LogHandler.WriteSystemLog("OnAddPlayerViewClose:" + e, LogLevel.Error);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnCustomPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region IconBar

        public ICommand NewGame
        {
            get
            {
                if (_newGame == null)
                    _newGame = new RelayCommand(
                        param => NewGameCommand(),
                        param => CanNewGameCommand()
                    );
                return _newGame;
            }
        }

        private bool CanNewGameCommand()
        {
            return true;
        }

        private void NewGameCommand()
        {
            ApplicationData.CreatedTeams = null;
            ApplicationData.TeamUserDataControls = null;
            TeamUserDataControls.Clear();

            IsSaveAllEnabled = true;
            IsAddPlayersEnabled = true;
            IsSortPositionsEnabled = true;
        }

        public ICommand AddPlayers
        {
            get
            {
                if (_addPlayers == null)
                    _addPlayers = new RelayCommand(
                        param => AddPlayersCommand(),
                        param => CanAddPlayersCommand()
                    );
                return _addPlayers;
            }
        }

        private bool CanAddPlayersCommand()
        {
            return true;
        }

        private void AddPlayersCommand()
        {
            Mediator.NotifyColleagues(MediatorGlobal.AddTeamPlayerViewOpen, null);
        }

        public ICommand SaveAll
        {
            get
            {
                if (_saveAll == null)
                    _saveAll = new RelayCommand(
                        param => SaveAllCommand(),
                        param => CanSaveAllCommand()
                    );
                return _saveAll;
            }
        }

        private bool CanSaveAllCommand()
        {
            return true;
        }

        private void SaveAllCommand()
        {
            try
            {
                SortList();
                DbHandler.SaveTeamMatch(ApplicationData.TeamUserDataControls);
                IsSaveAllEnabled = false;
                IsAddPlayersEnabled = false;
                IsSortPositionsEnabled = false;
                foreach (var team in TeamUserDataControls)
                {
                    foreach (var userDataControl in team.UserDataControls)
                    {
                        userDataControl.IsInputEnabled = false;
                    }
                }
            }
            catch (Exception e)
            {
                LogHandler.WriteSystemLog("SaveAllCommand:" + e, LogLevel.Error);
            }
        }

        public ICommand SortPositions
        {
            get
            {
                if (_sortPositions == null)
                    _sortPositions = new RelayCommand(
                        param => SortPositionsCommand(),
                        param => CanSortPositionsCommand()
                    );
                return _sortPositions;
            }
        }

        private bool CanSortPositionsCommand()
        {
            return true;
        }

        private void SortPositionsCommand()
        {
            SortList();
        }

        #endregion

        private List<UserDataControl> InitUserDataControls(Team team)
        {
            var userIds = team.Players.Select(x => x.UserId).ToList();
            List<UserDataControl> userDataControls = new List<UserDataControl>();

            List<User> users = DbHandler.GetUsers();

            foreach (var user in users)
            {
                if (userIds.Contains(user.UserId))
                {
                    UserDataControl userDataControl;
                    if (ApplicationData.TeamUserDataControls == null ||
                        !ApplicationData.TeamUserDataControls.Any(t => t.TeamName.Equals(team.TeamName)))
                    {
                        userDataControl = new UserDataControl(user.UserId);
                        EditUserDataControlValues(userDataControl, user);
                    }
                    else
                    {
                        var teamUserDataControl = ApplicationData.TeamUserDataControls.Single(t => t.TeamName.Equals(team.TeamName));

                        if (!teamUserDataControl.UserDataControls.Any(u => u.UserId.Equals(user.UserId)))
                        {
                            userDataControl = new UserDataControl(user.UserId);
                            EditUserDataControlValues(userDataControl, user);
                        }
                        else
                        {
                            userDataControl =
                                teamUserDataControl.UserDataControls.Single(u => u.UserId.Equals(user.UserId));
                            EditUserDataControlValues(userDataControl, user);
                        }
                    }
                    userDataControls.Add(userDataControl);
                }
            }
            return userDataControls;
        }

        private void EditUserDataControlValues(UserDataControl userDataControl, User user)
        {
            userDataControl.UserName = user.Name;

            List<Round> rounds = DbHandler.GetLastThreeRoundsFromUserOrderedByDate(user.UserId);

            CultureInfo cultureInfo = CultureInfo.CurrentCulture;

            double nullValueSum = 0;
            int meanCounter = 0;

            if (rounds.Count > 0)
            {
                nullValueSum += rounds[0].Points;
                meanCounter++;
                userDataControl.FirstOldest = rounds[0].Points.ToString(DOUBLE_FORMAT_ROUNDS);
                userDataControl.FirstOldestDateTime = rounds[0].Date.ToString(DATE_FORMAT, cultureInfo);
            }

            if (rounds.Count > 1)
            {
                nullValueSum += rounds[1].Points;
                meanCounter++;
                userDataControl.SecondOldest = rounds[1].Points.ToString(DOUBLE_FORMAT_ROUNDS);
                userDataControl.SecondOldestDateTime = rounds[1].Date.ToString(DATE_FORMAT, cultureInfo);
            }

            if (rounds.Count > 2)
            {
                nullValueSum += rounds[2].Points;
                meanCounter++;
                userDataControl.ThirdOldest = rounds[2].Points.ToString(DOUBLE_FORMAT_ROUNDS);
                userDataControl.ThirdOldestDateTime = rounds[2].Date.ToString(DATE_FORMAT, cultureInfo);
            }

            double meanNullValue = nullValueSum / meanCounter;
            userDataControl.NullValue = meanNullValue.ToString(DOUBLE_FORMAT);
        }

        private void SortList()
        {
            var teamUserDataControls = ApplicationData.TeamUserDataControls.OrderByDescending(t => t.TeamSortResult);
            int position = 1;

            var allPlayer = new List<UserDataControl>();

            foreach (var teamUserDataControl in teamUserDataControls)
            {
                teamUserDataControl.TeamPosition = position.ToString();
                allPlayer.AddRange(teamUserDataControl.UserDataControls);
                position++;
            }

            TeamUserDataControls = new ObservableCollection<TeamUserDataControl>(teamUserDataControls);

            var sortedPlayers = allPlayer.OrderByDescending(u => u.SortResult);

            int playerPosition = 1;
            foreach (var userDataControl in sortedPlayers)
            {
                userDataControl.Position = playerPosition.ToString();
                playerPosition++;
            }

            foreach (var teamUserDataControl in TeamUserDataControls)
            {
                teamUserDataControl.UserDataControls =
                    teamUserDataControl.UserDataControls.OrderByDescending(u => u.SortResult).ToList();
            }
        }
    }
}
