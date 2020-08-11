using System;

namespace HandicapBewerb.ViewModels.Interfaces
{
    interface ISettingsView
    {
        event EventHandler SaveConfig;
        event EventHandler RestoreLocalPathSettings;
        event EventHandler RestoreFeedLinkSettings;
        event EventHandler LogRefresh;

        void RefreshSettingsView();
    }
}
