﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using HandicapBewerb.Core.Data;
using HandicapBewerb.ViewModels.Handler;
using JetBrains.Annotations;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using PropertyChanged;

namespace HandicapBewerb.ViewModels
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
            UserDataView = 2,
            MatchDataView = 3
        }

        public MainViewModel()
        {
            CreateMenuItems();
            Mediator.Register(MediatorGlobal.AddPlayerViewOpen, OnAddPlayerViewOpen);
            Mediator.Register(MediatorGlobal.AddPlayerViewClose, OnAddPlayerViewClose);
            Mediator.Register(MediatorGlobal.LogInSuccessfull, OnLogInSuccessfull);

            LoginText = "Anmelden";
        }

        private void OnLogInSuccessfull(object obj)
        {
            ApplicationData.IsAdminLoggedIn = true;
            LoginText = "Abmelden";
        }

        private void OnAddPlayerViewClose(object obj)
        {
            SelectedIndex = 0;
        }

        private void OnAddPlayerViewOpen(object obj)
        {
            SelectedIndex = 1;
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
                    Label = "Handicap Turnier",
                    ToolTip = "Die Hautptanzeige für diesen Spielmodus.",
                    Tag = new HomeViewModel(),
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
            Console.WriteLine(SelectedIndex);

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