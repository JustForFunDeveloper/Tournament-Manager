using System.Windows;
using TournamentManager.Core;

namespace TournamentManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Logic();
        }
    }
}
