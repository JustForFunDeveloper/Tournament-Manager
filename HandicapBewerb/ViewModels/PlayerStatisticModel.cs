using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32.SafeHandles;
using TournamentManager.Core.Handler;
using TournamentManager.DataModels.DbModels;
using TournamentManager.ViewModels.Handler;

namespace TournamentManager.ViewModels
{
    public class PlayerStatisticModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly string DATE_FORMAT = "dd.MM.yy HH:mm";

        private ICommand _onShiftLeft;
        private ICommand _onShiftRight;
        private ICommand _onZoomIn;
        private ICommand _onZoomOut;

        private bool _canOnShiftLeft = true;
        private bool _canOnShiftRight = true;
        private bool _canOnZoomIn = true;
        private bool _canOnZoomOut = true;

        private int _currentZoomLevel = 100;
        private readonly int ZOOM_STEPS = 10;
        private readonly int MAX_ZOOM = 200;
        private int _currentSkipValue = 0;
        private int _lastDataSkipLevel = 0;

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

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
            RefreshChart();
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
            RefreshChart();
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

            _currentZoomLevel -= ZOOM_STEPS;

            if (_currentZoomLevel <= 0)
            {
                _currentZoomLevel = ZOOM_STEPS;
                _canOnZoomIn = false;
            }

            RefreshChart();
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

            _currentZoomLevel += ZOOM_STEPS;

            if (_currentZoomLevel > MAX_ZOOM)
            {
                _currentZoomLevel -= ZOOM_STEPS;
                _canOnZoomOut = false;
            }

            RefreshChart();
        }

        public PlayerStatisticModel()
        {
            DisplayGraphForUser(null);
        }

        public void DisplayGraphForUser(User user)
        {
            //var rounds = DbHandler.GetUserRounds(DbHandler.GetUsers()[0]);
            //var doubleValues = rounds
            //    .OrderByDescending(r => r.Date)
            //    .Take(INITIAL_ZOOM)
            //    .Select(r => r.Points)
            //    .Reverse();
            //var dates = rounds.Select(r => r.Date)
            //    .OrderByDescending(r => r)
            //    .Take(INITIAL_ZOOM)
            //    .Select(d => d.ToString(DATE_FORMAT))
            //    .Reverse()
            //    .ToArray();

            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Runden",
                    Values = new ChartValues<double> (),
                    LineSmoothness = 0,
                    PointGeometry = DefaultGeometries.Square,
                    PointGeometrySize = 8
                }
            };

            //Labels = dates;
            YFormatter = value => value.ToString("F1");
            RefreshChart();
        }

        private void RefreshChart()
        {
            var rounds = DbHandler.GetUserRounds(DbHandler.GetUsers()[0]);
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
                _canOnZoomOut = false;
                _currentZoomLevel = doubleValues.Count();
                _canOnZoomIn = true;
            }

            Console.WriteLine($"Skip: {_currentSkipValue} Zoom: {_currentZoomLevel} ");
        }
    }
}
