using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DialerWsLogParserLibrary;
using Microsoft.Win32;

namespace DialerWsLogParser
{
    public partial class MainWindow : Window
    {
        private FileReader _fileReader;
        private Parser _parser;
        private readonly Converter _converter;
        private readonly DataContractJsonSerializer _jsonFormatter;
        private readonly string _settingsFileName;
        private string _startTime;
        private string _finishTime;

        public MainWindow()
        {
            InitializeComponent();

            _fileReader = new FileReader();
            _parser = new Parser();
            _converter = new Converter();
            _jsonFormatter = new DataContractJsonSerializer(typeof(Settings));

            _settingsFileName = "FilterConditions.json";

            AllowDrop = true;
            DragEnter += new DragEventHandler(MainWindow_DragEnter);
            Drop += new DragEventHandler(MainWindow_Drop);

            LoadSettings();
        }

        private void ShowWarningDialog(string message)
        {
            MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void ShowErrorDialog(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void AddFiles(string[] files)
        {
            try
            {
                foreach (var file in files)
                {
                    if (!_fileReader.FileNames.Contains(file))
                    {
                        if (Path.GetExtension(file) == ".zip")
                            _fileReader.ReadFilesFromArchive(file);
                        else
                            _fileReader.ReadFile(file);
                    }
                    else
                    {
                        ShowWarningDialog(string.Format("{0} has already been added!", file));
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorDialog("Could not read file from disk. Original error: " + ex.Message);
            }
        }

        private void MainWindow_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
        }

        private void MainWindow_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            AddFiles(files);
            DataGrid_EventsGroups_Loaded(sender, e);
        }

        private string FindMinStartTime()
        {
            string result = _parser.FilteredEventsGroups[0].StartTime;

            for (int i = 1; i < _parser.FilteredEventsGroups.Count; i++)
                if (_parser.FilteredEventsGroups[i].StartTime != String.Empty && String.Compare(_parser.FilteredEventsGroups[i].StartTime, result) < 0)
                    result = _parser.FilteredEventsGroups[i].StartTime;

            return result;
        }

        private string FindMaxFinishTime()
        {
            string result = _parser.FilteredEventsGroups[0].FinishTime;

            for (int i = 1; i < _parser.FilteredEventsGroups.Count; i++)
                if (String.Compare(_parser.FilteredEventsGroups[i].FinishTime, result) > 0)
                    result = _parser.FilteredEventsGroups[i].FinishTime;

            return result;
        }

        private async void DataGrid_EventsGroups_Loaded(object sender, RoutedEventArgs e)
        {
            MainPanel.IsEnabled = false;
            FilterGrid.IsEnabled = false;
            Cursor = Cursors.Wait;
            var settings = new ParseSettings
            {
                DialerServiceOnly = checkBox_DialerServiceOnly.IsChecked ?? false,
                ExcludeGetState = checkBox_ExcludeGetState.IsChecked ?? false,
                ExcludeDuplicateNotifications = checkBox_ExcludeDuplicateNotifications.IsChecked ?? false,
                ExcludeOnHook = checkBox_ExcludeOnHook.IsChecked ?? false
            };

            await Task.Run(() =>
            {
                _parser.ParseEventsAndGroups(_fileReader.Text, settings);

            });

            if (Filter_StartTime.Text == _startTime)
                Filter_StartTime.Text = string.Empty;
            if (Filter_FinishTime.Text == _finishTime)
                Filter_FinishTime.Text = string.Empty;

            _parser.FillFilteredEventsGroups(Filter_Name.Text, Filter_StartTime.Text, Filter_FinishTime.Text, Filter_CompanyId.Text, Filter_DialerId.Text, Filter_CampaignId.Text,
                Filter_AgentId.Text, Filter_CallId.Text, Filter_InterviewId.Text, Filter_Duration.Text, Filter_AllInfo.Text);

            DataGrid_EventsGroups.ItemsSource = _parser.FilteredEventsGroups;

            if (_parser.FilteredEventsGroups.Count > 0)
            {
                if (Filter_StartTime.Text == String.Empty)
                {
                    _startTime = FindMinStartTime();
                    Filter_StartTime.Text = _startTime;
                }
                if (Filter_FinishTime.Text == String.Empty)
                {
                    _finishTime = FindMaxFinishTime();
                    Filter_FinishTime.Text = _finishTime;
                }
            }

            Cursor = Cursors.Arrow;
            FilterGrid.IsEnabled = true;
            MainPanel.IsEnabled = true;
            DataGrid_EventsGroups.SelectedIndex = -1;
            DataGrid_Events.ItemsSource = null;

            statusBarText.Text = "Total count of groups: " + _parser.EventsGroups.Count + ";";

            Text_UploadedFiles.Items.Clear();
            if (_fileReader.FileNames.Count > 0)
            {
                foreach (var file in _fileReader.FileNames)
                    Text_UploadedFiles.Items.Add(file);
            }

            FileMenu.Items.Clear();
            MenuItem fileMenuItem = InitFileMenuItem();

            if (_fileReader.RecentFileNames.Count > 0)
            {
                for (var i = 0; i < _fileReader.RecentFileNames.Count; i++)
                {
                    MenuItem recentFileItem = new MenuItem
                    {
                        Header = string.Format("{0}: {1}", i + 1, _fileReader.RecentFileNames[i])
                    };
                    recentFileItem.Click += new RoutedEventHandler(MenuItem_RecentFile_Click);

                    fileMenuItem.Items.Add(recentFileItem);
                }
            }

            FileMenu.Items.Add(fileMenuItem);
        }

        private MenuItem InitFileMenuItem()
        {
            MenuItem result = new MenuItem{Header = "File"};

            MenuItem openItem = new MenuItem{Header = "Open"};
            openItem.Click += new RoutedEventHandler(MenuItem_File_Click);
            result.Items.Add(openItem);

            MenuItem openLastLogItem = new MenuItem{Header = "Open last log"};
            openLastLogItem.Click += new RoutedEventHandler(MenuItem_LastLog_Click);
            result.Items.Add(openLastLogItem);

            MenuItem clearTimeItem = new MenuItem{Header = "Clear time filters"};
            clearTimeItem.Click += new RoutedEventHandler(MenuItem_ClearTimeFilters_Click);
            result.Items.Add(clearTimeItem);

            MenuItem clearItem = new MenuItem{Header = "Clear"};
            clearItem.Click += new RoutedEventHandler(MenuItem_Clear_Click);
            result.Items.Add(clearItem);

            result.Items.Add(new Separator());

            return result;
        }

        private void MenuItem_RecentFile_Click(object sender, RoutedEventArgs e)
        {
            string fileName = ((MenuItem)sender).Header.ToString();

            if (fileName[1] == '0')
                fileName = fileName.Substring(4);
            else
                fileName = fileName.Substring(3);

            AddFiles(new string[] { fileName });
            DataGrid_EventsGroups_Loaded(sender, e);
        }

        private void DataGrid_EventsGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedIndex = DataGrid_EventsGroups.SelectedIndex;

            if (selectedIndex > -1)
            {
                var items = DataGrid_EventsGroups.Items;
                long selectedRId = ((EventsGroup)items[selectedIndex]).RequestId;
                List<EventView> selectedEvents = new List<EventView>();

                foreach (var entry in _parser.Events)
                    if (entry.RequestId == selectedRId)
                    {
                        if (Filter_AllInfo.Text != string.Empty && entry.AllInfo.Contains(Filter_AllInfo.Text))
                            entry.IsHighlighted = true;
                        selectedEvents.Add(_converter.EventToEventView(entry));
                    }

                DataGrid_Events.ItemsSource = selectedEvents;
                
                if (selectedEvents.Count == 1)
                    Text_AllInfo.Text = selectedEvents[0].AllInfo;
                else
                    Text_AllInfo.Text = String.Empty;
            }
        }

        private void DataGrid_EventsGroups_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedCell = (DataGridCell)sender;

            foreach (TextBox textBox in FilterGrid.Children)
                if (textBox.Name.Contains((string)selectedCell.Column.Header))
                {
                    textBox.Text = ((TextBlock)selectedCell.Content).Text;
                    break;
                }
        }

        private void DataGrid_Events_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selIndex = DataGrid_Events.SelectedIndex;

            if (selIndex > -1)
            {
                var items = DataGrid_Events.Items;

                Text_AllInfo.Text = ((EventView)items[selIndex]).AllInfo;
            }
        }

        private void DataGrid_EventsGroups_LayoutUpdated(object sender, EventArgs e)
        {
            for (var i = 1; i < DataGrid_EventsGroups.Columns.Count; i++)
            {
                if (!double.IsNaN(DataGrid_EventsGroups.Columns[i].Width.DisplayValue))
                {
                    if (DataGrid_EventsGroups.Columns[i].Visibility == Visibility.Hidden)
                        FilterGrid.ColumnDefinitions[i - 1].Width = new GridLength(0);                 
                    else
                    {
                        if (i == 1)
                            FilterGrid.ColumnDefinitions[0].Width = new GridLength(DataGrid_EventsGroups.Columns[0].Width.DisplayValue +
                                DataGrid_EventsGroups.Columns[1].Width.DisplayValue);
                        else
                            FilterGrid.ColumnDefinitions[i - 1].Width = new GridLength(DataGrid_EventsGroups.Columns[i].Width.DisplayValue);
                    }
                }
            }
        }

        private async void MenuItem_File_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (Directory.Exists("C:\\DialerLogs"))
                openFileDialog.InitialDirectory = "C:\\DialerLogs";
            else
                openFileDialog.InitialDirectory = "C:\\";

            openFileDialog.Filter = "Log files (*.log)|*.log|Zip-archives (*.zip)|*.zip";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true)
            {
                statusBarText.Text += "  New file is loaded...";
                await Task.Run(() =>
                {
                    AddFiles(openFileDialog.FileNames);
                });
                DataGrid_EventsGroups_Loaded(sender, e);
            }
        }

        private void MenuItem_LastLog_Click(object sender, RoutedEventArgs e)
        {
            DateTime dateTimeLastFile = new DateTime();
            string[] lastFileName = new string[1];

            FileSystemInfo[] fileSystemInfos = new DirectoryInfo(@"C:\DialerLogs").GetFileSystemInfos();
            foreach (FileSystemInfo fileSI in fileSystemInfos)
            {
                if (fileSI.Extension == ".log")
                {
                    if (dateTimeLastFile == DateTime.MinValue || dateTimeLastFile < Convert.ToDateTime(fileSI.CreationTime))
                    {
                        dateTimeLastFile = Convert.ToDateTime(fileSI.CreationTime);
                        lastFileName[0] = fileSI.FullName;
                    }
                }
            }

            AddFiles(lastFileName);
            DataGrid_EventsGroups_Loaded(sender, e);
        }

        private void Filter(object sender, RoutedEventArgs e)
        {
            if (Filter_StartTime.Text == _startTime)
                Filter_StartTime.Text = string.Empty;
            if (Filter_FinishTime.Text == _finishTime)
                Filter_FinishTime.Text = string.Empty;

            if (Filter_AllInfo.Text == string.Empty)
                _parser.ResetMatchingCondition();

            _parser.FillFilteredEventsGroups(Filter_Name.Text, Filter_StartTime.Text, Filter_FinishTime.Text, Filter_CompanyId.Text, Filter_DialerId.Text, Filter_CampaignId.Text,
                Filter_AgentId.Text, Filter_CallId.Text, Filter_InterviewId.Text, Filter_Duration.Text, Filter_AllInfo.Text);

            DataGrid_EventsGroups.ItemsSource = _parser.FilteredEventsGroups;

            if (_parser.FilteredEventsGroups.Count > 0)
            {
                if (Filter_StartTime.Text == string.Empty)
                {
                    _startTime = FindMinStartTime();
                    Filter_StartTime.Text = _startTime;
                }
                if (Filter_FinishTime.Text == string.Empty)
                {
                    _finishTime = FindMaxFinishTime();
                    Filter_FinishTime.Text = _finishTime;
                }
            }

            DataGrid_EventsGroups.SelectedIndex = -1;
            DataGrid_Events.ItemsSource = null;
            Text_AllInfo.Text = string.Empty;
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Filter(sender, e);
        }

        private void MenuItem_ClearTimeFilters_Click(object sender, RoutedEventArgs e)
        {
            Filter_StartTime.Text = string.Empty;
            Filter_FinishTime.Text = string.Empty;
        }

        private void MenuItem_Clear_Click(object sender, RoutedEventArgs e)
        {
            _parser.Reset();
            _fileReader.Clean();

            DataGrid_EventsGroups.ItemsSource = null;
            DataGrid_Events.ItemsSource = null;
            Text_AllInfo.Text = string.Empty;

            if(Filter_StartTime.Text == _startTime)
            {
                _startTime = string.Empty;
                Filter_StartTime.Text = string.Empty;
            }
            if (Filter_FinishTime.Text == _finishTime)
            {
                _finishTime = string.Empty;
                Filter_FinishTime.Text = string.Empty;
            }
            Text_UploadedFiles.Items.Clear();
            statusBarText.Text = "Total count of groups: 0";
        }

        private void rb_AndOr_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is RadioButton radioButton))
                return;

            if (radioButton.Name == "ConditionAnd")
                _parser.ParserSettings.SetConditionalOperatorAnd();
            else if (radioButton.Name == "ConditionOr")
                _parser.ParserSettings.SetConditionalOperatorOr();

            Filter(sender, e);
        }

        private void rb_PosNeg_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is RadioButton radioButton))
                return;

            if (radioButton.Name == "PositiveСoincidence")
                _parser.ParserSettings.SetCoincidenceOperatorPos();
            else if (radioButton.Name == "NegativeСoincidence")
                _parser.ParserSettings.SetCoincidenceOperatorNeg();

            Filter(sender, e);
        }

        private void LoadSettings()
        {
            var isFileExist = File.Exists(_settingsFileName);

            if (!isFileExist || (isFileExist && !UseStoredSettings()))
            {
                UseDefaultSettings();
            }
        }

        private bool UseStoredSettings()
        {
            try
            {
                using (FileStream fs = new FileStream(_settingsFileName, FileMode.OpenOrCreate))
                {
                    DialerWsLogParserLibrary.Settings conditionHandler = (DialerWsLogParserLibrary.Settings)_jsonFormatter.ReadObject(fs);
                    _parser.SetSettings(conditionHandler);
                }

                if (_parser.ParserSettings.IsConditionalOperatorAnd)
                    ConditionAnd.IsChecked = true;
                else
                    ConditionOr.IsChecked = true;

                if (_parser.ParserSettings.IsCoincidenceOperatorPositive)
                    PositiveСoincidence.IsChecked = true;
                else
                    NegativeСoincidence.IsChecked = true;

                _fileReader = new FileReader();
                _fileReader.SetRecentFileNames(_parser.ParserSettings.RecentFiles); 

                Filter_Name.Text = _parser.ParserSettings.Filter.Name;
                Filter_StartTime.Text = _parser.ParserSettings.Filter.StartTime;
                Filter_FinishTime.Text = _parser.ParserSettings.Filter.FinishTime;
                Filter_CompanyId.Text = _parser.ParserSettings.Filter.CompanyId;
                Filter_DialerId.Text = _parser.ParserSettings.Filter.DialerId;
                Filter_CampaignId.Text = _parser.ParserSettings.Filter.CampaignId;
                Filter_AgentId.Text = _parser.ParserSettings.Filter.AgentId;
                Filter_CallId.Text = _parser.ParserSettings.Filter.CallId;
                Filter_InterviewId.Text = _parser.ParserSettings.Filter.InterviewId;
                Filter_Duration.Text = _parser.ParserSettings.Filter.Duration;
                Filter_AllInfo.Text = _parser.ParserSettings.Filter.AllInfo;

                checkBox_StartTime.IsChecked = _parser.ParserSettings.ColumnHandler.StartTime;
                checkBox_FinishTime.IsChecked = _parser.ParserSettings.ColumnHandler.FinishTime;
                checkBox_CompanyId.IsChecked = _parser.ParserSettings.ColumnHandler.CompanyId;
                checkBox_DialerId.IsChecked = _parser.ParserSettings.ColumnHandler.DialerId;
                checkBox_CampaignId.IsChecked = _parser.ParserSettings.ColumnHandler.CampaignId;
                checkBox_AgentId.IsChecked = _parser.ParserSettings.ColumnHandler.AgentId;
                checkBox_CallId.IsChecked = _parser.ParserSettings.ColumnHandler.CallId;
                checkBox_InterviewId.IsChecked = _parser.ParserSettings.ColumnHandler.InterviewId;
                checkBox_Duration.IsChecked = _parser.ParserSettings.ColumnHandler.Duration;

                DataGrid_EventsGroups.Columns.FirstOrDefault(w => w.Header.ToString() == "StartTime").Visibility = _parser.ParserSettings.ColumnHandler.StartTime ? Visibility.Visible : Visibility.Hidden;
                DataGrid_EventsGroups.Columns.FirstOrDefault(w => w.Header.ToString() == "FinishTime").Visibility = _parser.ParserSettings.ColumnHandler.FinishTime ? Visibility.Visible : Visibility.Hidden;
                DataGrid_EventsGroups.Columns.FirstOrDefault(w => w.Header.ToString() == "CompanyId").Visibility = _parser.ParserSettings.ColumnHandler.CompanyId ? Visibility.Visible : Visibility.Hidden;
                DataGrid_EventsGroups.Columns.FirstOrDefault(w => w.Header.ToString() == "DialerId").Visibility = _parser.ParserSettings.ColumnHandler.DialerId ? Visibility.Visible : Visibility.Hidden;
                DataGrid_EventsGroups.Columns.FirstOrDefault(w => w.Header.ToString() == "CampaignId").Visibility = _parser.ParserSettings.ColumnHandler.CampaignId ? Visibility.Visible : Visibility.Hidden;
                DataGrid_EventsGroups.Columns.FirstOrDefault(w => w.Header.ToString() == "AgentId").Visibility = _parser.ParserSettings.ColumnHandler.AgentId ? Visibility.Visible : Visibility.Hidden;
                DataGrid_EventsGroups.Columns.FirstOrDefault(w => w.Header.ToString() == "CallId").Visibility = _parser.ParserSettings.ColumnHandler.CallId ? Visibility.Visible : Visibility.Hidden;
                DataGrid_EventsGroups.Columns.FirstOrDefault(w => w.Header.ToString() == "InterviewId").Visibility = _parser.ParserSettings.ColumnHandler.InterviewId ? Visibility.Visible : Visibility.Hidden;
                DataGrid_EventsGroups.Columns.FirstOrDefault(w => w.Header.ToString() == "Duration").Visibility = _parser.ParserSettings.ColumnHandler.Duration ? Visibility.Visible : Visibility.Hidden;
            }
            catch (Exception ex)
            {
                ShowErrorDialog("Could not load stored settings. Original error: " + ex.Message);
                File.Delete(_settingsFileName);
                return false;
            }

            return true;
        }

        private void UseDefaultSettings()
        {
            checkBox_StartTime.IsChecked = true;
            checkBox_FinishTime.IsChecked = true;
            checkBox_CompanyId.IsChecked = true;
            checkBox_DialerId.IsChecked = true;
            checkBox_CampaignId.IsChecked = true;
            checkBox_AgentId.IsChecked = true;
            checkBox_CallId.IsChecked = true;
            checkBox_InterviewId.IsChecked = true;
            checkBox_Duration.IsChecked = true;

            ConditionAnd.IsChecked = true;
            PositiveСoincidence.IsChecked = true;

            _parser.ParserSettings.SetColumnsVisibility(true, true, true, true, true, true, true, true, true);
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox checkBox))
                return;

            var targetDataGridColumn = DataGrid_EventsGroups.Columns.FirstOrDefault(w => w.Header != null && w.Header.ToString() == checkBox.Content.ToString());
            var targetGridColumn = FilterGrid.ColumnDefinitions.FirstOrDefault(w => w.Tag != null && w.Tag.ToString() == checkBox.Content.ToString());

            if (targetDataGridColumn == null)
                return;

            if ((bool)checkBox.IsChecked)
            {
                targetDataGridColumn.Visibility = Visibility.Visible;
            }
            else
            {
                targetDataGridColumn.Visibility = Visibility.Hidden;
            }
            
            _parser.ParserSettings.ColumnHandler.SetColumn(targetDataGridColumn.Header.ToString(), (bool)checkBox.IsChecked);
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (File.Exists(_settingsFileName))
                File.WriteAllText(_settingsFileName, string.Empty);

            _parser.ParserSettings.SetRecentFiles(_fileReader.RecentFileNames);

            if (Filter_StartTime.Text == _startTime)
                _parser.ParserSettings.Filter.SetColumn("StartTime", string.Empty);
            if (Filter_FinishTime.Text == _finishTime)
                _parser.ParserSettings.Filter.SetColumn("FinishTime", string.Empty);

            using (FileStream fs = new FileStream(_settingsFileName, FileMode.OpenOrCreate))
            {
                _jsonFormatter.WriteObject(fs, _parser.ParserSettings);
            }
        }

        private void checkBox_filter_Checked(object sender, RoutedEventArgs e)
        {
            DataGrid_EventsGroups_Loaded(sender, e);
        }
    }
}
