using System;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;

namespace Confirmit.CATI.Core.Services.SampleServiceImplementation
{
    public interface ISampleDataStorage : ISampleRecordStorage, IDisposable
    {
        int BatchID { get; set; }
        int SurveySID { get; set; }
        int OperationId { get; set; }
        void InsertInterview(BvInterviewEntity interview);
        void UpdateInterview(BvInterviewEntity interview);
        void DeleteInterview(int surveySID, int interviewID);
        void InsertCall(BvCallEntity call);
        void UpdateCall(BvCallEntity call);
        void DeleteCall(int surveySID, int interviewID);

        bool IsCallDisabledByFCD { get; set; }
        
        /// <summary>
        /// /This method store data of current record to DataTable objects and reset them
        /// </summary>
        void SaveCurrentRecord();

        /// <summary>
        /// This method commit accumulating data to DB througth BULK INSERT
        /// </summary>
        /// 
        void Commit(IEventDetails eventDetails);
    }
}