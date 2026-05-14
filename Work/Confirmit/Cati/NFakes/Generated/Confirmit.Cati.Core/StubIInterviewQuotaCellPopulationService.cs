using System;
using System.Data;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubIInterviewQuotaCellPopulationService : IInterviewQuotaCellService 
    {
        private IInterviewQuotaCellService _inner;

        public StubIInterviewQuotaCellPopulationService()
        {
            _inner = null;
        }

        public IInterviewQuotaCellService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void InsertInt32DataTableDelegate(int surveyId, DataTable interviewsData);
        public InsertInt32DataTableDelegate InsertInt32DataTable;

        void IInterviewQuotaCellService.Insert(int surveyId, DataTable interviewsData)
        {

            if (InsertInt32DataTable != null)
            {
                InsertInt32DataTable(surveyId, interviewsData);
            } else if (_inner != null)
            {
                ((IInterviewQuotaCellService)_inner).Insert(surveyId, interviewsData);
            }
        }

        public delegate void DeleteInt32DataTableDelegate(int surveyId, DataTable interviewsData);
        public DeleteInt32DataTableDelegate DeleteInt32DataTable;

        void IInterviewQuotaCellService.Delete(int surveyId, DataTable interviewsData)
        {

            if (DeleteInt32DataTable != null)
            {
                DeleteInt32DataTable(surveyId, interviewsData);
            } else if (_inner != null)
            {
                ((IInterviewQuotaCellService)_inner).Delete(surveyId, interviewsData);
            }
        }

    }
}