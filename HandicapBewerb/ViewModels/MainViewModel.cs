using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TournamentManager.Core.Data;
using TournamentManager.ViewModels.Handler;
using JetBrains.Annotations;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using PropertyChanged;

namespace TournamentManager.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region private Member

        private ICommand _openMenu;
        private ICommand _itemClick;
        private ICommand _optionItemClick;
        private ICommand _logIn;

        private HamburgerMenuItemCollection _menuItems;
        private HamburgerMenuItemCollection _menuOptionItems;

        public bool IsPaneOpened { get; set; }
        public int SelectedIndex { get; set; }
        public string LoginText { get; set; }

        #endregion

        public enum ApplicationViewEnum
        {
            HomeView = 0,
            TeamHomeView = 1,
            AddPlayerView = 2,
            UserDataView = 3,
            MatchDataView = 4,
            PlayerStatisticView = 5,
            AddTeamPlayerView = 6,
            TeamMatchDataView = 7
        }

        public MainViewModel()
        {
            CreateMenuItems();
            Mediator.Register(MediatorGlobal.AddPlayerViewOpen, OnAddPlayerViewOpen);
            Mediator.Register(MediatorGlobal.AddTeamPlayerViewOpen, AddTeamPlayerViewOpen);
            Mediator.Register(MediatorGlobal.AddPlayerViewClose, OnAddPlayerViewClose);
            Mediator.Register(MediatorGlobal.AddTeamPlayerViewClose, AddTeamPlayerViewClose);
            Mediator.Register(MediatorGlobal.LogInSuccessfull, OnLogInSuccessfull);
            Mediator.Register(MediatorGlobal.OnBackToUserDataView, OnBackToUserDataView);
            Mediator.Register(MediatorGlobal.OnMatchStatistics, OnStatisticsWindow);
            Mediator.Register(MediatorGlobal.OnRoundStatistics, OnStatisticsWindow);

            LoginText = "Anmelden";
        }

        private void OnLogInSuccessfull(object obj)
        {
            ApplicationData.IsAdminLoggedIn = true;
            LoginText = "Abmelden";
        }

        private void OnAddPlayerViewClose(object obj)
        {
            SelectedIndex = (int)ApplicationViewEnum.HomeView;
        }

        private void OnAddPlayerViewOpen(object obj)
        {
            SelectedIndex = (int)ApplicationViewEnum.AddPlayerView;
        }

        private void OnBackToUserDataView(object obj)
        {
            SelectedIndex = (int)ApplicationViewEnum.UserDataView;
        }

        private void OnStatisticsWindow(object obj)
        {
            SelectedIndex = (int)ApplicationViewEnum.PlayerStatisticView;
        }

        private void AddTeamPlayerViewOpen(object obj)
        {
            SelectedIndex = (int)ApplicationViewEnum.AddTeamPlayerView;
        }

        private void AddTeamPlayerViewClose(object obj)
        {
            SelectedIndex = (int)ApplicationViewEnum.TeamHomeView;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnCustomPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Hamburger Menu

        private void CreateMenuItems()
        {
            MenuItems = new HamburgerMenuItemCollection
            {
                new HamburgerMenuIconItem()
                {
                    Icon = new PackIconMaterial() {Kind = PackIconMaterialKind.Tournament},
                    Label = "Handicap Solo Turnier",
                    ToolTip = "Die Hautptanzeige für diesen Spielmodus.",
                    Tag = new HomeViewModel()
                },
                new HamburgerMenuIconItem()
                {
                    Icon = new PackIconMaterial() {Kind = PackIconMaterialKind.Tournament},
                    Label = "Handicap Team Turnier",
                    ToolTip = "Die Hautptanzeige für diesen Team Spielmodus.",
                    Tag = new TeamHomeViewModel()
                },
                new HamburgerMenuIconItem()
                {
                    Icon = new PackIconMaterial() {Kind = PackIconMaterialKind.Play},
                    Label = "Spieler",
                    ToolTip = "Spieler hinzufügen.",
                    Tag = new AddPlayerViewModel(),
                    IsVisible = false
                },
                new HamburgerMenuIconItem()
                {
                    Icon = new PackIconMaterial() {Kind = PackIconMaterialKind.Account},
                    Label = "Schützen",
                    ToolTip = "Schützen bearbeiten.",
                    Tag = new UserDataModel()
                },
                new HamburgerMenuIconItem()
                {
                    Icon = new PackIconMaterial() {Kind = PackIconMaterialKind.Bullseye},
                    Label = "Matches",
                    ToolTip = "Matches ansehen.",
                    Tag = new MatchDataModel()
                },
                new HamburgerMenuIconItem()
                {
                    Icon = new PackIconMaterial() {Kind = PackIconMaterialKind.Earth},
                    Label = "Spieler Statistik",
                    ToolTip = "Statistiken ansehen.",
                    Tag = new PlayerStatisticModel(),
                    IsVisible = false
                },
                new HamburgerMenuIconItem()
                {
                    Icon = new PackIconMaterial() {Kind = PackIconMaterialKind.Earth},
                    Label = "TeamSpieler",
                    ToolTip = "TeamSpieler hinzufügen.",
                    Tag = new TeamAddPlayerViewModel(),
                    IsVisible = false
                },
                new HamburgerMenuIconItem()
                {
                    Icon = new PackIconMaterial() {Kind = PackIconMaterialKind.Bullseye},
                    Label = "Team Matches",
                    ToolTip = "Team Matches ansehen.",
                    Tag = new TeamMatchDataModel()
                }
            };

            MenuOptionItems = new HamburgerMenuItemCollection
            {
                new HamburgerMenuIconItem()
                {
                    Icon = new PackIconMaterial() {Kind = PackIconMaterialKind.InformationVariant},
                    Label = "Hilfe",
                    ToolTip = "Über diese Software",
                    Tag = new AboutViewModel()
                }
            };
        }

        [DoNotNotify]
        public HamburgerMenuItemCollection MenuItems
        {
            get => _menuItems;
            set
            {
                if (Equals(value, _menuItems)) return;
                _menuItems = value;
                OnCustomPropertyChanged();
            }
        }

        [DoNotNotify]
        public HamburgerMenuItemCollection MenuOptionItems
        {
            get { return _menuOptionItems; }
            set
            {
                if (Equals(value, _menuOptionItems)) return;
                _menuOptionItems = value;
                OnCustomPropertyChanged();
            }
        }

        public ICommand OpenMenu
        {
            get
            {
                if (_openMenu == null)
                {
                    _openMenu = new RelayCommand(
                        param => SaveOpenMenuCommand(),
                        param => CanSaveOpenMenuCommand()
                    );
                }

                return _openMenu;
            }
        }

        private bool CanSaveOpenMenuCommand()
        {
            return true;
        }

        private void SaveOpenMenuCommand()
        {
            IsPaneOpened = !IsPaneOpened;
        }

        public ICommand ItemClick
        {
            get
            {
                if (_itemClick == null)
                {
                    _itemClick = new RelayCommand<int>(
                        param => ItemClickCommand(),
                        param => CanItemClickCommand()
                    );
                }

                return _itemClick;
            }
        }

        private bool CanItemClickCommand()
        {
            return true;
        }

        private void ItemClickCommand()
        {
            IsPaneOpened = false;

            if (Enum.TryParse(SelectedIndex.ToString(),out ApplicationViewEnum clickedView))
            {
                Mediator.NotifyColleagues(MediatorGlobal.RefreshView, clickedView);
            }
        }

        public ICommand OptionItemClick
        {
            get
            {
                if (_optionItemClick == null)
                {
                    _optionItemClick = new RelayCommand<int>(
                        param => OptionItemClickCommand(),
                        param => CanOptionItemClickCommand()
                    );
                }

                return _optionItemClick;
            }
        }

        private bool CanOptionItemClickCommand()
        {
            return true;
        }

        private void OptionItemClickCommand()
        {
            IsPaneOpened = false;
        }

        public ICommand LogIn
        {
            get
            {
                if (_logIn == null)
                {
                    _logIn = new RelayCommand(
                        param => LogInCommand(),
                        param => CanLogInCommand()
                    );
                }

                return _logIn;
            }
        }

        private bool CanLogInCommand()
        {
            return true;
        }

        private void LogInCommand()
        {
            if (ApplicationData.IsAdminLoggedIn)
            {
                ApplicationData.IsAdminLoggedIn = false;
                LoginText = "Anmelden";
            }
            else
            {
                Mediator.NotifyColleagues(MediatorGlobal.LoginDialog, null);
            }
        }
        #endregion
    }
}
