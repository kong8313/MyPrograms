using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIInterviewQuotaCellRepository : IInterviewQuotaCellRepository 
    {
        private IInterviewQuotaCellRepository _inner;

        public StubIInterviewQuotaCellRepository()
        {
            _inner = null;
        }

        public IInterviewQuotaCellRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void InsertListOfBvInterviewQuotaCellEntityDelegate(List<BvInterviewQuotaCellEntity> cells);
        public InsertListOfBvInterviewQuotaCellEntityDelegate InsertListOfBvInterviewQuotaCellEntity;

        void IInterviewQuotaCellRepository.Insert(List<BvInterviewQuotaCellEntity> cells)
        {

            if (InsertListOfBvInterviewQuotaCellEntity != null)
            {
                InsertListOfBvInterviewQuotaCellEntity(cells);
            } else if (_inner != null)
            {
                ((IInterviewQuotaCellRepository)_inner).Insert(cells);
            }
        }

        public delegate void DeleteInt32ListOfInt32Delegate(int surveyId, List<int> interviewIds);
        public DeleteInt32ListOfInt32Delegate DeleteInt32ListOfInt32;

        void IInterviewQuotaCellRepository.Delete(int surveyId, List<int> interviewIds)
        {

            if (DeleteInt32ListOfInt32 != null)
            {
                DeleteInt32ListOfInt32(surveyId, interviewIds);
            } else if (_inner != null)
            {
                ((IInterviewQuotaCellRepository)_inner).Delete(surveyId, interviewIds);
            }
        }

        public delegate void DeleteInt32Delegate(int surveyId);
        public DeleteInt32Delegate DeleteInt32;

        void IInterviewQuotaCellRepository.Delete(int surveyId)
        {

            if (DeleteInt32 != null)
            {
                DeleteInt32(surveyId);
            } else if (_inner != null)
            {
                ((IInterviewQuotaCellRepository)_inner).Delete(surveyId);
            }
        }

        public delegate List<BvInterviewQuotaCellEntity> GetByInterviewIdInt32Int32Delegate(int surveyId, int interviewId);
        public GetByInterviewIdInt32Int32Delegate GetByInterviewIdInt32Int32;

        List<BvInterviewQuotaCellEntity> IInterviewQuotaCellRepository.GetByInterviewId(int surveyId, int interviewId)
        {


            if (GetByInterviewIdInt32Int32 != null)
            {
                return GetByInterviewIdInt32Int32(surveyId, interviewId);
            } else if (_inner != null)
            {
                return ((IInterviewQuotaCellRepository)_inner).GetByInterviewId(surveyId, interviewId);
            }

            return default(List<BvInterviewQuotaCellEntity>);
        }

    }
}