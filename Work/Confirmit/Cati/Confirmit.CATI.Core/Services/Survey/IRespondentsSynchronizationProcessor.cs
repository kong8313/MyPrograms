using System.Threading;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation;

namespace Confirmit.CATI.Core.Services.Survey
{
    public interface IRespondentsSynchronizationProcessor
    {
        void SynchronizeRespondents(RespondentsSynchronizationContext context, CancellationToken cancellationToken);
        RespondentRecord[] Records { get; }
    }
}