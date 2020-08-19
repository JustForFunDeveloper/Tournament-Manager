using System.Windows.Controls;
using TournamentManager.ViewModels.Handler;

namespace TournamentManager.Views
{
    /// <summary>
    /// Interaction logic for TeamAddPlayerView.xaml
    /// </summary>
    public partial class TeamAddPlayerView : UserControl
    {
        public TeamAddPlayerView()
        {
            InitializeComponent();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Mediator.NotifyColleagues(MediatorGlobal.SelectedTeamUsers, UserListBox.SelectedItems);
        }
    }
}
