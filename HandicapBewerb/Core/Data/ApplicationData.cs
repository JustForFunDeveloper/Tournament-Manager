using System.Collections.Generic;
using System.Windows.Media.Converters;
using HandicapBewerb.Views.UserControls;

namespace HandicapBewerb.Core.Data
{
    public static class ApplicationData
    {
        public static bool UserDataChanged { get; set; } 
        public static readonly string Password = "admin01!";
        public static bool IsAdminLoggedIn { get; set; }
        public static List<int> CurrentSelectedUser { get; set; }
        public static List<UserDataControl> UserDataControls { get; set; }
    }
}
