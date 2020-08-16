using System;
using System.Windows;

namespace TournamentManager.ViewModels.Handler
{
    // Not used at the moment, just for testing purposes.
    public static class PropertyChangedNotificationInterceptor
    {
        public static void Intercept(object target, Action onPropertyChangedAction, string propertyName)
        {
            Application.Current.Dispatcher.Invoke(onPropertyChangedAction);
        }
    }
}
