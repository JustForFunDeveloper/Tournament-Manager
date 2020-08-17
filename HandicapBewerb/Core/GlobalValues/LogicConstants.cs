using System.IO;

namespace TournamentManager.Core.GlobalValues
{
    public static class LogicConstants
    {
        public static readonly string LogFilePath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Logs" + Path.DirectorySeparatorChar;
        public static readonly string LogFileName = "log.txt";
    }
}
