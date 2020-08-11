using System.Windows;
using HandicapBewerb.Core;

namespace HandicapBewerb
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
