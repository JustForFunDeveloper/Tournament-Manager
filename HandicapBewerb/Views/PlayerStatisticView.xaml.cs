using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Configurations;
using SimpleWPFReporting;
using TournamentManager.DataModels.DbModels;

namespace TournamentManager.Views
{
    /// <summary>
    /// Interaction logic for PlayerStatistic.xaml
    /// </summary>
    public partial class PlayerStatisticView : UserControl
    {
        public PlayerStatisticView()
        {
            InitializeComponent();
            //CartesianChart.DataTooltip = new CustomMatchTooltip();

            //let create a mapper so LiveCharts know how to plot our CustomerViewModel class
            var matchMapper = Mappers.Xy<MatchResult>()
                .X((value, index) => index) // lets use the position of the item as X
                .Y(value => value.Result); //and PurchasedItems property as Y

            //lets save the mapper globally
            Charting.For<MatchResult>(matchMapper);

            var teamMatchMapper = Mappers.Xy<StatisticTeamMatchResult>()
                .X((value, index) => index) // lets use the position of the item as X
                .Y(value => value.SoloTeamMatchResult.Result); //and PurchasedItems property as Y

            //lets save the mapper globally
            Charting.For<StatisticTeamMatchResult>(teamMatchMapper);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Report.ExportVisualAsPdf(Print);
        }
    }
}
