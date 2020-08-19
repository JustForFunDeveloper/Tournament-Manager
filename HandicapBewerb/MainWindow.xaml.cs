using System;
using System.Collections.Generic;
using System.IO;
using AutoUpdaterDotNET;
using TournamentManager.Core.Data;
using TournamentManager.Core.Handler;
using TournamentManager.DataModels.DbModels;
using TournamentManager.ViewModels.Handler;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace TournamentManager
{
    // ReSharper disable once RedundantExtendsListEntry
    public partial class MainWindow : MetroWindow
    {
        private static MetroWindow _mainView;
        private bool _closeMe;

        public MainWindow()
        {
            InitializeComponent();
            _mainView = this;
            SetUpAutoUpdater();
            Mediator.Register(MediatorGlobal.AddUser, OnAddUser);
            Mediator.Register(MediatorGlobal.AddRound, OnAddRound);
            Mediator.Register(MediatorGlobal.ErrorDialog, OnErrorDialog);
            Mediator.Register(MediatorGlobal.LoginDialog, OnLoginDialog);
            Closing += WindowClosing;
        }

        private void SetUpAutoUpdater()
        {
            try
            {
                if (File.Exists(ApplicationData.UpdateFilePathFull))
                {
                    File.Delete(ApplicationData.UpdateFilePathFull);
                }
                string jsonPath = Path.Combine(Environment.CurrentDirectory, "autoUpdaterSettings.json");
                AutoUpdater.PersistenceProvider = new JsonFilePersistenceProvider(jsonPath);
                AutoUpdater.DownloadPath = ApplicationData.DownloadPath;

                AutoUpdater.Start(ApplicationData.UpdateUrl);
            }
            catch (Exception e)
            {
                LogHandler.WriteSystemLog("SetUpAutoUpdater:" + e, LogLevel.Error);
            }
        }

        private async void OnLoginDialog(object obj)
        {
            LoginDialogData result = await this.ShowLoginAsync("Anmelden als Administrator", "Bitte gib dein Passwort ein!",
                new LoginDialogSettings
                {
                    ColorScheme = this.MetroDialogOptions.ColorScheme,
                    ShouldHideUsername = true,
                    NegativeButtonText = "Abbrechen"
                });
            if (result == null)
            {
                //User pressed cancel
            }
            else
            {
                if (result.Password.Equals(ApplicationData.Password))
                {
                    ApplicationData.IsAdminLoggedIn = true;
                    Mediator.NotifyColleagues(MediatorGlobal.LogInSuccessfull, null);
                }
                else
                {
                    OnErrorDialog(new List<string>()
                    {
                        "Fehler",
                        "Falsches Passwort!"
                    });
                }
            }
        }

        private async void OnAddUser(object obj)
        {

            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Ok",
                NegativeButtonText = "Abbrechen",
                AnimateShow = true,
                AnimateHide = false
            };
            var result = await this.ShowInputAsync("Hallo", "Bitte gib deinen Namen ein!", mySettings);

            if (result == null) //user pressed cancel
                return;

            try
            {
                DbHandler.AddUser(result);
                Mediator.NotifyColleagues((string)obj, null);
            }
            catch (Exception)
            {
                OnErrorDialog(new List<string>()
                {
                    "Fehler",
                    "Spieler existiert bereits, bitte wähle einen anderen Namen!"
                });
            }
        }

        private async void OnAddRound(object obj)
        {
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Ok",
                NegativeButtonText = "Abbrechen",
                AnimateShow = true,
                AnimateHide = false
            };
            var result = await this.ShowInputAsync("Hallo", "Bitte gib deine Punkte ein!", mySettings);

            if (result == null) //user pressed cancel
                return;

            try
            {
                Mediator.NotifyColleagues((string)obj, new Round()
                {
                    Points = Double.Parse(result),
                    Date = DateTime.Now
                });
            }
            catch (Exception)
            {
                OnErrorDialog(new List<string>()
                {
                    "Fehler",
                    "Gib bitte einen korrekten Wert an!"
                });
            }
        }

        private async void OnErrorDialog(object obj)
        {
            var list = obj as List<string>;

            if (list == null || list.Count != 2)
                return;

            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Ok",
                AnimateShow = true,
                AnimateHide = false
            };

            await this.ShowMessageAsync(
                list[0],
                list[1],
                MessageDialogStyle.Affirmative, mySettings);
        }

        private async void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (e.Cancel) return;
            e.Cancel = !_closeMe;
            if (_closeMe) return;

            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Beenden",
                NegativeButtonText = "Abbrechen",
                AnimateShow = true,
                AnimateHide = false
            };
            var result = await this.ShowMessageAsync(
                "Tournament Manager beenden",
                "Bist du dir sicher, dass du das Programm beenden willst?",
                MessageDialogStyle.AffirmativeAndNegative, mySettings);

            _closeMe = result == MessageDialogResult.Affirmative;

            if (_closeMe) Close();
        }
    }
}
