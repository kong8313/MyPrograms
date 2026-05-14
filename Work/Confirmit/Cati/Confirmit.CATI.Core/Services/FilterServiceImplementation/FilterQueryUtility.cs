namespace Confirmit.CATI.Core.Services.FilterServiceImplementation
{
    static class FilterQueryUtility
    {
        private const string DefaultJoinOnShiftTypeTable = @"LEFT JOIN BvShiftZones ON BvShiftZones.[ID] = BvCall.ShiftTypeID  
                                   LEFT JOIN BvShiftType ON  BvShiftType.ObjectID = BvShiftZones.ShiftTypeID";

        private const string DefaultJoinOnAppointmentTable = @"LEFT JOIN BvAppointment ON BvCall.ApptID = BvAppointment.ID";

        private const string DefaultJoinOnResourceTable = @"LEFT JOIN BvViewPersonAndGroup ON  BvViewPersonAndGroup.SID = BvCall.ExplicitSID";

        private const string DefaultJoinOnCFVariablesTable = @"LEFT JOIN {0} CFinterview ON CFinterview.respid = BvCall.InterviewID";

        private const string DefaultJoinOnInterviewTable = @"INNER JOIN BvInterview ON BvCall.InterviewID = BvInterview.[ID] 
                                    AND BvCall.SurveySID = BvInterview.SurveySID";

        public static string GetDefaultJoinOnShiftTypeTable()
        {
            return DefaultJoinOnShiftTypeTable;
        }

        public static string GetDefaultJoinOnAppointmentTable()
        {
            return DefaultJoinOnAppointmentTable;
        }

        public static string GetDefaultJoinOnResourceTable()
        {
            return DefaultJoinOnResourceTable;
        }

        public static string GetDefaultJoinOnCFVariablesTable(string tableName)
        {
            return string.Format(DefaultJoinOnCFVariablesTable, tableName);
        }

        public static string GetDefaultJoinOnInterviewTable()
        {
            return DefaultJoinOnInterviewTable;
        }
    }
}