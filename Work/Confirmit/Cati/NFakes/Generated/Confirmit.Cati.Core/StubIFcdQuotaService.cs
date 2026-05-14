using System;
using Confirmit.CATI.Core.Services.Interfaces;
using System.Collections.Generic;
using Confirmit.CATI.Core.ManagementService;
using System.Threading;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubIFcdQuotaService : IFcdQuotaService 
    {
        private IFcdQuotaService _inner;

        public StubIFcdQuotaService()
        {
            _inner = null;
        }

        public IFcdQuotaService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void OnQuotaCellChangedInt32Int32Int32QuotaCellStateDelegate(int surveyId, int quotaId, int cellId, QuotaCellState state);
        public OnQuotaCellChangedInt32Int32Int32QuotaCellStateDelegate OnQuotaCellChangedInt32Int32Int32QuotaCellState;

        void IFcdQuotaService.OnQuotaCellChanged(int surveyId, int quotaId, int cellId, QuotaCellState state)
        {

            if (OnQuotaCellChangedInt32Int32Int32QuotaCellState != null)
            {
                OnQuotaCellChangedInt32Int32Int32QuotaCellState(surveyId, quotaId, cellId, state);
            } else if (_inner != null)
            {
                ((IFcdQuotaService)_inner).OnQuotaCellChanged(surveyId, quotaId, cellId, state);
            }
        }

        public delegate void OnQuotaUpdateInt32Int32Delegate(int surveyId, int quotaId);
        public OnQuotaUpdateInt32Int32Delegate OnQuotaUpdateInt32Int32;

        void IFcdQuotaService.OnQuotaUpdate(int surveyId, int quotaId)
        {

            if (OnQuotaUpdateInt32Int32 != null)
            {
                OnQuotaUpdateInt32Int32(surveyId, quotaId);
            } else if (_inner != null)
            {
                ((IFcdQuotaService)_inner).OnQuotaUpdate(surveyId, quotaId);
            }
        }

        public delegate void OnQuotaCellsChangedInt32Int32ArrayOfInt32ArrayOfInt32ArrayOfInt32Delegate(int surveySid, int quotaSid, int[] openedCfCellIds, int[] closedCfCellIds, int[] optimisticallyClosedCfCellIds);
        public OnQuotaCellsChangedInt32Int32ArrayOfInt32ArrayOfInt32ArrayOfInt32Delegate OnQuotaCellsChangedInt32Int32ArrayOfInt32ArrayOfInt32ArrayOfInt32;

        void IFcdQuotaService.OnQuotaCellsChanged(int surveySid, int quotaSid, int[] openedCfCellIds, int[] closedCfCellIds, int[] optimisticallyClosedCfCellIds)
        {

            if (OnQuotaCellsChangedInt32Int32ArrayOfInt32ArrayOfInt32ArrayOfInt32 != null)
            {
                OnQuotaCellsChangedInt32Int32ArrayOfInt32ArrayOfInt32ArrayOfInt32(surveySid, quotaSid, openedCfCellIds, closedCfCellIds, optimisticallyClosedCfCellIds);
            } else if (_inner != null)
            {
                ((IFcdQuotaService)_inner).OnQuotaCellsChanged(surveySid, quotaSid, openedCfCellIds, closedCfCellIds, optimisticallyClosedCfCellIds);
            }
        }

        public delegate void OnQuotaCellsStateChangedInt32Int32ListOfCatiQuotaCellCountersStateDelegate(int surveySid, int quotaSid, List<CatiQuotaCellCountersState> quotaCellsCountersStates);
        public OnQuotaCellsStateChangedInt32Int32ListOfCatiQuotaCellCountersStateDelegate OnQuotaCellsStateChangedInt32Int32ListOfCatiQuotaCellCountersState;

        void IFcdQuotaService.OnQuotaCellsStateChanged(int surveySid, int quotaSid, List<CatiQuotaCellCountersState> quotaCellsCountersStates)
        {

            if (OnQuotaCellsStateChangedInt32Int32ListOfCatiQuotaCellCountersState != null)
            {
                OnQuotaCellsStateChangedInt32Int32ListOfCatiQuotaCellCountersState(surveySid, quotaSid, quotaCellsCountersStates);
            } else if (_inner != null)
            {
                ((IFcdQuotaService)_inner).OnQuotaCellsStateChanged(surveySid, quotaSid, quotaCellsCountersStates);
            }
        }

        public delegate void OnLaunchSurveyInt32BooleanCancellationTokenDelegate(int surveyId, bool runForceImport, CancellationToken cancellationToken);
        public OnLaunchSurveyInt32BooleanCancellationTokenDelegate OnLaunchSurveyInt32BooleanCancellationToken;

        void IFcdQuotaService.OnLaunchSurvey(int surveyId, bool runForceImport, CancellationToken cancellationToken)
        {

            if (OnLaunchSurveyInt32BooleanCancellationToken != null)
            {
                OnLaunchSurveyInt32BooleanCancellationToken(surveyId, runForceImport, cancellationToken);
            } else if (_inner != null)
            {
                ((IFcdQuotaService)_inner).OnLaunchSurvey(surveyId, runForceImport, cancellationToken);
            }
        }

        public delegate void OnDeleteSurveyInt32Delegate(int surveyId);
        public OnDeleteSurveyInt32Delegate OnDeleteSurveyInt32;

        void IFcdQuotaService.OnDeleteSurvey(int surveyId)
        {

            if (OnDeleteSurveyInt32 != null)
            {
                OnDeleteSurveyInt32(surveyId);
            } else if (_inner != null)
            {
                ((IFcdQuotaService)_inner).OnDeleteSurvey(surveyId);
            }
        }

        public delegate string GetCellInfoInt32Int32Int32Delegate(int surveyId, int quotaId, int cellId);
        public GetCellInfoInt32Int32Int32Delegate GetCellInfoInt32Int32Int32;

        string IFcdQuotaService.GetCellInfo(int surveyId, int quotaId, int cellId)
        {


            if (GetCellInfoInt32Int32Int32 != null)
            {
                return GetCellInfoInt32Int32Int32(surveyId, quotaId, cellId);
            } else if (_inner != null)
            {
                return ((IFcdQuotaService)_inner).GetCellInfo(surveyId, quotaId, cellId);
            }

            return default(string);
        }

    }
}