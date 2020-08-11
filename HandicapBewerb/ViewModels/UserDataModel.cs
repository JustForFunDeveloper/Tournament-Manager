using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using HandicapBewerb.Core.Data;
using HandicapBewerb.Core.Handler;
using HandicapBewerb.DataModels.DbModels;
using HandicapBewerb.ViewModels.Handler;

namespace HandicapBewerb.ViewModels
{
    public class UserDataModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<Round> _rounds;
        private User _selection;
        private List<Round> _roundsToDelete = new List<Round>();
        private User _editedUser;

        private ICommand _onAddUser;
        private ICommand _onDeleteUser;
        private ICommand _onSaveUser;
        private ICommand _onDiscardUser;
        private ICommand _onAddRound;
        private ICommand _onDeleteRound;

        public bool IsUsersReadOnly { get; set; }
        public bool IsUsersEnabled { get; set; }
        public bool IsRoundsReadOnly { get; set; }
        public bool IsRoundEditingEnabled { get; set; }
        public bool IsUserEditingEnabled { get; set; }
        public Visibility EditingVisibility { get; set; }
        public ObservableCollection<User> Users { get; set; }
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

        public User Selection
        {
            get
            {
                return _selection;
            }
            set
            {
                if (value != null)
                {
                    Rounds = new ObservableCollection<Round>(DBHandler.GetUserRounds(value));
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
            Mediator.Register(MediatorGlobal.UserDataViewOpen, OnUserDataViewOpen);
            Mediator.Register(MediatorGlobal.PlayerOnBegininningEdit, OnPlayerOnBegininningEdit);
            Mediator.Register(MediatorGlobal.PlayerEditCancel, OnPlayerEditCancel);
            Mediator.Register(MediatorGlobal.LogInSuccessfull, OnLogInSuccessfull);
            Mediator.Register(MediatorGlobal.OnAddedRound, OnAddedRound);
            Mediator.Register(MediatorGlobal.RefreshView, OnRefreshView);
            Mediator.Register(MediatorGlobal.PlayerEditUserComit, OnPlayerEditUserComit);
            OnUserDataViewOpen(null);
        }

        private void OnPlayerEditUserComit(object obj)
        {
            _editedUser = Users.Single(u => u.UserId.Equals((int) obj));

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
            Users = new ObservableCollection<User>(DBHandler.GetUsersIncludingRounds());
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

            IsRoundEditingEnabled = true;
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
            Selection.Rounds = Rounds;
            DBHandler.UpdateUser(Selection);
            if (_roundsToDelete.Count > 0)
            {
                DBHandler.RemoveRounds(_roundsToDelete, Selection);
                _roundsToDelete = new List<Round>();
            }

            if (_editedUser != null)
            {
                DBHandler.UpdateUser(_editedUser);
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
                    "Um diesen Vorgang durchzführen muss man angemeldet sein!"
                });
                return;
            }

            try
            {
                DBHandler.deleteUsers(Users.ToList());
                OnUserDataViewOpen(null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
                Console.WriteLine(e);
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
                    "Um diesen Vorgang durchzführen muss man angemeldet sein!"
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
                Console.WriteLine(e);
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
}
