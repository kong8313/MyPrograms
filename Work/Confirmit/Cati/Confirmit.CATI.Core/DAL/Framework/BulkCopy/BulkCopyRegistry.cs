using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Common.ServiceLocation;

namespace Confirmit.CATI.Core.DAL.Framework.BulkCopy
{
    public class BulkCopyRegistry : IServiceLocatorRegistry
    {
        public void RegisterTypes(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.Register<IBulkCopy, BulkCopy>();
            serviceRegistrator.RegisterSingleton<IBulkCopyCommiter, BulkCopyCommiter>();
            serviceRegistrator.Register<IBulkCopyEntityAccumulator<IInterviewerActivityEventBase>, BulkCopyEntityAccumulator<IInterviewerActivityEventBase>>();
            serviceRegistrator.Register<IBulkCopyEntitySerializer<IInterviewerActivityEventBase>, BulkCopyEntitySerializerBase<IInterviewerActivityEventBase>>();
        }
    }
}
