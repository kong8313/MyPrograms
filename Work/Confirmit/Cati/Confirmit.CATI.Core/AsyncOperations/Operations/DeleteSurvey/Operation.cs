using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.Services.Survey;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.DeleteSurvey
{
    public class Operation : IAsyncOperation
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly IInboundTelephoneNumberRepository _inboundTelephoneNumberRepository;
        private readonly ISurveyService _surveyService;
        private readonly ISurveyStateService _surveyStateService;
        private readonly ISurveyPublishService _surveyPublishService;
        private readonly IReplicationSchemaService _replicationSchemaService;

        public Operation(ISurveyRepository surveyRepository,
                        IInboundTelephoneNumberRepository inboundTelephoneNumberRepository,
                        ISurveyService surveyService,
                        ISurveyStateService surveyStateService,
                        ISurveyPublishService surveyPublishService,
                        IReplicationSchemaService replicationSchemaService)
        {
            _surveyRepository = surveyRepository;
            _inboundTelephoneNumberRepository = inboundTelephoneNumberRepository;
            _surveyService = surveyService;
            _surveyStateService = surveyStateService;
            _surveyPublishService = surveyPublishService;
            _replicationSchemaService = replicationSchemaService;
        }

        public IOperationDescriptor Descriptor
        {
            get { return new Descriptor(); }
        }

        private Parameters DeserializeParameters(string parameters)
        {
            var serializer = new XmlSerializer(typeof(Parameters));

            using (var reader = new StringReader(parameters))
            {
                return (Parameters)serializer.Deserialize(reader);
            }
        }

        //cancellation of delete survey operation can't be supported, so we just ignore CancellationToken parameter
        public AsyncOperationResult Execute(BvAsyncOperationQueueEntity entity, string serializedParameters, IAsyncOperationProgressLogger progressLogger, CancellationToken _)
        {
            var sw = Stopwatch.StartNew();

            var parameters = DeserializeParameters(serializedParameters);

            parameters.UnassignedDdiNumbers = GetLogInformationAboutAssignedDdiNumbers(parameters.SurveyId);

            var evt = new DeleteSurveyEvent(parameters.SurveyId, parameters.ProjectId, parameters, entity);
            
            var result = new AsyncOperationResult { ProcessedItemsCount = 1, State = AsyncOperationState.Completed };

            var survey = _surveyRepository.GetById(parameters.SurveyId);

            using (new EventDetailsScope(evt.Details))
            {
                progressLogger.AppendText(entity.Id, "Shutting down survey...", sw.Elapsed, true);
                // survey should be closed and its tasks removed before deletion
                _surveyStateService.ShutdownSurvey(parameters.SurveyId);
                
                progressLogger.AppendText(entity.Id, "Deleting interview records and scheduled calls...", sw.Elapsed, true);
                _surveyService.CleanSurvey(parameters.SurveyId, CancellationToken.None);
                
                progressLogger.AppendText(entity.Id, "Deleting survey from CATI...", sw.Elapsed, true);
                using (var dbTransactionScope = new DatabaseTransactionScope("Async.DeleteSurvey"))
                {
                    // Remove replicated data.
                    _replicationSchemaService.UpdateSurveyReplicationScheme(parameters.SurveyId, null);

                    _surveyPublishService.OnDeleteSurvey(parameters.SurveyId);

                    _surveyRepository.Delete(parameters.SurveyId);

                    dbTransactionScope.Commit();
                }
            }
            
            evt.Finish();

            if (survey.State != (int)SurveyState.SoftDeleted)
            {
                Trace.TraceWarning("Survey '{0}' in {1} state was permanently deleted while hard-delete operation.", parameters.ProjectId, ((SurveyState)survey.State).ToString());
            }

            return result;
        }

        private string GetLogInformationAboutAssignedDdiNumbers(int surveyId)
        {
            var ddiNumbers = _inboundTelephoneNumberRepository.GetBySurveyId(surveyId);

            if (ddiNumbers.Count == 0)
            {
                return string.Empty;
            }

            var message = new StringBuilder("\r\nThe following DDI numbers will be unassigned from the survey:\r\n");
            foreach (var ddiNumber in ddiNumbers)
            {
                message.AppendLine(ddiNumber.TelephoneNumber);
            }

            return message.ToString();
        }
    }
}
