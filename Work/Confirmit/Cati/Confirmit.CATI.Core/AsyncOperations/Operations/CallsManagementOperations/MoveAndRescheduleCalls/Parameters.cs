using System;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Batch;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.MoveAndRescheduleCalls
{
    [Serializable]
    public class Parameters : IAsyncOperationParameters
    {
        public int SurveyId { get; set; }
        public BatchParameters BatchParameters { get; set; }
        public int StateId { get; set; }
        public Appointment AppointmentPrm { get; set; }
    }
}
