using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Confirmit.CATI.Installation.Common.Interfaces;
using Microsoft.Deployment.WindowsInstaller;
using SessionCustomAction.Properties;

namespace SessionCustomAction
{
    public enum DialogType
    {
        Open,
        Save,
        SelectFolder
    }

    public class FileBrowser
    {
        private readonly Session _session;
        private readonly ILogger _logger;
        private readonly DialogType _dialogType;
        private readonly string _sessionPropertyName;
        private readonly string _defaultFilePath;
        private readonly string _title;

        public FileBrowser(ILogger logger, Session session, DialogType dialogType, string sessionPropertyName, string defaultFilePath, string title)
        {
            _logger = logger;
            _session = session;
            _dialogType = dialogType;
            _sessionPropertyName = sessionPropertyName;
            _defaultFilePath = defaultFilePath;
            _title = title;
        }

        /// <summary>
        /// Select file from Safe or Open file dialog and put it to selected property
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="session">Session variable</param>
        /// <param name="dialogType">What kind of dialog should be appeared</param>
        /// <param name="sessionPropertyName">Session property name for selected file</param>
        /// <param name="defaultFilePath">Path to default file</param>
        /// <param name="title">Title of dialog</param>
        public static void SelectObject(ILogger logger, Session session, DialogType dialogType, string sessionPropertyName, string defaultFilePath, string title)
        {
            var fileBrowser = new FileBrowser(logger, session, dialogType, sessionPropertyName, defaultFilePath, title);

            var worker = new Thread(fileBrowser.SelectObject);
            worker.SetApartmentState(ApartmentState.STA);
            worker.Start();
            worker.Join();
        }

        private void SelectObject()
        {
            try
            {
                if (_dialogType == DialogType.SelectFolder)
                {
                    SelectFolder();
                }
                else
                {
                    SelectFile();
                }
            }
            catch (Exception ex)
            {
                _session[_sessionPropertyName] = Resources.AnUnexpectedErrorOccured + ex.Message;
                _logger.WriteLog(TraceEventType.Error, ex.ToString());
            }
        }

        /// <summary>
        /// Select folder by using FolderBrowserDialog class
        /// </summary>
        private void SelectFolder()
        {
            var folderDialog = new FolderBrowserDialog 
            { 
                RootFolder = Environment.SpecialFolder.MyComputer, 
                Description = _title 
            };

            if (!string.IsNullOrEmpty(_defaultFilePath))
            {
                folderDialog.SelectedPath = _defaultFilePath;
            }

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                _session[_sessionPropertyName] = folderDialog.SelectedPath;
            }
        }

        /// <summary>
        /// Select file by using OpenFileDialog or SaveFileDialog class
        /// </summary>
        private void SelectFile()
        {
            FileDialog fileDialog;
            if (_dialogType == DialogType.Open)
            {
                fileDialog = new OpenFileDialog();
            }
            else
            {
                fileDialog = new SaveFileDialog { AddExtension = true };
            }

            if (!string.IsNullOrEmpty(_defaultFilePath))
            {
                fileDialog.InitialDirectory = Path.GetDirectoryName(_defaultFilePath);
                fileDialog.FileName = Path.GetFileName(_defaultFilePath);
            }

            fileDialog.Title = _title;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                _session[_sessionPropertyName] = fileDialog.FileName;
            }
        }
    }
}
