using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using JetBrains.Annotations;
using TournamentManager.Core.Data;
using TournamentManager.Core.Handler;
using TournamentManager.DataModels.DbModels;
using TournamentManager.ViewModels.Handler;

namespace TournamentManager.ViewModels
{
    class TeamAddPlayerViewModel : INotifyPropertyChanged
    {
        private IList _selectedUsers;

        private ICommand _onClose;
        private ICommand _onAddUser;
        private ICommand _onDeleteUser;
        private ICommand _onSaveTeam;
        private ICommand _onDiscardTeam;
        private ICommand _onAddTeam;
        private ICommand _onDeleteTeam;

        private bool _canOnDeleteTeam = false;
        private bool _canOnAddUser = false;
        private bool _canOnDeleteUser = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<User> Users { get; set; }
        public ObservableCollection<Team> TeamControls { get; set; }
        public Visibility EditingVisibility { get; set; } = Visibility.Collapsed;

        public TeamAddPlayerViewModel()
        {
            Mediator.Register(MediatorGlobal.AddTeamPlayerViewOpen, OnAddTeamPlayerViewOpen);
            Mediator.Register(MediatorGlobal.SelectedTeamUsers, OnSelectedTeamUsers);
        }

        private void OnSelectedTeamUsers(object obj)
        {
            _selectedUsers = (IList)obj;
        }

        private void OnAddTeamPlayerViewOpen(object obj)
        {
            if (ApplicationData.TeamControls != null && ApplicationData.TeamControls.Count > 0)
            {
                TeamControls = new ObservableCollection<Team>();

                foreach (var teamControl in ApplicationData.TeamControls)
                {
                    var team = new Team()
                    {
                        TeamName = String.Copy(teamControl.TeamName)
                    };
                    teamControl.Players.ToList().ForEach(x => team.Players.Add(x));
                    team.TeamChanged += OnTeamChanged;
                    TeamControls.Add(team);
                }
            }
            else
            {
                TeamControls = new ObservableCollection<Team>();
                var team = new Team();
                team.TeamChanged += OnTeamChanged;
                TeamControls.Add(team);
            }

            Users = new ObservableCollection<User>();
            var myUsers = DbHandler.GetUsers();

            foreach (var user in myUsers)
            {
                if (!TeamControls.Any(tm => tm.Players.Any(p => p.Name == user.Name)))
                {
                    Users.Add(user);
                }
            }

            TeamControls.CollectionChanged += (sender, args) =>
            {
                OnTeamChanged(this, null);
            };

            EditingVisibility = Visibility.Collapsed;
            _canOnAddUser = true;
            _canOnDeleteUser = true;
        }

        public ICommand OnClose
        {
            get
            {
                if (_onClose == null)
                    _onClose = new RelayCommand(
                        param => OnCloseCommand(),
                        param => CanOnCloseCommand()
                    );
                return _onClose;
            }
        }

        private bool CanOnCloseCommand()
        {
            return true;
        }

        private void OnCloseCommand()
        {
            Mediator.NotifyColleagues(MediatorGlobal.AddTeamPlayerViewClose, null);
        }

        public ICommand OnSaveTeam
        {
            get
            {
                if (_onSaveTeam == null)
                    _onSaveTeam = new RelayCommand(
                        param => OnSaveTeamCommand(),
                        param => CanOnSaveTeamCommand()
                    );
                return _onSaveTeam;
            }
        }

        private bool CanOnSaveTeamCommand()
        {
            return true;
        }

        private void OnSaveTeamCommand()
        {
            if (!CheckTeamData())
            {
                Mediator.NotifyColleagues(MediatorGlobal.ErrorDialog, new List<string>()
                {
                    "Unvollständige Daten",
                    "Für jedes Team muss ein eindeutiger Name eingetragen werden.\n" +
                    "Leere Teams sind nicht erlaubt!"
                });
                return;
            }

            ApplicationData.TeamControls = TeamControls.ToList();
            OnAddTeamPlayerViewOpen(null);
            EditingVisibility = Visibility.Collapsed;
            _canOnAddUser = true;
            _canOnDeleteUser = true;
        }

        public ICommand OnDiscardTeam
        {
            get
            {
                if (_onDiscardTeam == null)
                    _onDiscardTeam = new RelayCommand(
                        param => OnDiscardTeamCommand(),
                        param => CanOnDiscardTeamCommand()
                    );
                return _onDiscardTeam;
            }
        }

        private bool CanOnDiscardTeamCommand()
        {
            return true;
        }

        private void OnDiscardTeamCommand()
        {
            OnAddTeamPlayerViewOpen(null);
        }

        public ICommand OnAddUser
        {
            get
            {
                if (_onAddUser == null)
                    _onAddUser = new RelayCommand(
                        param => OnAddUserCommand(),
                        param => CanOnAddUserCommand()
                    );
                return _onAddUser;
            }
        }

        private bool CanOnAddUserCommand()
        {
            return _canOnAddUser;
        }

        private void OnAddUserCommand()
        {
            Mediator.NotifyColleagues(MediatorGlobal.AddUser, MediatorGlobal.AddTeamPlayerViewOpen);
        }

        public ICommand OnDeleteUser
        {
            get
            {
                if (_onDeleteUser == null)
                    _onDeleteUser = new RelayCommand(
                        param => OnDeleteUserCommand(),
                        param => CanOnDeleteUserCommand()
                    );
                return _onDeleteUser;
            }
        }

        private bool CanOnDeleteUserCommand()
        {
            return _canOnDeleteUser;
        }

        private void OnDeleteUserCommand()
        {
            if (!ApplicationData.IsAdminLoggedIn)
            {
                Mediator.NotifyColleagues(MediatorGlobal.ErrorDialog, new List<string>()
                {
                    "Zugriff verweigert",
                    "Um diesen Vorgang durchzuführen muss man angemeldet sein!"
                });
                return;
            }

            try
            {
                var listOfUsers = Users.Where(x => _selectedUsers.Contains(x)).ToList();
                listOfUsers.ForEach(u => u.UsersToDelete = true);
                DbHandler.DeleteUsers(listOfUsers);
                OnAddTeamPlayerViewOpen(null);
            }
            catch (Exception e)
            {
                LogHandler.WriteSystemLog("OnExceptionEvent:" + e, LogLevel.Error);
            }
        }

        public ICommand OnAddTeam
        {
            get
            {
                if (_onAddTeam == null)
                    _onAddTeam = new RelayCommand(
                        param => OnAddTeamCommand(),
                        param => CanOnAddTeamCommand()
                    );
                return _onAddTeam;
            }
        }

        private bool CanOnAddTeamCommand()
        {
            return true;
        }

        private void OnAddTeamCommand()
        {
            var team = new Team();
            team.TeamChanged += OnTeamChanged;
            TeamControls.Add(team);
            _canOnDeleteTeam = true;
        }

        public ICommand OnDeleteTeam
        {
            get
            {
                if (_onDeleteTeam == null)
                    _onDeleteTeam = new RelayCommand(
                        param => OnDeleteTeamCommand(),
                        param => CanOnDeleteTeamCommand()
                    );
                return _onDeleteTeam;
            }
        }

        private bool CanOnDeleteTeamCommand()
        {
            return _canOnDeleteTeam;
        }

        private void OnDeleteTeamCommand()
        {
            if (TeamControls.Count <= 1)
            {
                _canOnDeleteTeam = false;
                return;
            }

            Team lastTeam = TeamControls[TeamControls.Count - 1];
            if (lastTeam.Players.Count > 0)
            {
                foreach (var player in lastTeam.Players)
                {
                    Users.Add(player);
                }   
            }
            TeamControls.Remove(lastTeam);
        }

        private void OnTeamChanged(object sender, EventArgs e)
        {
            EditingVisibility = Visibility.Visible;
            _canOnAddUser = false;
            _canOnDeleteUser = false;
        }

        private bool CheckTeamData()
        {
            // Check if there are empty TeamNames
            if (TeamControls.Any(x => x.TeamName == null || x.TeamName.Length <= 0))
            {
                return false;
            }

            // Check if all TeamNames are unique
            var uniqueNames = TeamControls.Select(x => x.TeamName);
            if (uniqueNames.Distinct().Count() < TeamControls.Count)
            {
                return false;
            }

            if (TeamControls.Any(x => x.Players.Count <= 0))
            {
                return false;
            }

            return true;
        }
    }

    public class Team : INotifyPropertyChanged
    {
        public event EventHandler TeamChanged;

        protected virtual void OnTeamChanged()
        {
            TeamChanged?.Invoke(this, null);
        }

        public ObservableCollection<User> Players { get; set; } = new ObservableCollection<User>();
        public string TeamName { get; set; }

        public Team()
        {
            Players.CollectionChanged += (sender, args) =>
            {
                OnTeamChanged();
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnTeamChanged();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
