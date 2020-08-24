using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using TournamentManager.Core.Data;
using TournamentManager.Core.Handler;
using TournamentManager.DataModels.DbModels;
using TournamentManager.ViewModels.Handler;

namespace TournamentManager.ViewModels
{
    public class TeamMatchDataModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<SoloTeamMatchResult> _soloTeamMatchResults;
        private ObservableCollection<TeamMatchResult> _teamMatchResults;
        private TeamMatch _selection;

        private ICommand _onDeleteMatch;
        private ICommand _onRefresh;

        public ObservableCollection<TeamMatch> TeamMatches { get; set; }

        public ObservableCollection<SoloTeamMatchResult> SoloTeamMatchResults
        {
            get
            {
                return _soloTeamMatchResults;
            }
            set
            {
                _soloTeamMatchResults = value;
            }
        }

        public ObservableCollection<TeamMatchResult> TeamMatchResults
        {
            get
            {
                return _teamMatchResults;
            }
            set
            {
                _teamMatchResults = value;
            }
        }

        public TeamMatch Selection
        {
            get
            {
                return _selection;
            }
            set
            {
                if (value != null)
                {
                    SoloTeamMatchResults = new ObservableCollection<SoloTeamMatchResult>(DbHandler.GetSoloTeamMatchResults(value));
                    TeamMatchResults = new ObservableCollection<TeamMatchResult>(DbHandler.GetTeamMatchResults(value));
                }
                else
                {
                    SoloTeamMatchResults = new ObservableCollection<SoloTeamMatchResult>();
                    TeamMatchResults = new ObservableCollection<TeamMatchResult>();
                }
                _selection = value;
            }
        }

        public TeamMatchDataModel()
        {
            Mediator.Register(MediatorGlobal.RefreshView, OnRefreshView);
            RefreshData();
        }

        private void OnRefreshView(object obj)
        {
            var view = (MainViewModel.ApplicationViewEnum)obj;
            if (view.Equals(MainViewModel.ApplicationViewEnum.TeamMatchDataView))
            {
                RefreshData();
            }
        }

        private void RefreshData()
        {
            TeamMatches = new ObservableCollection<TeamMatch>(DbHandler.GetTeamMatchesIncludingResults());
            SoloTeamMatchResults = new ObservableCollection<SoloTeamMatchResult>();
            TeamMatchResults = new ObservableCollection<TeamMatchResult>();
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
                    "Um diesen Vorgang durchzuführen muss man angemeldet sein!"
                });
                return;
            }

            try
            {
                DbHandler.DeleteTeamMatches(TeamMatches.ToList());
                RefreshData();
            }
            catch (Exception e)
            {
                LogHandler.WriteSystemLog("OnDeleteMatchCommand:" + e, LogLevel.Error);
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
