using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TournamentManager.Views.UserControls
{
    /// <summary>
    /// Interaction logic for TeamUserDataControl.xaml
    /// </summary>
    public partial class TeamUserDataControl : UserControl
    {
        private readonly string DOUBLE_FORMAT = "##.#0";

        #region Public Properties

        public string TeamName
        {
            get => (string)GetValue(TeamNameProperty);
            set => SetValue(TeamNameProperty, value);
        }

        public static readonly DependencyProperty TeamNameProperty =
            DependencyProperty.Register("TeamName", typeof(string),
                typeof(TeamUserDataControl), new PropertyMetadata(""));


        public string TeamPosition
        {
            get => (string)GetValue(TeamPositionProperty);
            set => SetValue(TeamPositionProperty, value);
        }

        public static readonly DependencyProperty TeamPositionProperty =
            DependencyProperty.Register("TeamPosition", typeof(string),
                typeof(TeamUserDataControl), new PropertyMetadata(""));

        public string TeamResult
        {
            get => (string)GetValue(TeamResultProperty);
            set => SetValue(TeamResultProperty, value);
        }

        public static readonly DependencyProperty TeamResultProperty =
            DependencyProperty.Register("TeamResult", typeof(string),
                typeof(TeamUserDataControl), new PropertyMetadata(""));

        public List<UserDataControl> UserDataControls
        {
            get => (List<UserDataControl>)GetValue(UserDataControlsProperty);
            set
            {
                foreach (var dataControl in value)
                {
                    dataControl.OnResultChanged += DataControlOnOnResultChanged;
                }
                SetValue(UserDataControlsProperty, value);
            }
        }

        public double TeamSortResult { get; set; }

        private void DataControlOnOnResultChanged(object sender, RoutedEventArgs e)
        {
            var sum = UserDataControls.Select(x =>
            {
                if (Double.TryParse(x.Result, out double result))
                {
                    if (result == -9999d)
                    {
                        return 0.0d;
                    }
                    return result;
                }
                return 0.0d;
            }).Sum();

            TeamSortResult = sum;

            if (sum.Equals(0d))
            {
                TeamResult = "0,00";
            }
            else
            {
                TeamResult = sum.ToString(DOUBLE_FORMAT);
            }
        }

        public static readonly DependencyProperty UserDataControlsProperty =
            DependencyProperty.Register("UserDataControls", typeof(List<UserDataControl>),
                typeof(TeamUserDataControl), new PropertyMetadata(null));

        #endregion

        public TeamUserDataControl()
        {
            InitializeComponent();
        }

        public void RefreshResults()
        {
            DataControlOnOnResultChanged(null, null);
        }
    }
}
