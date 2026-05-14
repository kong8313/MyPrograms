using System.Text;
using Confirmit.CATI.Core.Services.Survey;

namespace Confirmit.CATI.Core.Services.SampleServiceImplementation
{
    /// <summary>
    /// Holds some state related to add sample operation. E.g. keeps data related to
    /// results of interviewer assignments, etc made during sample addition.
    /// </summary>
    public class SampleProcessingStateContainer
    {
        private readonly int m_SurveyId;
        private readonly int m_BatchId;
        private int m_RecordsWithInvalidExtendedStatus;
        private int m_RecordsWithInvalidTimeToCall;
        private int m_RecordsWithInvalidTimeToExpire;
        private int m_RecordsWithInvalidCallProirity;
        private int m_RecordsWithInvalidShiftType;
        private int m_RecordsWithInvalidCallState;
        private int m_RecordsWithInvalidResource;
        private int m_RecordsWithInvalidResources;

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleProcessingStateContainer"/> class.
        /// </summary>
        public SampleProcessingStateContainer(int surveyId, int batchId)
        {
            m_SurveyId = surveyId;
            m_BatchId = batchId;
        }

        /// <summary>
        /// Stores the result of a single 'assign resource' operation.
        /// </summary>
        /// <param name="result">The operation result.</param>
        public void AddAssignResourceResult(SampleRecordOperationType result)
        {
            if (result == SampleRecordOperationType.Incorrect)
            {
                m_RecordsWithInvalidResource++;
            }
        }

        /// <summary>
        /// Stores the result of a single 'assign resources' operation.
        /// </summary>
        /// <param name="result">The operation result.</param>
        public void AddAssingResourcesResult(SampleRecordOperationType result)
        {
            if (result == SampleRecordOperationType.Incorrect)
            {
                m_RecordsWithInvalidResources++;
            }
        }

        /// <summary>
        /// Stores the result of a single 'set extended status' operation.
        /// </summary>
        /// <param name="result">The operation result.</param>
        public void AddSetExtendedStatusResult(SampleRecordOperationType result)
        {
            if (result == SampleRecordOperationType.Incorrect)
            {
                m_RecordsWithInvalidExtendedStatus++;
            }
        }

        /// <summary>
        /// Stores the result of a single 'set time to call' operation.
        /// </summary>
        /// <param name="result">The operation result.</param>
        public void AddSetTimeToCallResult(SampleRecordOperationType result)
        {
            if (result == SampleRecordOperationType.Incorrect)
            {
                m_RecordsWithInvalidTimeToCall++;
            }
        }

        /// <summary>
        /// Stores the result of a single 'set expiration time to call' operation.
        /// </summary>
        /// <param name="result">The operation result.</param>
        public void AddSetTimeToExpireResult(SampleRecordOperationType result)
        {
            if (result == SampleRecordOperationType.Incorrect)
            {
                m_RecordsWithInvalidTimeToExpire++;
            }
        }
        
        public void AddSetCallPriorityResult(SampleRecordOperationType result)
        {
            if (result == SampleRecordOperationType.Incorrect)
            {
                m_RecordsWithInvalidCallProirity++;
            }
        }
        
        public void AddSetShiftTypeResult(SampleRecordOperationType result)
        {
            if (result == SampleRecordOperationType.Incorrect)
            {
                m_RecordsWithInvalidShiftType++;
            }
        }
        
        public void AddSetCallStateResult(SampleRecordOperationType result)
        {
            if (result == SampleRecordOperationType.Incorrect)
            {
                m_RecordsWithInvalidCallState++;
            }
        }

        /// <summary>
        /// Determines whether there were some invalid values in the sample records. 
        /// </summary>
        /// <returns>Boolean value indicating whether there were some invalid values in the sample records.</returns>
        public bool AreInvalidRecordsFound()
        {
            return m_RecordsWithInvalidExtendedStatus > 0 ||
                m_RecordsWithInvalidTimeToCall > 0 ||
                m_RecordsWithInvalidResource > 0 ||
                m_RecordsWithInvalidResources > 0 ||
                m_RecordsWithInvalidTimeToExpire > 0 || 
                m_RecordsWithInvalidCallProirity > 0 || 
                m_RecordsWithInvalidCallState > 0 || 
                m_RecordsWithInvalidShiftType > 0;
        }

        /// <summary>
        /// Gets the formatted warning message with the description of invalid records.
        /// </summary>
        /// <returns>The warning message.</returns>
        public string GetWarningMessage()
        {
            if (AreInvalidRecordsFound() == false)
            {
                return null;
            }

            var text = new StringBuilder();
            text.AppendFormat(
                "During sample addition for survey {0} with batch ID {1} some records had incorrect values in the following columns:",
                SurveyService.GetFormattedSurveyName(m_SurveyId),
                m_BatchId);
            text.AppendLine();

            AppendWarningText(text, "CatiInterviewerId", m_RecordsWithInvalidResource);
            AppendWarningText(text, "CatiAssignments", m_RecordsWithInvalidResources);
            AppendWarningText(text, "CatiExtendedStatus", m_RecordsWithInvalidExtendedStatus);
            AppendWarningText(text, "CatiCallTime", m_RecordsWithInvalidTimeToCall);
            AppendWarningText(text, "CatiCallExpirationTime", m_RecordsWithInvalidTimeToExpire);
            AppendWarningText(text, "CatiCallPriority", m_RecordsWithInvalidCallProirity);
            AppendWarningText(text, "CatiShiftType", m_RecordsWithInvalidShiftType);
            AppendWarningText(text, "CatiCallState", m_RecordsWithInvalidCallState);

            return text.ToString();
        }

        private static void AppendWarningText(StringBuilder text, string columnName, int invalidRecordsCount)
        {
            if (invalidRecordsCount > 0)
            {
                text.AppendFormat("{1} records in the '{0}' column", columnName, invalidRecordsCount);
                text.AppendLine();
            }
        }
    }
}