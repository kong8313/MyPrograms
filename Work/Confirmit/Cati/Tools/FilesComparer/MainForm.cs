using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Confirmit.CATI.Installation.Common.Interfaces;
using System;
using Confirmit.CATI.Installation.Common;

namespace FilesComparer
{
    public partial class MainForm : Form
    {
        private string _folderPath1;
        private string _folderPath2;
        private bool _compareFilesFromSubfolders;
        private string _includeFileMask;

        private readonly List<string> _folder1UniqueFiles;
        private readonly List<string> _folder2UniqueFiles;
        private readonly Dictionary<string, string> _filesToCompare;
        private readonly Dictionary<KeyValuePair<string, string>, CompareState> _compareResult;
        private readonly string _idlasmPath;
        private readonly string _tempComparingPath;

        private ILogger _logger;
        private FileComparer _fileComparer;

        private bool _stopExecution;

        private delegate void VoidFunctionWithOneIntParameterDelegate(int stringParameter);
        private delegate void VoidFunctionWithoutParameters();

        public MainForm()
        {
            InitializeComponent();

            _folder1UniqueFiles = new List<string>();
            _folder2UniqueFiles = new List<string>();
            _filesToCompare = new Dictionary<string, string>();
            _compareResult = new Dictionary<KeyValuePair<string, string>, CompareState>();
            _idlasmPath = Path.Combine(Application.StartupPath, "ildasm.exe");
            _tempComparingPath = Path.GetTempPath();
        }

        private void ButtonSelectFolder1Click(object sender, EventArgs e)
        {
            folderBrowserDialog.SelectedPath = textBoxFolderPath1.Text;
            if (DialogResult.OK == folderBrowserDialog.ShowDialog())
            {
                textBoxFolderPath1.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void ButtonSelectFolder2Click(object sender, EventArgs e)
        {
            folderBrowserDialog.SelectedPath = textBoxFolderPath2.Text;
            if (DialogResult.OK == folderBrowserDialog.ShowDialog())
            {
                textBoxFolderPath2.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void ButtonStartComparingClick(object sender, EventArgs e)
        {
            _folder1UniqueFiles.Clear();
            _folder2UniqueFiles.Clear();
            _filesToCompare.Clear();
            _compareResult.Clear();
            _folderPath1 = textBoxFolderPath1.Text.TrimEnd('\\');
            _folderPath2 = textBoxFolderPath2.Text.TrimEnd('\\');
            _compareFilesFromSubfolders = checkBoxCompareInSubfolders.Checked;
            _includeFileMask = textBoxFileMasks.Text;
            if (string.IsNullOrEmpty(_includeFileMask))
            {
                _includeFileMask = "*.*";
            }

            if (!CheckParameters())
            {
                return;
            }

            var assemblyComparer = new AssemblyComparer(new ExternalInvoker(_logger, 0), _idlasmPath, _tempComparingPath);
            var externalInvoker = new ExternalInvoker(_logger, 0);
            _fileComparer = new FileComparer(_logger, assemblyComparer, externalInvoker, textBoxIgnoreFilesMasks.Text, checkBoxLogWrongComparedFiles.Checked);

            labelWait.Text = "Wait. Program collect information about files to compare...";
            labelWait.Visible = true;
            buttonStopComparing.Enabled = true;
            buttonStartComparing.Enabled = false;
            _stopExecution = false;

            new TaskFactory().StartNew(StartComparing);
        }

        private void StartComparing()
        {
            _logger.WriteLog("Start execution");
            _logger.WriteLog("Folder path 1: {0}", _folderPath1);
            _logger.WriteLog("Folder path 2: {0}", _folderPath2);
            _logger.WriteLog("Compare files from subfolders: {0}", _compareFilesFromSubfolders.ToString());
            _logger.WriteLog("File mask: {0}", _includeFileMask);

            _logger.WriteLog("Collect files to compare");
            CollectFilesToCompare(_folderPath1);

            _logger.WriteLog("Files to compare: {0}", _filesToCompare.Count);
            _logger.WriteLog("Unique files and folders in the folder 1: {0}", _folder1UniqueFiles.Count);
            _logger.WriteLog("Unique files and folders in the folder 2: {0}", _folder2UniqueFiles.Count);

            BeginInvoke(new VoidFunctionWithOneIntParameterDelegate(SetMaxValueToProgressBar), _filesToCompare.Count);

            _logger.WriteLog("Start file comparing");
            int i = 0;
            foreach (string filePath1 in _filesToCompare.Keys)
            {
                if (_stopExecution)
                {
                    break;
                }

                string filePath2 = _filesToCompare[filePath1];
                try
                {
                    CompareState compareState = _fileComparer.AreTwoFilesEqual(filePath1, filePath2);
                    _compareResult.Add(new KeyValuePair<string, string>(filePath1, filePath2), compareState);

                    if (compareState == CompareState.Different)
                    {
                        _logger.WriteLog("Different files were found.\r\n\t{0}\r\n\t{1}", filePath1, filePath2);
                    }
                }
                catch (Exception ex)
                {
                    _logger.WriteLog("!!!Unexpected error during comparing of two files:\r\n\t{0}\r\n\t{1}\r\n{2}", filePath1, filePath2, ex.ToString());
                    
                }

                BeginInvoke(new VoidFunctionWithOneIntParameterDelegate(SetProgressValue), ++i);
            }

            RemoveTempIdlasmFiles();

            _logger.WriteLog("Comparing was finished");

            BeginInvoke(new VoidFunctionWithoutParameters(ShowResult));
        }

        private void RemoveTempIdlasmFiles()
        {
            RemoveTempFolder(Path.Combine(Path.GetTempPath(), "1"));
            RemoveTempFolder(Path.Combine(Path.GetTempPath(), "2"));
        }

        private void RemoveTempFolder(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        private static string[] GetFiles(string sourceFolder, string filters)
        {
            return filters.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).SelectMany(filter => Directory.GetFiles(sourceFolder, filter)).ToArray();
        }

        private void CollectFilesToCompare(string folderPath1)
        {
            if (_stopExecution)
            {
                return;
            }

            string folderPath2 = folderPath1.Replace(_folderPath1, _folderPath2);

            string[] filePaths1 = GetFiles(folderPath1, _includeFileMask);
            string[] filePaths2 = GetFiles(folderPath2, _includeFileMask);

            foreach (var filePath1 in filePaths1)
            {
                string filepath2 = filePath1.Replace(_folderPath1, _folderPath2);
                if (filePaths2.Contains(filepath2))
                {
                    _filesToCompare.Add(filePath1, filepath2);
                }
                else
                {
                    _folder1UniqueFiles.Add(filePath1);
                }
            }

            foreach (var filePath2 in filePaths2)
            {
                string filepath1 = filePath2.Replace(_folderPath2, _folderPath1);
                if (!filePaths1.Contains(filepath1))
                {
                    _folder2UniqueFiles.Add(filePath2);
                }
            }

            if (!_compareFilesFromSubfolders)
            {
                return;
            }

            foreach (var subDirectory2 in Directory.GetDirectories(folderPath2))
            {
                string subDirectory1 = subDirectory2.Replace(_folderPath2, _folderPath1);
                if (!Directory.Exists(subDirectory1))
                {
                    _folder2UniqueFiles.Add(subDirectory2);
                }
            }

            foreach (var subDirectory1 in Directory.GetDirectories(folderPath1))
            {
                string subDirectory2 = subDirectory1.Replace(_folderPath1, _folderPath2);
                if (!Directory.Exists(subDirectory2))
                {
                    _folder1UniqueFiles.Add(subDirectory1);
                }
            }

            foreach (var subDirectory1 in Directory.GetDirectories(folderPath1))
            {
                string subDirectory2 = subDirectory1.Replace(_folderPath1, _folderPath2);
                if (Directory.Exists(subDirectory2))
                {
                    CollectFilesToCompare(subDirectory1);
                }
            }
        }

        private void ShowResult()
        {
            FilesList.Rows.Clear();

            if (checkBoxShowUnique.Checked)
            {
                foreach (var filePath in _folder1UniqueFiles)
                {
                    string type = Directory.Exists(filePath) ? "folder" : "file";
                    FilesList.Rows.Add(new object[] { filePath, "Unique " + type, string.Empty });
                }

                foreach (var filePath in _folder2UniqueFiles)
                {
                    string type = Directory.Exists(filePath) ? "folder" : "file";
                    FilesList.Rows.Add(new object[] { string.Empty, "Unique " + type, filePath });
                }
            }

            foreach (var filePaths in _compareResult.Keys)
            {
                if ((!checkBoxShowDifferent.Checked && _compareResult[filePaths] == CompareState.Different) ||
                    (!checkBoxShowEqual.Checked && _compareResult[filePaths] == CompareState.Equal) ||
                    (!checkBoxShowSkipped.Checked && _compareResult[filePaths] == CompareState.Skipped) ||
                    (!checkBoxShowNotCompared.Checked && _compareResult[filePaths] == CompareState.NotCompared))
                {
                    continue;
                }

                FilesList.Rows.Add(new object[] { filePaths.Key, _compareResult[filePaths].ToString(), filePaths.Value });
            }

            progressBar.Value = 0;
            buttonStopComparing.Enabled = false;
            buttonStartComparing.Enabled = true;

            Application.DoEvents();
        }

        private void SetMaxValueToProgressBar(int value)
        {
            labelWait.Visible = false;
            progressBar.Maximum = value;
            progressBar.Value = 0;
        }

        private void SetProgressValue(int value)
        {
            if (value >= 0 && value < progressBar.Maximum)
            {
                progressBar.Value = value;
            }
        }

        private bool CheckParameters()
        {
            if (!Directory.Exists(_folderPath1))
            {
                ShowWarning("Folder path 1 is wrong");
                textBoxFolderPath1.Focus();
                return false;
            }

            if (!Directory.Exists(_folderPath2))
            {
                ShowWarning("Folder path 2 is wrong");
                textBoxFolderPath2.Focus();
                return false;
            }

            if (_folderPath1 == _folderPath2)
            {
                ShowWarning("Folders must be different");
                textBoxFolderPath1.Focus();
                return false;
            }

            if (!File.Exists(_idlasmPath))
            {
                ShowWarning("File 'ildasm.exe' is not found");
                return false;
            }

            try
            {
                _logger = new FileAndConsoleLogger(Path.Combine(Application.StartupPath, "FilesComparerLog.txt"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error with logging:\r\n" + ex.Message, "Critical error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void ShowWarning(string text)
        {
            MessageBox.Show(text, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void SelectedShowModeChanged(object sender, EventArgs e)
        {
            ShowResult();
        }

        private void ButtonStopComparingClick(object sender, EventArgs e)
        {
            _stopExecution = true;
            _logger.WriteLog("User press Stop button. Execution stopping");
        }

        /// <summary>
        /// Compare selected files in compare tool (create il file for dll and exe)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilesListCellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (FilesList.SelectedRows.Count == 0 || FilesList.SelectedRows[0].Cells[1].Value.ToString().StartsWith("Unique"))
            {
                return;
            }

            labelWait.Text = "Wait. Program is comparing two files...";
            labelWait.Visible = true;
            Application.DoEvents();

            string filePath1 = FilesList.SelectedRows[0].Cells[0].Value.ToString();
            string filePath2 = FilesList.SelectedRows[0].Cells[2].Value.ToString();

            _fileComparer.CompareTwoFilesInAraxis(filePath1, filePath2, _tempComparingPath);

            labelWait.Visible = false;
        }
    }
}
