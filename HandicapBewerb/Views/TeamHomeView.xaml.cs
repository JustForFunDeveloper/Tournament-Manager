using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace TournamentManager.Views
{
    /// <summary>
    /// Interaction logic for TeamHomeView.xaml
    /// </summary>
    public partial class TeamHomeView : UserControl
    {
        public TeamHomeView()
        {
            InitializeComponent();
        }

        private void UserDataControl_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                UserDataScroll.ScrollToVerticalOffset(UserDataScroll.ContentVerticalOffset - 80);
            else
                UserDataScroll.ScrollToVerticalOffset(UserDataScroll.ContentVerticalOffset + 80);
            e.Handled = true;
        }

        private void VScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            UserDataScroll.ScrollToVerticalOffset(e.NewValue);
        }
    }
}
