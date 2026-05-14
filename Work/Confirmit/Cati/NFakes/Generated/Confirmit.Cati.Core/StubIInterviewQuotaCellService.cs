using System;
using Confirmit.CATI.Core.Services.Survey.Quota;
using System.Data;
using Confirmit.CATI.Core.Services.Interfaces;
using System.Collections.Generic;
using System.Threading;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubIInterviewQuotaCellService : IInterviewQuotaCellService 
    {
        private IInterviewQuotaCellService _inner;

        public StubIInterviewQuotaCellService()
        {
            _inner = null;
        }

        public IInterviewQuotaCellService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void PopulateBatchQuotaMatcherDataTableDelegate(QuotaMatcher quotaMatcher, DataTable interviewsData);
        public PopulateBatchQuotaMatcherDataTableDelegate PopulateBatchQuotaMatcherDataTable;

        void IInterviewQuotaCellService.PopulateBatch(QuotaMatcher quotaMatcher, DataTable interviewsData)
        {

            if (PopulateBatchQuotaMatcherDataTable != null)
            {
                PopulateBatchQuotaMatcherDataTable(quotaMatcher, interviewsData);
            } else if (_inner != null)
            {
                ((IInterviewQuotaCellService)_inner).PopulateBatch(quotaMatcher, interviewsData);
            }
        }

        public delegate void DeleteInt32ListOfInt32Delegate(int surveyId, List<int> interviewIds);
        public DeleteInt32ListOfInt32Delegate DeleteInt32ListOfInt32;

        void IInterviewQuotaCellService.Delete(int surveyId, List<int> interviewIds)
        {

            if (DeleteInt32ListOfInt32 != null)
            {
                DeleteInt32ListOfInt32(surveyId, interviewIds);
            } else if (_inner != null)
            {
                ((IInterviewQuotaCellService)_inner).Delete(surveyId, interviewIds);
            }
        }

        public delegate void PopulateInt32Int32Delegate(int surveyId, int quotaId);
        public PopulateInt32Int32Delegate PopulateInt32Int32;

        void IInterviewQuotaCellService.Populate(int surveyId, int quotaId)
        {

            if (PopulateInt32Int32 != null)
            {
                PopulateInt32Int32(surveyId, quotaId);
            } else if (_inner != null)
            {
                ((IInterviewQuotaCellService)_inner).Populate(surveyId, quotaId);
            }
        }

        public delegate void PopulateInt32CancellationTokenDelegate(int surveyId, CancellationToken cancellationToken);
        public PopulateInt32CancellationTokenDelegate PopulateInt32CancellationToken;

        void IInterviewQuotaCellService.Populate(int surveyId, CancellationToken cancellationToken)
        {

            if (PopulateInt32CancellationToken != null)
            {
                PopulateInt32CancellationToken(surveyId, cancellationToken);
            } else if (_inner != null)
            {
                ((IInterviewQuotaCellService)_inner).Populate(surveyId, cancellationToken);
            }
        }

    }
}