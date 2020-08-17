using System;
using System.Threading.Tasks;
using System.Windows;
using TournamentManager.Core;
using TournamentManager.Core.Handler;

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
            LogHandler.WriteSystemLog("Application started!", LogLevel.Debug);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SetupExceptionHandling();
        }

        private void SetupExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

            DispatcherUnhandledException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
                e.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
                e.SetObserved();
            };
        }

        private void LogUnhandledException(Exception exception, string source)
        {
            string message = $"Unhandled exception ({source})";
            try
            {
                System.Reflection.AssemblyName assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
                message = string.Format("Unhandled exception in {0} v{1}", assemblyName.Name, assemblyName.Version);
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("Exception in LogUnhandledException:\n" + ex, LogLevel.Error);
                //_logger.Error(ex, "Exception in LogUnhandledException");
            }
            finally
            {
                LogHandler.WriteSystemLog(message + ":\n" + exception, LogLevel.Error);
                Current.Shutdown();
            }
        }
    }
}
