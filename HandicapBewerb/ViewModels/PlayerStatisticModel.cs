using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using TournamentManager.Core.Handler;
using TournamentManager.ViewModels.Handler;

namespace TournamentManager.ViewModels
{
    public class PlayerStatisticModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly string DATE_FORMAT = "dd.MM.yy HH:mm";
        private int? _currentUserId = null;
        private bool _isRoundMode;

        private ICommand _onClose;
        private ICommand _onShiftLeft;
        private ICommand _onShiftRight;
        private ICommand _onZoomIn;
        private ICommand _onZoomOut;

        private bool _canOnShiftLeft = true;
        private bool _canOnShiftRight = true;
        private bool _canOnZoomIn = true;
        private bool _canOnZoomOut = true;

        private int _currentZoomLevel = 100;
        private int _zoomSteps = 10;
        private readonly int MAX_ZOOM = 200;
        private int _currentSkipValue = 0;
        private int _lastDataSkipLevel = 0;

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }
        public string UserText { get; set; }

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
            Mediator.NotifyColleagues(MediatorGlobal.OnBackToUserDataView, null);
        }

        public ICommand OnShiftLeft
        {
            get
            {
                if (_onShiftLeft == null)
                    _onShiftLeft = new RelayCommand(
                        param => OnShiftLeftCommand(),
                        param => CanOnShiftLeftCommand()
                    );
                return _onShiftLeft;
            }
        }

        private bool CanOnShiftLeftCommand()
        {
            return _canOnShiftLeft;
        }

        private void OnShiftLeftCommand()
        {
            _canOnShiftRight = true;
            _currentSkipValue += _currentZoomLevel;
            if (_isRoundMode)
                RefreshRoundChart();
            else
                RefreshMatchChart();
        }

        public ICommand OnShiftRight
        {
            get
            {
                if (_onShiftRight == null)
                    _onShiftRight = new RelayCommand(
                        param => OnShiftRightCommand(),
                        param => CanOnShiftRightCommand()
                    );
                return _onShiftRight;
            }
        }

        private bool CanOnShiftRightCommand()
        {
            return _canOnShiftRight;
        }

        private void OnShiftRightCommand()
        {
            _canOnShiftLeft = true;

            _currentSkipValue -= _currentZoomLevel;
            if (_currentSkipValue < 0)
            {
                _currentSkipValue = 0;
                _canOnShiftRight = false;
            }

            if (_isRoundMode)
                RefreshRoundChart();
            else
                RefreshMatchChart();
        }

        public ICommand OnZoomIn
        {
            get
            {
                if (_onZoomIn == null)
                    _onZoomIn = new RelayCommand(
                        param => OnZoomInCommand(),
                        param => CanOnZoomInCommand()
                    );
                return _onZoomIn;
            }
        }

        private bool CanOnZoomInCommand()
        {
            return _canOnZoomIn;
        }

        private void OnZoomInCommand()
        {
            _canOnShiftLeft = true;
            _canOnShiftRight = true;
            _canOnZoomOut = true;

            _currentZoomLevel -= _zoomSteps;

            if (_currentZoomLevel <= 0)
            {
                _currentZoomLevel = _zoomSteps;
                _canOnZoomIn = false;
            }

            if (_isRoundMode)
                RefreshRoundChart();
            else
                RefreshMatchChart();
        }

        public ICommand OnZoomOut
        {
            get
            {
                if (_onZoomOut == null)
                    _onZoomOut = new RelayCommand(
                        param => OnZoomOutCommand(),
                        param => CanOnZoomOutCommand()
                    );
                return _onZoomOut;
            }
        }

        private bool CanOnZoomOutCommand()
        {
            return _canOnZoomOut;
        }

        private void OnZoomOutCommand()
        {
            _canOnShiftLeft = true;
            _canOnShiftRight = true;
            _canOnZoomIn = true;

            _currentSkipValue -= _currentZoomLevel;
            if (_currentSkipValue < 0)
            {
                _currentSkipValue = 0;
                _canOnShiftRight = false;
            }

            _currentZoomLevel += _zoomSteps;

            if (_currentZoomLevel > MAX_ZOOM)
            {
                _currentZoomLevel -= _zoomSteps;
                _canOnZoomOut = false;
            }

            if (_isRoundMode)
                RefreshRoundChart();
            else
                RefreshMatchChart();
        }

        public PlayerStatisticModel()
        {
            Mediator.Register(MediatorGlobal.OnRoundStatistics, OnRoundStatistics);
            Mediator.Register(MediatorGlobal.OnMatchStatistics, OnMatchStatistics);
        }

        private void OnMatchStatistics(object obj)
        {
            _isRoundMode = false;
            _currentZoomLevel = 100;
            DisplayMatchGraphForUser((int?)obj);
        }

        private void OnRoundStatistics(object obj)
        {
            _isRoundMode = true;
            _currentZoomLevel = 100;
            DisplayRoundGraphForUser((int?)obj);
        }

        private void DisplayRoundGraphForUser(int? userId)
        {
            _currentUserId = userId;
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Runden",
                    Values = new ChartValues<double> (),
                    LineSmoothness = 0,
                    PointGeometry = DefaultGeometries.Square,
                    PointGeometrySize = 10
                }
            };
            YFormatter = value => value.ToString("F1");
            RefreshRoundChart();
        }

        private void DisplayMatchGraphForUser(int? userId)
        {
            _currentUserId = userId;
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Matches",
                    Values = new ChartValues<double> (),
                    LineSmoothness = 0,
                    PointGeometry = DefaultGeometries.Square,
                    PointGeometrySize = 10,
                    Stroke = Brushes.Green
                }
            };
            YFormatter = value => value.ToString("F1");
            RefreshMatchChart();
        }

        private void RefreshRoundChart()
        {
            if (_currentUserId == null)
                return;

            var user = DbHandler.GetUsers().Single(u => u.UserId.Equals((int) _currentUserId));
            UserText = $"Runden Statistik von {user.Name}";

            var rounds = DbHandler.GetUserRounds(user);
            var doubleValues = rounds
                .OrderByDescending(r => r.Date)
                .Skip(_currentSkipValue).Take(_currentZoomLevel)
                .Select(r => r.Points)
                .Reverse();

            if (doubleValues.Count() <= 2)
            {
                _canOnShiftLeft = false;
                if (doubleValues.Count() <= 0)
                {
                    _canOnShiftLeft = false;
                    _currentSkipValue = _lastDataSkipLevel;

                    doubleValues = rounds
                        .OrderByDescending(r => r.Date)
                        .Skip(_currentSkipValue).Take(_currentZoomLevel)
                        .Select(r => r.Points)
                        .Reverse();
                }
            }
            else
            {
                _lastDataSkipLevel = _currentSkipValue;
            }

            var dates = rounds
                .Select(r => r.Date)
                .OrderByDescending(r => r)
                .Skip(_currentSkipValue)
                .Take(_currentZoomLevel)
                .Select(d => d.ToString(DATE_FORMAT))
                .Reverse()
                .ToArray();

            SeriesCollection[0].Values = new ChartValues<double>(doubleValues);
            Labels = dates;


            if (doubleValues.Count() < _currentZoomLevel)
            {
                //_canOnZoomOut = false;
                _currentZoomLevel = doubleValues.Count();
                _canOnZoomIn = true;
            }

            _zoomSteps = doubleValues.Count() / 2;
        }

        private void RefreshMatchChart()
        {
            if (_currentUserId == null)
                return;

            var user = DbHandler.GetUsers().Single(u => u.UserId.Equals((int)_currentUserId));
            UserText = $"Match Statistik von {user.Name}";

            var matches = DbHandler.GetAllMatchesFromUser(user);
            var doubleValues = matches
                .OrderByDescending(r => r.Date)
                .Skip(_currentSkipValue).Take(_currentZoomLevel)
                .SelectMany(r => r.MatchResults.Where(mr => mr.UserName == user.Name).Select(mr => mr.Result))
                .Reverse();

            if (doubleValues.Count() <= 2)
            {
                _canOnShiftLeft = false;
                if (doubleValues.Count() <= 0)
                {
                    _canOnShiftLeft = false;
                    _currentSkipValue = _lastDataSkipLevel;

                    doubleValues = matches
                        .OrderByDescending(r => r.Date)
                        .Skip(_currentSkipValue).Take(_currentZoomLevel)
                        .SelectMany(r => r.MatchResults.Where(mr => mr.UserName == user.Name).Select(mr => mr.Result))
                        .Reverse();
                }
            }
            else
            {
                _lastDataSkipLevel = _currentSkipValue;
            }

            var dates = matches
                .Select(r => r.Date)
                .OrderByDescending(r => r)
                .Skip(_currentSkipValue)
                .Take(_currentZoomLevel)
                .Select(d => d.ToString(DATE_FORMAT))
                .Reverse()
                .ToArray();

            SeriesCollection[0].Values = new ChartValues<double>(doubleValues);
            Labels = dates;


            if (doubleValues.Count() < _currentZoomLevel)
            {
                //_canOnZoomOut = false;
                _currentZoomLevel = doubleValues.Count();
                _canOnZoomIn = true;
            }

            _zoomSteps = doubleValues.Count() / 2;
        }
    }
}
