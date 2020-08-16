using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using TournamentManager.Views.UserControls;

namespace TournamentManager.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class HomeView : UserControl
    {
        public HomeView()
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
