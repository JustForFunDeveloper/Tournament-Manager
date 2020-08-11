using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using HandicapBewerb.Core.Data;
using HandicapBewerb.Core.Handler;
using HandicapBewerb.DataModels.DbModels;
using HandicapBewerb.ViewModels.Handler;

namespace HandicapBewerb.ViewModels
{
    public class AddPlayerViewModel : INotifyPropertyChanged
    {
        private ICommand _onClose;
        private ICommand _onAddUser;
        private ICommand _onDeleteUser;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<User> Users { get; set; }

        public AddPlayerViewModel()
        {
            Mediator.Register(MediatorGlobal.AddPlayerViewOpen, OnAddPlayerViewOpen);
        }

        private void OnAddPlayerViewOpen(object obj)
        {
            Users = new ObservableCollection<User>(DBHandler.GetUsers());

            var userDataControls = ApplicationData.UserDataControls;
            if (userDataControls != null)
            {
                foreach (var userDataControl in userDataControls)
                {
                    Users.First(u => u.UserId.Equals(userDataControl.UserId)).IsSelected = true;
                }
            }
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
            List<int> userIds = new List<int>();
            foreach (var user in Users)
            {
                if (user.IsSelected)
                {
                    userIds.Add(user.UserId);
                }
            }

            ApplicationData.CurrentSelectedUser = userIds;
            Mediator.NotifyColleagues(MediatorGlobal.AddPlayerViewClose, null);
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
            Mediator.NotifyColleagues(MediatorGlobal.AddUser, MediatorGlobal.AddPlayerViewOpen);
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
                OnAddPlayerViewOpen(null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
