using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Reports
{
    public class InboundHandlerOperationsProvider : IInboundHandlerOperationsProvider
    {
        private static readonly List<InboundHandlerOperation> Operations = GetOperationsNamesBasedOnLocale(); 

        public List<InboundHandlerOperation> GetAll()
        {
            return Operations;
        }

        private static List<InboundHandlerOperation> GetOperationsNamesBasedOnLocale()
        {
            //currently we just get English version

            var operations = new List<InboundHandlerOperation>
            {
                new InboundHandlerOperation { Title = Strings.InboundOperationUndefined, Id = InboundHandlerOperationType.Undefined },
                new InboundHandlerOperation { Title = Strings.InboundOperationPlacedInQueue, Id = InboundHandlerOperationType.PlacedInQueue },
                new InboundHandlerOperation { Title = Strings.InboundOpeationSentToDialer, Id = InboundHandlerOperationType.SendToDialer },
                new InboundHandlerOperation { Title = Strings.DropByRespondent, Id = InboundHandlerOperationType.DropByRespondent },
                new InboundHandlerOperation { Title = Strings.DropBySystemInterviewNotFound, Id = InboundHandlerOperationType.DropBySystemInterviewNotFound },
                new InboundHandlerOperation { Title = Strings.DropBySystemInboundDisabled, Id = InboundHandlerOperationType.DropBySystemInboundDisabled },
                new InboundHandlerOperation { Title = Strings.DropBySystemDdiRecordNotFound, Id = InboundHandlerOperationType.DropBySystemDdiRecordNotFound },
                new InboundHandlerOperation { Title = Strings.DropBySystemWrongCallState, Id = InboundHandlerOperationType.DropBySystemWrongCallState },
                new InboundHandlerOperation { Title = Strings.DropBySchedulingScript, Id = InboundHandlerOperationType.DropBySchedulingScript },
                new InboundHandlerOperation { Title = Strings.DropBySystemSurveyIsNotOpened, Id = InboundHandlerOperationType.DropBySystemSurveyIsNotOpened },
                new InboundHandlerOperation { Title = Strings.DropBySystemSurveyIsNotFound, Id = InboundHandlerOperationType.DropBySystemSurveyIsNotFound },
                new InboundHandlerOperation { Title = Strings.DropBySystemShiftIsNotFound, Id = InboundHandlerOperationType.DropBySystemShiftIsNotFound },
                new InboundHandlerOperation { Title = Strings.DropBySystemNoAgentsAvailable, Id = InboundHandlerOperationType.DropBySystemNoAgentsAvailable },
                new InboundHandlerOperation { Title = Strings.DropBySystemInternalServerError, Id = InboundHandlerOperationType.DropBySystemInternalServerError },
                new InboundHandlerOperation { Title = Strings.ConnectedToAgent, Id = InboundHandlerOperationType.ConnectedToAgent },
            };

            return operations;
        }
    }
}