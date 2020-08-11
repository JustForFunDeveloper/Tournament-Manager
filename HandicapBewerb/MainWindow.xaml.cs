﻿ using System;
using System.Collections.Generic;
 using HandicapBewerb.Core.Data;
 using HandicapBewerb.Core.Handler;
 using HandicapBewerb.DataModels.DbModels;
 using HandicapBewerb.ViewModels.Handler;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace HandicapBewerb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class MainWindow : MetroWindow
    {
        private static MetroWindow _mainView;
        private bool _closeMe;

        public MainWindow()
        {
            InitializeComponent();
            _mainView = this;
            Mediator.Register(MediatorGlobal.AddUser, OnAddUser);
            Mediator.Register(MediatorGlobal.AddRound, OnAddRound);
            Mediator.Register(MediatorGlobal.ErrorDialog, OnErrorDialog);
            Mediator.Register(MediatorGlobal.LoginDialog, OnLoginDialog);
            Closing += WindowClosing;
        }

        private async void OnLoginDialog(object obj)
        {
            LoginDialogData result = await this.ShowLoginAsync("Anmelden als Administrator", "Bitte gib dein Passwort ein.",
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

        //private void OnCustomDialog(object obj)
        //{
        //    if (obj == null)
        //        return;
        //    List<string> list = obj as List<string>;

        //    if (list == null)
        //        return;

        //    if (list.Count != 3)
        //        return;

        //    CustomDialog(list[0], list[1], list[2]);
        //}

        private async void OnAddUser(object obj)
        {
            var result = await this.ShowInputAsync("Hallo", "Bitte gib den Namen ein.");

            if (result == null) //user pressed cancel
                return;

            try
            {
                DBHandler.AddUser(result);
                Mediator.NotifyColleagues((string)obj, null);
            }
            catch (Exception)
            {
                OnErrorDialog(new List<string>()
                {
                    "Fehler",
                    "Spieler existiert bereits, bitte wähle einen anderen Namen."
                });
            }
        }

        private async void OnAddRound(object obj)
        {
            var result = await this.ShowInputAsync("Hallo", "Bitte gib deine Punkte ein.");

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
                    "Gib bitte einen korrekten Wert"
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
                "Handicap Statistik beenden",
                "Bist du dir sicher, dass du das Programm beenden willst?",
                MessageDialogStyle.AffirmativeAndNegative, mySettings);

            _closeMe = result == MessageDialogResult.Affirmative;

            if (_closeMe) Close();
        }

        public static MetroWindow GetInstance
        {
            get => _mainView;
        }

        private void Slider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            Mediator.NotifyColleagues(MediatorGlobal.SliderRateValueChanged, (int)e.NewValue);
        }

        private void EpisodeSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            Mediator.NotifyColleagues(MediatorGlobal.EpisodeSliderRateValueChanged, (int)e.NewValue);
        }

        //private async void CustomDialog(string identifier, string title, string message)
        //{
        //    var mySettings = new MetroDialogSettings()
        //    {
        //        AffirmativeButtonText = "Ok",
        //        NegativeButtonText = "Abbrechen",
        //        AnimateShow = true,
        //        AnimateHide = false
        //    };

        //    var result = await this.ShowMessageAsync(
        //        title,
        //        message,
        //        MessageDialogStyle.AffirmativeAndNegative, mySettings);
        //}
    }
}
