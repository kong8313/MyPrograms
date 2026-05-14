namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface IFeatureToggleClient
    {
        FeatureToggleAccessResult FeatureToggleAccess(string toggleName);
    }

    public class FeatureToggleAccessResult
    {
        public bool HasAccess { get; set; }
    }
}