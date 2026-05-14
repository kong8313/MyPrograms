using System.Data;
using System.Linq;
using Confirmit.CATI.Core.SupervisorService;

namespace Confirmit.CATI.Supervisor.Classes.CallManagement
{
    internal class AddHasAudioColumnToCallList : IAddHasAudioColumnToCallList
    {
        private readonly ISupervisorServiceClient _supervisorServiceClient;

        public AddHasAudioColumnToCallList(ISupervisorServiceClient supervisorServiceClient)
        {
            _supervisorServiceClient = supervisorServiceClient;
        }

        public void Add(DataTable list, int surveySid)
        {
            list.Columns.Add(new DataColumn(CallHelper.HasAudioColumnName, typeof(bool)));

            const string interviewIdColumnName = "InterviewID";

            var interviewIds = list.Select().Select(row => (int)row[interviewIdColumnName]).ToArray();
            var interviewIdToIndexMap = Enumerable
                .Range(0, interviewIds.Length).ToDictionary(index => interviewIds[index], index => index);

            bool[] hasAudioFlags = _supervisorServiceClient.AreRecordsExists(surveySid, interviewIds);

            for (var i = 0; i < list.Rows.Count; i++)
            {
                var answerIndex = interviewIdToIndexMap[(int)list.Rows[i][interviewIdColumnName]];
                list.Rows[i][CallHelper.HasAudioColumnName] = hasAudioFlags[answerIndex];
            }
        }
    }
}