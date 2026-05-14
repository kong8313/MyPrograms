using System;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using System.Data;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.ReplicationServiceImplementation.Fakes
{
    public class StubIReplicatedDataRepository : IReplicatedDataRepository 
    {
        private IReplicatedDataRepository _inner;

        public StubIReplicatedDataRepository()
        {
            _inner = null;
        }

        public IReplicatedDataRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IDataReader ExecuteReplicatedDataReaderInt32Delegate(int surveyId);
        public ExecuteReplicatedDataReaderInt32Delegate ExecuteReplicatedDataReaderInt32;

        IDataReader IReplicatedDataRepository.ExecuteReplicatedDataReader(int surveyId)
        {


            if (ExecuteReplicatedDataReaderInt32 != null)
            {
                return ExecuteReplicatedDataReaderInt32(surveyId);
            } else if (_inner != null)
            {
                return ((IReplicatedDataRepository)_inner).ExecuteReplicatedDataReader(surveyId);
            }

            return default(IDataReader);
        }

        public delegate DataTable GetInterviewsDataInt32ListOfInt32Delegate(int surveyId, List<int> interviewsIds);
        public GetInterviewsDataInt32ListOfInt32Delegate GetInterviewsDataInt32ListOfInt32;

        DataTable IReplicatedDataRepository.GetInterviewsData(int surveyId, List<int> interviewsIds)
        {


            if (GetInterviewsDataInt32ListOfInt32 != null)
            {
                return GetInterviewsDataInt32ListOfInt32(surveyId, interviewsIds);
            } else if (_inner != null)
            {
                return ((IReplicatedDataRepository)_inner).GetInterviewsData(surveyId, interviewsIds);
            }

            return default(DataTable);
        }

        public delegate IDictionary<string, string> GetReplicationValuesInt32Int32Delegate(int surveyId, int interviewId);
        public GetReplicationValuesInt32Int32Delegate GetReplicationValuesInt32Int32;

        IDictionary<string, string> IReplicatedDataRepository.GetReplicationValues(int surveyId, int interviewId)
        {


            if (GetReplicationValuesInt32Int32 != null)
            {
                return GetReplicationValuesInt32Int32(surveyId, interviewId);
            } else if (_inner != null)
            {
                return ((IReplicatedDataRepository)_inner).GetReplicationValues(surveyId, interviewId);
            }

            return default(IDictionary<string, string>);
        }

    }
}