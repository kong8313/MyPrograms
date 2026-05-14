using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Batch;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.ChangeDialModeOfInterviews
{
    [Serializable]
    public class Parameters : IAsyncOperationParameters
    {
        public int SurveyId { get; set; }
        public BatchParameters BatchParameters { get; set; }
        public DialingMode? DialingMode { get; set; }
    }
}
