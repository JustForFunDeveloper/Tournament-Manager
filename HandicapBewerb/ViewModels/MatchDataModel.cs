﻿using System;
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
    public class MatchDataModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<MatchResult> _matchResults;
        private Match _selection;

        private ICommand _onDeleteMatch;
        private ICommand _onRefresh;

        public ObservableCollection<Match> Matches { get; set; }

        public ObservableCollection<MatchResult> MatchResults
        {
            get
            {
                return _matchResults;
            }
            set
            {
                _matchResults = value;
            }
        }

        public Match Selection
        {
            get
            {
                return _selection;
            }
            set
            {
                if (value != null)
                {
                    MatchResults = new ObservableCollection<MatchResult>(DBHandler.GetMatchResults(value));
                }
                else
                {
                    MatchResults = new ObservableCollection<MatchResult>();
                }
                _selection = value;
            }
        }

        public MatchDataModel()
        {
            Mediator.Register(MediatorGlobal.RefreshView, OnRefreshView);
            RefreshData();
        }

        private void OnRefreshView(object obj)
        {
            var view = (MainViewModel.ApplicationViewEnum)obj;
            if (view.Equals(MainViewModel.ApplicationViewEnum.MatchDataView))
            {
                RefreshData();
            }
        }

        private void RefreshData()
        {
            Matches = new ObservableCollection<Match>(DBHandler.GetMatchesIncludingResults());
            MatchResults = new ObservableCollection<MatchResult>();
        }

        public ICommand OnDeleteMatch
        {
            get
            {
                if (_onDeleteMatch == null)
                    _onDeleteMatch = new RelayCommand(
                        param => OnDeleteMatchCommand(),
                        param => CanOnDeleteMatchCommand()
                    );
                return _onDeleteMatch;
            }
        }

        private bool CanOnDeleteMatchCommand()
        {
            return true;
        }

        private void OnDeleteMatchCommand()
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
                DBHandler.deleteMatches(Matches.ToList());
                RefreshData();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public ICommand OnRefresh
        {
            get
            {
                if (_onRefresh == null)
                    _onRefresh = new RelayCommand(
                        param => OnRefreshCommand(),
                        param => CanOnRefreshCommand()
                    );
                return _onRefresh;
            }
        }

        private bool CanOnRefreshCommand()
        {
            return true;
        }

        private void OnRefreshCommand()
        {
           RefreshData();
        }
    }
}