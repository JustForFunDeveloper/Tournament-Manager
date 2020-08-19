using System;
using System.Collections.Generic;
using System.IO;
using TournamentManager.ViewModels;
using TournamentManager.Views.UserControls;

namespace TournamentManager.Core.Data
{
    public static class ApplicationData
    {
        public static readonly string UpdateUrl =
            "https://raw.githubusercontent.com/JustForFunDeveloper/Tournament-Manager/master/AutoUpdate/AutoUpdate.xml";
        public static readonly string Password = "admin01!";
        public static readonly string DownloadPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static readonly string UpdateFilePathFull = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "TournamentManager.zip";

        public static bool UserDataChanged { get; set; }
        public static bool IsAdminLoggedIn { get; set; }
        public static List<int> CurrentSelectedUser { get; set; }
        public static List<UserDataControl> UserDataControls { get; set; }

        public static List<Team> TeamControls { get; set; }
    }
}
