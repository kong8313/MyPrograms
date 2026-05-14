using BootstrapperLibrary.Interfaces;

using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace BootstrapperLibrary
{
    public class ObjectFactory : IObjectFactory
    {
        public IExternalInvoker CreateExternalInvokerObject(ILogger logger, int successCode)
        {
            return new ExternalInvoker(logger, successCode);
        }

        public ICertificateEngine CreateCertificateEngineObject(IDialogService dialogService)
        {
            return new CertificateEngine(dialogService);
        }

        public IInstalledProductsReader CreateInstalledProductsReaderObject()
        {
            return new InstalledProductsReader();
        }

        public IInstalledProductSearcher CreateInstalledProductsSearcherObject(string currentProductName, string productNameMask, ILogger logger)
        {
            return new InstalledProductSearcher(currentProductName, productNameMask, logger, CreateInstalledProductsReaderObject());
        }

        public IPrereqChecker CreatePrereqCheckerObject()
        {
            return new PrereqChecker();
        }      

        public IConfirmitCATIValidator CreateConfirmitCATIValidatorObject()
        {
            return new ConfirmitCATIValidator();
        }

        public IDatabaseEngine CreateDatabaseEngineObject(string connectionString)
        {
            return new DatabaseEngine(connectionString);
        }

        public IDatabaseEngine CreateDatabaseEngineObject(string sqlServerName, string sqlUser, string sqlPassword)
        {
            return new DatabaseEngine(sqlServerName, sqlUser, sqlPassword);
        }
        
        public IDialogService CreateDialogservice()
        {
            return new DialogService();
        }
    }
}
