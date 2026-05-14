namespace Confirmit.CATI.Core.Misc.ConfirmitClientKey
{
    public class BackendConfirmitClientKeyProvider : IConfirmitClientKeyProvider
    {
        public string Get()
        {
            //TODO: Security Critical: here we should use real client key. So we need to have real user which will used only for auth inside CATI backend.
            return "Cati.Backend-ACCD8359-BC33-4BC4-9BA1-4631E07F9B08";
        }
    }
}