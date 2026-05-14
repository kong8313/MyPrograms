using Confirmit.CATI.Core.DAL.Framework.BulkCopy;
using Confirmit.CATI.Common.ServiceLocation;

namespace Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging
{
    public class InterviewerActivityEventRegistry : IServiceLocatorRegistry
    {
        public void RegisterTypes(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator
                .Register<IBulkCopyEntitySerializer<IInterviewerActivityEventBase>, InterviewerActivityEventsBulkCopySerializer>()
                .RegisterSingleton<IBulkCopyEntityAccumulator<IInterviewerActivityEventBase>, BulkCopyEntityAccumulator<IInterviewerActivityEventBase> >();
        }
    }
}
