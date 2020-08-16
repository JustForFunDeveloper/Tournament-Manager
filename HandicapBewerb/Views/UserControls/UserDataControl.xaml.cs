using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TournamentManager.Views.UserControls
{
    public class CustomEventArgs : RoutedEventArgs
    {
        private readonly int _userId;

        public int UserId
        {
            get { return _userId; }
        }

        public CustomEventArgs(RoutedEvent routedEvent, int userId) : base(routedEvent)
        {
            this._userId = userId;
        }
    }

    /// <summary>
    /// Interaction logic for UserDataControl.xaml
    /// </summary>
    public partial class UserDataControl : UserControl
    {
        #region Public Properties

        public string Position
        {
            get => (string)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(string),
                typeof(UserDataControl), new PropertyMetadata(""));

        public string UserName
        {
            get => (string)GetValue(UserNameProperty);
            set => SetValue(UserNameProperty, value);
        }

        public static readonly DependencyProperty UserNameProperty =
            DependencyProperty.Register("UserName", typeof(string),
                typeof(UserDataControl), new PropertyMetadata(""));

        public string ThirdOldestDateTime
        {
            get => (string)GetValue(ThirdOldestDateTimeProperty);
            set => SetValue(ThirdOldestDateTimeProperty, value);
        }

        public static readonly DependencyProperty ThirdOldestDateTimeProperty =
            DependencyProperty.Register("ThirdOldestDateTime", typeof(string),
                typeof(UserDataControl), new PropertyMetadata(""));

        public string ThirdOldest
        {
            get => (string)GetValue(ThirdOldestProperty);
            set => SetValue(ThirdOldestProperty, value);
        }

        public static readonly DependencyProperty ThirdOldestProperty =
            DependencyProperty.Register("ThirdOldest", typeof(string),
                typeof(UserDataControl), new PropertyMetadata(""));

        public string SecondOldestDateTime
        {
            get => (string)GetValue(SecondOldestDateTimeProperty);
            set => SetValue(SecondOldestDateTimeProperty, value);
        }

        public static readonly DependencyProperty SecondOldestDateTimeProperty =
            DependencyProperty.Register("SecondOldestDateTime", typeof(string),
                typeof(UserDataControl), new PropertyMetadata(""));

        public string SecondOldest
        {
            get => (string)GetValue(SecondOldestProperty);
            set => SetValue(SecondOldestProperty, value);
        }

        public static readonly DependencyProperty SecondOldestProperty =
            DependencyProperty.Register("SecondOldest", typeof(string),
                typeof(UserDataControl), new PropertyMetadata(""));

        public string FirstOldestDateTime
        {
            get => (string)GetValue(FirstOldestDateTimeProperty);
            set => SetValue(FirstOldestDateTimeProperty, value);
        }

        public static readonly DependencyProperty FirstOldestDateTimeProperty =
            DependencyProperty.Register("FirstOldestDateTime", typeof(string),
                typeof(UserDataControl), new PropertyMetadata(""));

        public string FirstOldest
        {
            get => (string)GetValue(FirstOldestProperty);
            set => SetValue(FirstOldestProperty, value);
        }

        public static readonly DependencyProperty FirstOldestProperty =
            DependencyProperty.Register("FirstOldest", typeof(string),
                typeof(UserDataControl), new PropertyMetadata(""));

        public string NullValue
        {
            get => (string)GetValue(NullValueProperty);
            set { SetValue(NullValueProperty, value); }
        }

        public static readonly DependencyProperty NullValueProperty =
            DependencyProperty.Register("NullValue", typeof(string),
                typeof(UserDataControl), new PropertyMetadata(""));

        public double CurrentRound
        {
            get => (double)GetValue(CurrentRoundProperty);
            set => SetValue(CurrentRoundProperty, value);
        }

        public static readonly DependencyProperty CurrentRoundProperty =
            DependencyProperty.Register("CurrentRound", typeof(double),
                typeof(UserDataControl), new PropertyMetadata());

        public string Result
        {
            get => (string)GetValue(ResultProperty);
            set => SetValue(ResultProperty, value);
        }

        public static readonly DependencyProperty ResultProperty =
            DependencyProperty.Register("Result", typeof(string),
                typeof(UserDataControl), new PropertyMetadata(""));

        public bool IsInputEnabled
        {
            get => (bool)GetValue(IsInputEnabledProperty);
            set => SetValue(IsInputEnabledProperty, value);
        }

        public static readonly DependencyProperty IsInputEnabledProperty =
            DependencyProperty.Register("IsInputEnabled", typeof(bool),
                typeof(UserDataControl), new PropertyMetadata(true));

        public double SortResult { get; set; }

        #endregion

        #region Events

        public static readonly RoutedEvent OnResultChangedEvent = EventManager.RegisterRoutedEvent(
            "OnResultChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UserDataControl));

        public event RoutedEventHandler OnResultChanged
        {
            add { AddHandler(OnResultChangedEvent, value); }
            remove { RemoveHandler(OnResultChangedEvent, value); }
        }

        #endregion

        #region Private Properties

        public int UserId { get; }

        #endregion

        public UserDataControl()
        {
            InitializeComponent();
        }

        public UserDataControl(int userId)
        {
            InitializeComponent();
            this.UserId = userId;
            CalculateResults(Double.NaN);
        }

        private void NumericUpDown_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (e.NewValue != null && NullValue.Length > 0)
            {
                CalculateResults((double)e.NewValue);

                CustomEventArgs newCustomEventArgs = new CustomEventArgs(UserDataControl.OnResultChangedEvent, UserId);
                RaiseEvent(newCustomEventArgs);
            }
        }

        private void CalculateResults(double value)
        {
            double result = -9999;
            if (!Double.IsNaN(value))
            {
                result = value - Double.Parse(NullValue);
            }

            if (result.Equals(0))
            {
                Result = "0.00";
                SortResult = 0.0;
            }
            else
            {
                if (Double.IsNaN(result))
                {
                    result= -9999;
                }

                Result = result.ToString("##.#0");
                SortResult = result;
            }

            if (result < 0)
            {
                TxtResult.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                TxtResult.Foreground = new SolidColorBrush(Colors.Green);
            }
        }
    }
}
