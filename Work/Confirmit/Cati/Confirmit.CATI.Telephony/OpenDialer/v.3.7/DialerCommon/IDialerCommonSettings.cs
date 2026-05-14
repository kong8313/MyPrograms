namespace DialerCommon
{
    public interface IDialerCommonSettings
    {
        string MonitorRoot { get; }

        string UrlRoot { get; }

        string FileNamePattern { get; }

        int LogHealthCheckPeriodInMinutes { get; }
    }
}