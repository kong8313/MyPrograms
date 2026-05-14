namespace DialerCommon
{
    public class DialerCommonSettings : IDialerCommonSettings
    {
        public string MonitorRoot 
        {
            get
            {
                return Properties.Settings.Default.MonitorRoot;
            }
        }

        public string UrlRoot
        {
            get
            {
                return Properties.Settings.Default.UrlRoot;
            }
        }

        public string FileNamePattern
        {
            get
            {
                return Properties.Settings.Default.FileNamePattern;
            }
        }

        public int LogHealthCheckPeriodInMinutes
        {
            get
            {
                return Properties.Settings.Default.LogHealthCheckPeriodInMinutes;
            }
        }
    }
}