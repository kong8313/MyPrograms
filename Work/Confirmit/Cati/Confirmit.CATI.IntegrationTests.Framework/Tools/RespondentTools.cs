using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Common.ServiceLocation;

namespace Confirmit.CATI.IntegrationTests.Framework.Tools
{
    public class RespondentTools
    {
        private readonly IntegrationTestingFramework _framework;

        public RespondentTools(IntegrationTestingFramework framework)
        {
            _framework = framework;
        }

        public AsyncOperationResult DeleteRespondentsAsync(string projectId, int[] respIDs)
        {
            var operationId = new ManagementService().DeleteRespondentsAsync(respIDs, projectId);
            var operation = ServiceLocator.Resolve<IAsyncOperationRepository>().Get(operationId);
            return ServiceLocator.Resolve<IAsyncOperationExecutor>().ExecuteOperationSync(operation);
        }
    }
}