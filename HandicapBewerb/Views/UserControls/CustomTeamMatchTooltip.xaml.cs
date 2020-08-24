using System.ComponentModel;
using LiveCharts;
using LiveCharts.Wpf;

namespace TournamentManager.Views.UserControls
{
    /// <summary>
    /// Interaction logic for CustomTeamMatchTooltip.xaml
    /// </summary>
    public partial class CustomTeamMatchTooltip : IChartTooltip
    {
        private TooltipData _data;

        public CustomTeamMatchTooltip()
        {
            InitializeComponent();

            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public TooltipData Data
        {
            get { return _data; }
            set
            {
                _data = value;
                OnPropertyChanged("Data");
            }
        }

        public TooltipSelectionMode? SelectionMode { get; set; }


        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
