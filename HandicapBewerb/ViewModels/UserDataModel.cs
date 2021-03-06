﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TournamentManager.Core.Data;
using TournamentManager.Core.Handler;
using TournamentManager.DataModels.DbModels;
using TournamentManager.ViewModels.Handler;

namespace TournamentManager.ViewModels
{
    public class UserDataModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<Round> _rounds;
        private CalculatedUser _selection;
        private List<Round> _roundsToDelete = new List<Round>();
        private User _editedUser;

        private ICommand _onAddUser;
        private ICommand _onDeleteUser;
        private ICommand _onSaveUser;
        private ICommand _onDiscardUser;
        private ICommand _onAddRound;
        private ICommand _onDeleteRound;
        private ICommand _onRoundStatistic;
        private ICommand _onMatchStatistic;
        private ICommand _onTeamMatchStatistic;

        public bool IsUsersReadOnly { get; set; }
        public bool IsUsersEnabled { get; set; }
        public bool IsRoundsReadOnly { get; set; }
        public bool IsRoundEditingEnabled { get; set; }
        public bool IsUserEditingEnabled { get; set; }
        public Visibility EditingVisibility { get; set; }
        public ObservableCollection<CalculatedUser> Users { get; set; }
        public Round SelectedRound { get; set; }

        public ObservableCollection<Round> Rounds {
            get
            {
                return _rounds;
            }
            set
            {
                _rounds = value;
            }
        }

        public CalculatedUser Selection
        {
            get
            {
                return _selection;
            }
            set
            {
                if (value != null)
                {
                    IsRoundEditingEnabled = true;
                    Rounds = new ObservableCollection<Round>(DbHandler.GetUserRounds(value.User));
                }
                else
                {
                    Rounds = new ObservableCollection<Round>();
                }
                _selection = value;
            }
        }

        public UserDataModel()
        {
            Mediator.Register(MediatorGlobal.PlayerEditComit, OnPlayerEditComit);
            Mediator.Register(MediatorGlobal.UserDataViewOpen, OnUserDataViewOpen);
            Mediator.Register(MediatorGlobal.PlayerOnBegininningEdit, OnPlayerOnBegininningEdit);
            Mediator.Register(MediatorGlobal.PlayerEditCancel, OnPlayerEditCancel);
            Mediator.Register(MediatorGlobal.LogInSuccessfull, OnLogInSuccessfull);
            Mediator.Register(MediatorGlobal.OnAddedRound, OnAddedRound);
            Mediator.Register(MediatorGlobal.RefreshView, OnRefreshView);
            Mediator.Register(MediatorGlobal.PlayerEditUserComit, OnPlayerEditUserComit);
        }

        private void OnPlayerEditUserComit(object obj)
        {
            _editedUser = Users.Single(u => u.User.UserId.Equals((int) obj)).User;

            EditingVisibility = Visibility.Visible;
            IsRoundsReadOnly = true;
            IsRoundEditingEnabled = true;
            IsUsersEnabled = false;
        }

        private void OnRefreshView(object obj)
        {
            var view = (MainViewModel.ApplicationViewEnum) obj;
            if (view.Equals(MainViewModel.ApplicationViewEnum.UserDataView))
            {
                OnUserDataViewOpen(null);
            }
        }

        private void OnAddedRound(object obj)
        {
            var round = (Round) obj;
            Rounds.Add(round);
            OnPlayerEditComit(null);
        }

        private void OnLogInSuccessfull(object obj)
        {
            IsUsersReadOnly = false;
            IsRoundsReadOnly = false;
        }

        private void OnPlayerEditCancel(object obj)
        {
            IsRoundEditingEnabled = true;
            IsUserEditingEnabled = true;
            IsUsersEnabled = true;
        }

        private void OnPlayerOnBegininningEdit(object obj)
        {
            IsRoundEditingEnabled = false;
            IsUserEditingEnabled = false;
            IsUsersEnabled = false;
        }

        private void OnUserDataViewOpen(object obj)
        {
            var users = DbHandler.GetUsersIncludingRounds();

            List<CalculatedUser> calculatedUsers = new List<CalculatedUser>();
            foreach (var user in users)
            {
                double oldNullValue = Double.NaN;
                double currentNullValue = Double.NaN;
                if (user.Rounds.Count > 0)
                {
                    try
                    {
                        int skipValue = 1;
                        int takeValue = 3;
                        if (user.Rounds.Count < 4)
                        {
                            skipValue = 0;
                            takeValue = user.Rounds.Count;
                        }

                        oldNullValue = user.Rounds.OrderByDescending(r => r.Date)
                            .Skip(skipValue).Take(takeValue).Select(x => x.Points).Average();
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        int takeValue = 3;

                        if (user.Rounds.Count < 3)
                        {
                            takeValue = user.Rounds.Count;
                        }

                        currentNullValue = user.Rounds.OrderByDescending(r => r.Date)
                            .Take(3).Select(x => x.Points).Average();
                    }
                    catch (Exception)
                    {
                    }
                }

                calculatedUsers.Add(new CalculatedUser()
                {
                    User = user,
                    OldNullValue = oldNullValue,
                    CurrentNullValue = currentNullValue
                });

            }
            Users = new ObservableCollection<CalculatedUser>(calculatedUsers);
            Rounds = new ObservableCollection<Round>();

            if (ApplicationData.IsAdminLoggedIn)
            {
                IsUsersReadOnly = false;
                IsRoundsReadOnly = false;
            }
            else
            {
                IsUsersReadOnly = true;
                IsRoundsReadOnly = true;
            }

            IsRoundEditingEnabled = false;
            IsUserEditingEnabled = true;
            IsUsersEnabled = true;
            EditingVisibility = Visibility.Collapsed;
        }

        public ICommand OnSaveUser
        {
            get
            {
                if (_onSaveUser == null)
                    _onSaveUser = new RelayCommand(
                        param => OnSaveUserCommand(),
                        param => CanOnSaveUserCommand()
                    );
                return _onSaveUser;
            }
        }

        private bool CanOnSaveUserCommand()
        {
            return true;
        }

        private void OnSaveUserCommand()
        {
            Selection.User.Rounds = Rounds;
            DbHandler.UpdateUser(Selection.User);
            if (_roundsToDelete.Count > 0)
            {
                DbHandler.RemoveRounds(_roundsToDelete, Selection.User);
                _roundsToDelete = new List<Round>();
            }

            if (_editedUser != null)
            {
                DbHandler.UpdateUser(_editedUser);
                _editedUser = null;
            }

            ApplicationData.UserDataChanged = true;
            OnUserDataViewOpen(null);
        }

        public ICommand OnDiscardUser
        {
            get
            {
                if (_onDiscardUser == null)
                    _onDiscardUser = new RelayCommand(
                        param => OnDiscardUserCommand(),
                        param => CanOnDiscardUserCommand()
                    );
                return _onDiscardUser;
            }
        }

        private bool CanOnDiscardUserCommand()
        {
            return true;
        }

        private void OnDiscardUserCommand()
        {
            OnUserDataViewOpen(null);
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
            return true;
        }

        private void OnAddUserCommand()
        {
            Mediator.NotifyColleagues(MediatorGlobal.AddUser, MediatorGlobal.UserDataViewOpen);
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
            return true;
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
                DbHandler.DeleteUsers(Users.Select(x => x.User).ToList());
                OnUserDataViewOpen(null);
            }
            catch (Exception e)
            {
                LogHandler.WriteSystemLog("OnDeleteUserCommand:" + e, LogLevel.Error);
            }
        }

        public ICommand OnAddRound
        {
            get
            {
                if (_onAddRound == null)
                    _onAddRound = new RelayCommand(
                        param => OnAddRoundCommand(),
                        param => CanOnAddRoundCommand()
                    );
                return _onAddRound;
            }
        }

        private bool CanOnAddRoundCommand()
        {
            return true;
        }

        private void OnAddRoundCommand()
        {
            try
            {
                Mediator.NotifyColleagues(MediatorGlobal.AddRound, MediatorGlobal.OnAddedRound);
            }
            catch (Exception e)
            {
                LogHandler.WriteSystemLog("OnAddRoundCommand:" + e, LogLevel.Error);
            }
        }

        public ICommand OnDeleteRound
        {
            get
            {
                if (_onDeleteRound == null)
                    _onDeleteRound = new RelayCommand(
                        param => OnDeleteRoundCommand(),
                        param => CanOnDeleteRoundCommand()
                    );
                return _onDeleteRound;
            }
        }

        private bool CanOnDeleteRoundCommand()
        {
            return true;
        }

        private void OnDeleteRoundCommand()
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
                _roundsToDelete.Add(SelectedRound);
                Rounds.Remove(SelectedRound);
                OnPlayerEditComit(null);
            }
            catch (Exception e)
            {
                LogHandler.WriteSystemLog("OnDeleteRoundCommand:" + e, LogLevel.Error);
            }
        }

        public ICommand OnRoundStatistic
        {
            get
            {
                if (_onRoundStatistic == null)
                    _onRoundStatistic = new RelayCommand(
                        param => OnRoundStatisticCommand(),
                        param => CanOnRoundStatisticCommand()
                    );
                return _onRoundStatistic;
            }
        }

        private bool CanOnRoundStatisticCommand()
        {
            return true;
        }

        private void OnRoundStatisticCommand()
        {
            if (Selection != null)
            {
                Mediator.NotifyColleagues(MediatorGlobal.OnRoundStatistics, Selection.User.UserId);
            }
        }

        public ICommand OnMatchStatistic
        {
            get
            {
                if (_onMatchStatistic == null)
                    _onMatchStatistic = new RelayCommand(
                        param => OnMatchStatisticCommand(),
                        param => CanOnMatchStatisticCommand()
                    );
                return _onMatchStatistic;
            }
        }

        private bool CanOnMatchStatisticCommand()
        {
            return true;
        }

        private void OnMatchStatisticCommand()
        {
            if (Selection != null)
            {
                Mediator.NotifyColleagues(MediatorGlobal.OnMatchStatistics, Selection.User.UserId);
            }
        }

        public ICommand OnTeamMatchStatistic
        {
            get
            {
                if (_onTeamMatchStatistic == null)
                    _onTeamMatchStatistic = new RelayCommand(
                        param => OnTeamMatchStatisticCommand(),
                        param => CanOnTeamMatchStatisticCommand()
                    );
                return _onTeamMatchStatistic;
            }
        }

        private bool CanOnTeamMatchStatisticCommand()
        {
            return true;
        }

        private void OnTeamMatchStatisticCommand()
        {
            if (Selection != null)
            {
                Mediator.NotifyColleagues(MediatorGlobal.OnTeamMatchStatistics, Selection.User.UserId);
            }
        }

        private void OnPlayerEditComit(object obj)
        {
            EditingVisibility = Visibility.Visible;
            IsUsersReadOnly = true;
            IsUsersEnabled = false;
            IsRoundEditingEnabled = true;
            IsUserEditingEnabled = false;
        }
    }

    public class CalculatedUser
    {
        public User User { get; set; }
        public double OldNullValue { get; set; }
        public double CurrentNullValue { get; set; }
    }
}
