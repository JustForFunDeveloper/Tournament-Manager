using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HandicapBewerb.Core.Data;
using HandicapBewerb.Core.GlobalValues;
using HandicapBewerb.Core.Handler;
using HandicapBewerb.DataModels.DbModels;
using HandicapBewerb.DataModels.XmlModels;
using HS_Feed_Manager.Control;
using Microsoft.EntityFrameworkCore;

namespace HandicapBewerb.Core
{
    public class Logic
    {
        #region Public Properties

        public static Config LocalConfig => _config;

        public static string Log => _log;
        #endregion

        #region Private Properties

        private Controller _controller;
        private FileHandler _fileHandler;
        private static Config _config;
        private static string _log;

        #endregion

        public Logic()
        {
            _controller = new Controller();
            _fileHandler = new FileHandler();
            InitDataBase();
            var unused = new LogHandler(LogLevel.Debug, _fileHandler);

            _fileHandler.ExceptionEvent += OnExceptionEvent;

            _controller.SaveConfig += OnSaveConfig;
            _controller.RestoreLocalPathSettings += OnRestoreLocalPathSettings;
            _controller.RestoreFeedLinkSettings += OnRestoreFeedLinkSettings;
            _controller.LogRefresh += OnLogRefresh;

            try
            {
                LoadOrCreateConfig();
                OnDownloadFeed(null, null);
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("Logic: " + ex, LogLevel.Error);
            }
        }

        private void InitDataBase()
        {
            try
            {
                DBHandler.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void OnDownloadFeed(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("OnDownloadFeed: " + ex, LogLevel.Error);
            }
        }

        private async void OnSearchLocalFolder(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(() => ScanFolder());
                _controller.FinishedSearchLocalFolder();
                _controller.RefreshData();
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("OnSearchLocalFolder: " + ex, LogLevel.Error);
            }
        }

        private void ScanFolder()
        {
            try
            {

            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("OnDeleteEpisode: " + ex, LogLevel.Error);
            }
        }

        private void OnDeleteEpisode(object sender, object e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("OnDeleteEpisode: " + ex, LogLevel.Error);
            }
        }

        private void OnDeleteTvShow(object sender, object e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("OnDeleteTvShow: " + ex, LogLevel.Error);
            }
        }

        private void OpenFolder(object sender, object e)
        {

        }

        private void LoadOrCreateConfig()
        {
            try
            {
                FileInfo fileInfo = new FileInfo(LogicConstants.StandardXmlPath + LogicConstants.StandardXmlName);

                if (!fileInfo.Exists)
                {
                    // Create new xml file since its missing and set it to standard values
                    string standardXml = XmlHandler.GetSerializedConfigXml(typeof(Config), new Config());
                    _fileHandler.CreateFileIfNotExist(LogicConstants.StandardXmlName, LogicConstants.StandardXmlPath, false);
                    _fileHandler.AppendText(LogicConstants.StandardXmlName, standardXml, LogicConstants.StandardXmlPath);
                }
                var configAsString = _fileHandler.ReadAllText(LogicConstants.StandardXmlName, LogicConstants.StandardXmlPath);
                _config = (Config)XmlHandler.GetDeserializedConfigObject(typeof(Config), configAsString);
                RefreshLocalConfig();
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("LoadOrCreateConfig: " + ex, LogLevel.Error);
            }
        }

        private void RefreshLocalConfig()
        {
            try
            {
                _fileHandler.FileEndings = _config.FileEndings?.Split(';').ToList();
                _fileHandler.LocalPath1 = _config.LocalPath1;
                _fileHandler.LocalPath2 = _config.LocalPath2;
                _fileHandler.LocalPath3 = _config.LocalPath3;

                _controller.RefreshSettingsView();
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("RefreshLocalConfig: " + ex, LogLevel.Error);
            }
        }

        private void OnSaveConfig(object sender, object e)
        {
            try
            {
                string standardXml = XmlHandler.GetSerializedConfigXml(typeof(Config), _config);
                _fileHandler.OverwriteFile(LogicConstants.StandardXmlName, standardXml, LogicConstants.StandardXmlPath);
                RefreshLocalConfig();
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("RefreshLocalConfig: " + ex, LogLevel.Error);
            }
        }

        private void OnRestoreLocalPathSettings(object sender, object e)
        {
            try
            {
                _config.FileEndings = LogicConstants.FileEndings;
                _config.LocalPath1 = LogicConstants.LocalPath1;
                _config.LocalPath2 = LogicConstants.LocalPath2;
                _config.LocalPath3 = LogicConstants.LocalPath3;
                OnSaveConfig(null, null);
                RefreshLocalConfig();
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("OnRestoreLocalPathSettings: " + ex, LogLevel.Error);
            }
        }

        private void OnRestoreFeedLinkSettings(object sender, object e)
        {
            try
            {
                _config.FeedUrl = LogicConstants.FeedUrl;
                _config.NameFrontRegex = LogicConstants.NameFrontRegex;
                _config.NameBackRegex = LogicConstants.NameBackRegex;
                _config.NumberFrontRegex = LogicConstants.NumberFrontRegex;
                _config.NumberBackRegex = LogicConstants.NumberBackRegex;
                OnSaveConfig(null, null);
                RefreshLocalConfig();
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("OnRestoreFeedLinkSettings: " + ex, LogLevel.Error);
            }
        }

        private void OnLogRefresh(object sender, object e)
        {
            try
            {
                _log = _fileHandler?.ReadAllText(LogHandler.CurrentLogName, LogicConstants.LogFilePath);
                _controller.RefreshSettingsView();
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("OnLogRefresh: " + ex, LogLevel.Error);
            }
        }

        private void OnExceptionEvent(object sender, Exception e)
        {
            LogHandler.WriteSystemLog("OnExceptionEvent:" + e, LogLevel.Debug);
        }
    }
}
