using System.Windows.Forms;
using BootstrapperLibrary.Interfaces;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace Confirmit.CATI.Setup.UnitTests.FakeClasses
{
    public class FakeObjectFactory : IObjectFactory
    {
        public bool CreateRealPrereqChecker { get; set; }        
        public bool IsFramework462Installed { get; set; }

        public DialogResult DefaultDialogResult { get; set; }

        public bool DoesExecutionFinishWithError { get; set; }
        public string Output { get; set; }

        public bool IsProductAlreadyInstalled { get; set; }

        public FakeDialogService CreatedFakeDialogService { get; private set; }

        public FakeObjectFactory()
        {
            CreateRealPrereqChecker = false;
            IsFramework462Installed = true;

            DefaultDialogResult = DialogResult.OK;

            DoesExecutionFinishWithError = false;
            Output = string.Empty;

            IsProductAlreadyInstalled = true;
        }

        public IExternalInvoker CreateExternalInvokerObject(ILogger logger, int successCode)
        {
            return new FakeExternalInvoker(DoesExecutionFinishWithError, Output);
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
            throw new System.NotImplementedException();
        }

        public IPrereqChecker CreatePrereqCheckerObject()
        {
            if (CreateRealPrereqChecker)
            {
                return new PrereqChecker();
            }

            return new FakePrereqChecker(IsFramework462Installed);
        }

        public IConfirmitCATIValidator CreateConfirmitCATIValidatorObject()
        {
            return new ConfirmitCATIValidator();
        }

        public IDatabaseEngine CreateDatabaseEngineObject(string connectionString)
        {
            return new FakeDatabaseEngine();
        }

        public IDatabaseEngine CreateDatabaseEngineObject(string sqlServerName, string sqlUser, string sqlPassword)
        {
            return new FakeDatabaseEngine();
        }

        public IDialogService CreateDialogservice()
        {
            CreatedFakeDialogService = new FakeDialogService(DefaultDialogResult);
            return CreatedFakeDialogService;
        }
    }
}