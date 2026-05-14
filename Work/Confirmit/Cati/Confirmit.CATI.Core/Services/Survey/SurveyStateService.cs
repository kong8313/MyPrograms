using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using BvCallHandlerLibrary.Tools;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Cache;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using Confirmit.CATI.Core.Telephony;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Services.Survey
{
    public class SurveyStateService : ISurveyStateService
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IDialerCampaignInitializer _dialerCampaignInitializer;
        private readonly IMnTciTools _mnTciTools;
        private readonly ITelephony _telephony;
        private readonly ICallQueueService _callQueueService;
        private readonly IDatabaseLockTimeouts _databaseLockTimeouts;
        private readonly ISqlTableUpdatedPublisher _sqlTableUpdatedPublisher;
        private readonly ITaskRepository _taskRepository;

        public SurveyStateService(
            ISurveyRepository surveyRepository,
            IPersonRepository personRepository,
            IDialerCampaignInitializer dialerCampaignInitializer,
            IMnTciTools mnTciTools,
            ITelephony telephony,
            ICallQueueService callQueueService,
            IDatabaseLockTimeouts databaseLockTimeouts,
            ISqlTableUpdatedPublisher sqlTableUpdatedPublisher, 
            ITaskRepository taskRepository)
        {
            _surveyRepository = surveyRepository;
            _personRepository = personRepository;
            _dialerCampaignInitializer = dialerCampaignInitializer;
            _mnTciTools = mnTciTools;
            _telephony = telephony;
            _callQueueService = callQueueService;
            _databaseLockTimeouts = databaseLockTimeouts;
            _sqlTableUpdatedPublisher = sqlTableUpdatedPublisher;
            _taskRepository = taskRepository;
        }


        public void Open(int sid)
        {
            var surveyEntity = _surveyRepository.GetById(sid);

            var dialerStartCampaignResult = _dialerCampaignInitializer.OpenSurveyOnDialerIfNeeded(surveyEntity.Name, surveyEntity.DialMode);
            if (dialerStartCampaignResult.Count == 0 || dialerStartCampaignResult.Any(x => x.ErrorCode == DialerErrorCode.Success))
            {
                using (var dbLock = DatabaseLockService.CreateLock(
                    DatabaseLockTimeoutsAndRecourceNames.GetOpenOrCloseSurveyRecourceName(sid),
                    "SurveyService.Open",
                    _databaseLockTimeouts.SurveyOperationTimioutInMs))
                {
                    if (!dbLock.TryEnterLock())
                    {
                        return;
                    }

                    using (var transaction = new DatabaseTransactionScope("OpenSurvey", DeadlockPriority.Supervisor))
                    {
                        if (surveyEntity.State == (int)SurveyState.Open)
                        {
                            Trace.TraceWarning("Survey '{0}' already opened", surveyEntity.Name);
                            return;
                        }
                        if (surveyEntity.State == (int)SurveyState.SoftDeleted)
                        {
                            Trace.TraceWarning("Survey '{0}' was deleted", surveyEntity.Name);
                            return;
                        }

                        var evt = new OpenSurveyEvent(surveyEntity.SID, surveyEntity.Name);

                        //
                        // Update the database and fire corresponding event
                        //
                        surveyEntity.State = (int)SurveyState.Open;
                        surveyEntity.LastTouchTime = DateTime.UtcNow;
                        BvSurveyAdapter.Update(surveyEntity);
                        BvSurveyCache.Instance.OnTableChanged();
                        _sqlTableUpdatedPublisher.PublishSurveyUpdated();
                        
                        Trace.TraceInformation("Survey '{0}' successfully opened", surveyEntity.Name);

                        evt.Finish();

                        transaction.Commit();
                    }
                }
                try
                {
                    _callQueueService.Schedule();
                }
                catch (Exception ex)
                {
                    TraceHelper.TraceException(ex,
                        "Scheduling proceedure is failed during open survey.");
                }
            }

            var errorsDescription = String.Empty;
            var dialersInfo = new List<DialerInfo>();
            foreach (var result in dialerStartCampaignResult)
            {
                if (result.ErrorCode == DialerErrorCode.Success)
                    continue;
                errorsDescription += string.Format(" [id: {0}, name: {1}]", result.DialerId, result.DialerName);
                dialersInfo.Add(new DialerInfo() { Id = result.DialerId, Name = result.DialerName, ErrorCode = result.ErrorCode });
            }

            if (!string.IsNullOrEmpty(errorsDescription))
            {
                throw new DialerStartCampaignException(
                    string.Format(
                        "Warning: Survey '{0}' unavailable on dialers: {1}",
                        surveyEntity.Name,
                        errorsDescription), dialersInfo);
            }
        }

        public void ShutdownSurvey(int sid)
        {
            var surveyEntity = _surveyRepository.GetById(sid);

            var evt = new ShutdownSurveyEvent(
                sid,
                surveyEntity.Name);

            using (var dbLock = DatabaseLockService.CreateLock(
                DatabaseLockTimeoutsAndRecourceNames.GetOpenOrCloseSurveyRecourceName(sid),
                "SurveyService.ShutDown",
                _databaseLockTimeouts.SurveyOperationTimioutInMs))
            {
                if (!dbLock.TryEnterLock())
                {
                    return;
                }

                ShutdownSurvey(surveyEntity);

                TerminateNotLockedTasks(surveyEntity);

                KillCampaignOnDialer(surveyEntity);

                TerminateAllTasks(surveyEntity);

            } // var dbLock = DatabaseLockService.CreateLock

            Trace.TraceInformation("Survey '{0}' successfully shutdown", surveyEntity.Name);

            evt.Finish();
        }

        public void CloseSurvey(int sid)
        {
            var surveyEntity = _surveyRepository.GetById(sid);

            var evt = new CloseSurveyEvent(
                sid,
                surveyEntity.Name);

            using (var dbLock = DatabaseLockService.CreateLock(
                DatabaseLockTimeoutsAndRecourceNames.GetOpenOrCloseSurveyRecourceName(sid),
                "SurveyService.Close",
                _databaseLockTimeouts.SurveyOperationTimioutInMs))
            {
                if (!dbLock.TryEnterLock())
                {
                    return;
                }

                using (var transaction = new DatabaseTransactionScope("CloseSurvey", DeadlockPriority.Supervisor))
                {
                    //
                    // Change state only if it is opened because state can be e.g. SoftDeleted
                    //
                    if (surveyEntity.State == (int)SurveyState.Open)
                    {
                        surveyEntity.State = (int)SurveyState.Close;
                        surveyEntity.LastTouchTime = DateTime.UtcNow;
                        BvSurveyAdapter.Update(surveyEntity);
                    }

                    BvSurveyCache.Instance.OnTableChanged();
                    _sqlTableUpdatedPublisher.PublishSurveyUpdated();
                    
                    transaction.Commit();
                }

                var dialingMode = (DialingMode)surveyEntity.DialMode;

                if (dialingMode != DialingMode.Manual)
                {
                    if (_mnTciTools.DoesCompanyUseTelephony())
                    {
                        try
                        {
                            _telephony.StopCampaign(surveyEntity.CampaignId, dialingMode);
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError("SurveyStateService.CloseSurvey: {0} /// " +
                                             "SurveyName={1}, SurveySID={2}, CompanyId={3}, CompanyName={4}",
                                ex,
                                surveyEntity.Name, sid,
                                BackendInstance.Current.CompanyId, BackendInstance.Current.CompanyName);
                        }
                    }
                    else
                    {
                        Trace.TraceWarning("SurveyStateService.CloseSurvey: Dialing mode [{0}] is not 'Manual' but company does not use Telephony /// " +
                                           "SurveyName={1}, SurveySID={2}, CompanyId={3}, CompanyName={4}",
                            dialingMode,
                            surveyEntity.Name, sid,
                            BackendInstance.Current.CompanyId, BackendInstance.Current.CompanyName);
                    }
                }
            }

            Trace.TraceInformation("Survey '{0}' successfully closed", surveyEntity.Name);

            evt.Finish();
        }

        private void KillCampaignOnDialer(BvSurveyEntity surveyEntity)
        {
            var dialingMode = (DialingMode)surveyEntity.DialMode;
            if (dialingMode != DialingMode.Manual)
            {
                if (_mnTciTools.DoesCompanyUseTelephony())
                {
                    try
                    {
                        _telephony.KillCampaign(surveyEntity.CampaignId, dialingMode);
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError("SurveyStateService.ShutdownSurvey: {0} /// " +
                                         "SurveyName={1}, SurveySID={2}, CompanyId={3}, CompanyName={4}",
                            ex,
                            surveyEntity.Name, surveyEntity.SID,
                            BackendInstance.Current.CompanyId, BackendInstance.Current.CompanyName);
                    }
                }
                else
                {
                    Trace.TraceWarning(
                        "SurveyStateService.ShutdownSurvey: Dialing mode [{0}] is not 'Manual' but company does not use Telephony /// " +
                        "SurveyName={1}, SurveySID={2}, CompanyId={3}, CompanyName={4}",
                        dialingMode,
                        surveyEntity.Name, surveyEntity.SID,
                        BackendInstance.Current.CompanyId, BackendInstance.Current.CompanyName);
                }
            }
        }

        private void ShutdownSurvey(BvSurveyEntity surveyEntity)
        {
            using (var transaction = new DatabaseTransactionScope("ShutdownSurvey", DeadlockPriority.Supervisor))
            {
                //
                // Change state only if it is opened because state can be e.g. SoftDeleted
                //
                if (surveyEntity.State == (int)SurveyState.Open)
                {
                    surveyEntity.State = (int)SurveyState.Close;
                    surveyEntity.LastTouchTime = DateTime.UtcNow;
                    BvSurveyAdapter.Update(surveyEntity);
                }

                BvSpSurvey_ShutdownAdapter.ExecuteNonQuery(surveyEntity.SID);

                BvSurveyCache.Instance.OnTableChanged();
                _sqlTableUpdatedPublisher.PublishSurveyUpdated();

                transaction.Commit();
            }
        }

        private void TerminateAllTasks(BvSurveyEntity surveyEntity)
        {
            var stopWatch = Stopwatch.StartNew();
            while(TaskService.IsSurveyHasTasks(surveyEntity.SID) && stopWatch.Elapsed < TimeSpan.FromSeconds(1))
            {
                TerminateNotLockedTasks(surveyEntity);
                Thread.Sleep(500);
            }
        }
        
        private void TerminateNotLockedTasks(BvSurveyEntity surveyEntity)
        {
            var tasks = _taskRepository.GetBySurveyNotLocked(surveyEntity.SID);

            foreach (var task in tasks)
            {
                try
                {
                    TaskService.TerminateTask(
                        task.PersonSID,
                        new DatabaseTransactionOptions("ShutdownSurvey", DeadlockPriority.Supervisor));
                }
                catch (Exception e)
                {
                    var person = _personRepository.GetById(task.PersonSID);

                    TraceHelper.TraceException(
                        e,
                        string.Format(
                            "SurveyStateService.ShutDown failed during terminating task on survey '{0}' for the person '{1}'({2}).",
                            surveyEntity.Name,
                            person.Name,
                            person.SID));
                }
            } // foreach (var task in tasks)
        }
    }
}