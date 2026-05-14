using Confirmit.CATI.Installation.Common.Interfaces;

namespace BootstrapperLibrary.Interfaces
{
    public interface IObjectFactory
    {
        IExternalInvoker CreateExternalInvokerObject(ILogger logger, int successCode);

        ICertificateEngine CreateCertificateEngineObject(IDialogService dialogService);

        IInstalledProductsReader CreateInstalledProductsReaderObject();
        
        IInstalledProductSearcher CreateInstalledProductsSearcherObject(string currentProductName, string productNameMask, ILogger logger);

        IPrereqChecker CreatePrereqCheckerObject();
        
        IConfirmitCATIValidator CreateConfirmitCATIValidatorObject();

        IDatabaseEngine CreateDatabaseEngineObject(string sqlServerName, string sqlUser, string sqlPassword);

        IDialogService CreateDialogservice();
    }
}
