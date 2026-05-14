using System.Linq;
using System.Data.SqlClient;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;

namespace Confirmit.CATI.Core.Repositories
{
    public static class AppointmentRepository
    {
        public static int InsertUpdate([NotNull] BvAppointmentEntity appointment)
        {
            int newSid;

            BvSpAppointmentUpdateAdapter.ExecuteNonQuery(
                appointment.ID,
                appointment.SurveySID,
                appointment.InterviewSID,
                appointment.Time,
                appointment.ExpTime,
                appointment.ContactName,
                appointment.State,
                appointment.TZID,
                out newSid);

            return newSid;
        }

        [CanBeNull]
        public static BvAppointmentEntity GetById(int surveySID, int interviewID)
        {
            var result = BvAppointmentAdapter.GetByCondition("SurveySID = @SurveySID AND InterviewSID = @InterviewID",
                    new SqlParameter("@SurveySID", surveySID),
                    new SqlParameter("@InterviewID", interviewID));

            return result.FirstOrDefault();
        }

        public static void DeleteBatch(int surveySID, int batchID)
        {
            BvAppointmentAdapter.DeleteByCondition(@"
                            SurveySID = @SurveySID AND 
                            InterviewSID IN (SELECT ItemID FROM BvTransferArrays WHERE BatchID = @BatchID )",
                    new SqlParameter("@SurveySID", surveySID),
                    new SqlParameter("@BatchID", batchID));
        }

        /// <summary>
        /// Returns newly created appointment.
        /// Returns null if no such appointment available.
        /// </summary>
        /// <param name="surveySID">Survey ID</param>
        /// <param name="interviewID">Interview ID</param>
        [CanBeNull]
        public static BvAppointmentEntity GetNewlyCreatedAppointment(int surveySID, int interviewID)
        {
            return GetAppointmentForInterview(surveySID, interviewID, AppointmentState.ActiveWithoutCall);
        }

        public static BvAppointmentEntity GetAppointmentForInterview(int surveySID, int interviewID, AppointmentState state)
        {
            return BvAppointmentAdapter.GetByCondition(
                    "SurveySID = @SurveySID AND InterviewSID = @InterviewID AND [State] = @State",
                    new SqlParameter("@SurveySID", surveySID),
                    new SqlParameter("@InterviewID", interviewID),
                    new SqlParameter("@State", state))
                    .FirstOrDefault();
        }

    }
}