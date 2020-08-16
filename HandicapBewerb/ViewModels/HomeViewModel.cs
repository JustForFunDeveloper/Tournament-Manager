using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TournamentManager.Core.Data;
using TournamentManager.Core.Handler;
using TournamentManager.DataModels.DbModels;
using TournamentManager.ViewModels.Handler;
using TournamentManager.Views.UserControls;
using JetBrains.Annotations;

namespace TournamentManager.ViewModels
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class HomeViewModel : INotifyPropertyChanged
    {
        // ReSharper disable once NotAccessedField.Local
        public event PropertyChangedEventHandler PropertyChanged;

        #region private Member

        private ICommand _searchLocalFolder;
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

        public ObservableCollection<UserDataControl> UserDataControls { get; set; }

        public HomeViewModel()
        {
            Mediator.Register(MediatorGlobal.AddPlayerViewClose, OnAddPlayerViewClose);
            Mediator.Register(MediatorGlobal.RefreshView, OnRefreshView);
            UserDataControls = new ObservableCollection<UserDataControl>();

            IsAddPlayersEnabled = true;
            IsSaveAllEnabled = true;
            IsSortPositionsEnabled = true;
        }

        private void OnRefreshView(object obj)
        {
            var view = (MainViewModel.ApplicationViewEnum)obj;
            if (view.Equals(MainViewModel.ApplicationViewEnum.HomeView))
            {
               OnAddPlayerViewClose(null);
            }
        }

        private void OnAddPlayerViewClose(object obj)
        {
            try
            {
                if (ApplicationData.CurrentSelectedUser != null)
                {
                    InitUserData(ApplicationData.CurrentSelectedUser);
                }
                else
                {
                    UserDataControls.Clear();
                    if (ApplicationData.UserDataControls != null)
                    {
                        ApplicationData.UserDataControls.Clear();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnCustomPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region IconBar

        public ICommand SearchLocalFolder
        {
            get
            {
                if (_searchLocalFolder == null)
                    _searchLocalFolder = new RelayCommand(
                        param => SearchLocalFolderCommand(),
                        param => CanSearchLocalFolderCommand()
                    );
                return _searchLocalFolder;
            }
        }

        private bool CanSearchLocalFolderCommand()
        {
            return true;
        }

        private void SearchLocalFolderCommand()
        {
        }

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
            ApplicationData.UserDataControls = null;
            ApplicationData.CurrentSelectedUser = null;
            UserDataControls.Clear();

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
            Mediator.NotifyColleagues(MediatorGlobal.AddPlayerViewOpen, null);
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
                DbHandler.SaveMatch(ApplicationData.UserDataControls);
                IsSaveAllEnabled = false;
                IsAddPlayersEnabled = false;
                IsSortPositionsEnabled = false;
                foreach (var userDataControl in UserDataControls)
                {
                    userDataControl.IsInputEnabled = false;
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
                Console.ResetColor();
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

        private void InitUserData(List<int> userIds)
        {
            List<UserDataControl> userDataControls = new List<UserDataControl>();

            List<User> users = DbHandler.GetUsers();

            foreach (var user in users)
            {
                if (userIds.Contains(user.UserId))
                {
                    UserDataControl userDataControl;
                    if (ApplicationData.UserDataControls == null ||
                        !ApplicationData.UserDataControls.Any(u => u.UserId.Equals(user.UserId)))
                    {
                        userDataControl = new UserDataControl(user.UserId);
                        EditUserDataControlValues(userDataControl, user);
                    }
                    else
                    {
                        userDataControl = ApplicationData.UserDataControls.Single(u => u.UserId.Equals(user.UserId));
                        EditUserDataControlValues(userDataControl, user);
                    }

                    userDataControls.Add(userDataControl);
                }
            }
            UserDataControls = new ObservableCollection<UserDataControl>(userDataControls);
            ApplicationData.UserDataControls = userDataControls;
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
            var userDataControls = ApplicationData.UserDataControls.OrderByDescending(u => u.SortResult);
            int position = 1;
            foreach (var userDataControl in userDataControls)
            {
                userDataControl.Position = position.ToString();
                position++;
            }

            UserDataControls = new ObservableCollection<UserDataControl>(userDataControls);
        }
    }
}