namespace Confirmit.CATI.Core.SystemSettings
{
    public partial interface IToggleSettings
    {
        bool ShowDialType { get; }
    }

    public partial class ToggleSettings
    {
       public bool ShowDialType => EnableTCPA || EnableAgentAssistedDialling;
    }
}
