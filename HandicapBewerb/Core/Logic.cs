using System;
using TournamentManager.Core.Handler;

namespace TournamentManager.Core
{
    public class Logic
    {
        #region Private Properties

        private readonly FileHandler _fileHandler;
        private readonly LogHandler _logHandler;

        #endregion

        public Logic()
        {
            _fileHandler = new FileHandler();
            InitDataBase();
            _logHandler = new LogHandler(LogLevel.Debug, _fileHandler);

            _fileHandler.ExceptionEvent += OnExceptionEvent;
        }

        private void InitDataBase()
        {
            try
            {
                DbHandler.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void OnExceptionEvent(object sender, Exception e)
        {
            LogHandler.WriteSystemLog("OnExceptionEvent:" + e, LogLevel.Debug);
        }
    }
}
