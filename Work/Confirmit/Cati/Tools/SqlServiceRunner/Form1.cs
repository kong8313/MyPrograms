using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace SqlServiceRunner
{
    public partial class MainForm : Form
    {
        private readonly Logger _logger;
        private readonly ServicesRunner _servicesRunner;

        public MainForm()
        {
            InitializeComponent();

            _logger = new Logger(richTextBoxLog);
            _servicesRunner = new ServicesRunner(_logger);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
            
            try
            {
                AddToAutoStart();
            }
            catch (Exception ex)
            {
                _logger.WriteLog($"App cannot be added to autoraun because an error: {ex.Message}. App continue to work as usual.");
            }

            timer.Enabled = true;
        }

        private void buttonExcludeFromAutostart_Click(object sender, EventArgs e)
        {
            try
            {
                RemoveFromAutostart();

                _logger.WriteLog("App was removed from autorun. It will be added to autorun after next start.");
            }
            catch (Exception ex)
            {
                _logger.WriteLog("App cannot be removed from autorun because an error: " + ex.Message);
            }
        }

        const string AutostartRegPath = @"Software\Microsoft\Windows\CurrentVersion\Run";

        private void AddToAutoStart()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(AutostartRegPath, true))
            {
                key.SetValue("SqlServiceRunnerApp", Application.ExecutablePath);
            }
        }

        private void RemoveFromAutostart()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(AutostartRegPath, true))
            {
                key.DeleteValue("SqlServiceRunnerApp", false);
            }
        }

        /// <summary>
        /// Check if symantec window is appeared and press "Allow the file" button on it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            _servicesRunner.VerifyAndRun();
        }
    }
}
