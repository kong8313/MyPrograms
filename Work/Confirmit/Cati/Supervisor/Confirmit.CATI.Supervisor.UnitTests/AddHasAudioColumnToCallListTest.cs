using System.Data;
using System.Linq;
using Confirmit.CATI.Core.SupervisorService.Fakes;
using Confirmit.CATI.Supervisor.Classes.CallManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Supervisor.UnitTests
{
    [TestClass]
    public class AddHasAudioColumnToCallListTest
    {
        private IAddHasAudioColumnToCallList _addColumn;
        private DataTable _callList;

        [TestInitialize]
        public void TestInitialize()
        {
            var stubISupervisorServiceClient = new StubISupervisorServiceClient
            {
                AreRecordsExistsInt32ArrayOfInt32 = (sid, ids) => new[] { false, true, false }
            };

            _addColumn = new AddHasAudioColumnToCallList(stubISupervisorServiceClient);

            ConstructCallList();
        }

        private void ConstructCallList()
        {
            _callList = new DataTable();
            _callList.Columns.Add(new DataColumn("InterviewID", typeof (int)));

            _callList.Rows.Add(123);
            _callList.Rows.Add(43);
            _callList.Rows.Add(34243);
        }

        [TestMethod]
        public void Add_SomeInterviewWithAudio_HasAudioFlagIsSetCorrectly()
        {
            _addColumn.Add(_callList, 0);

            Assert.IsTrue(_callList.Columns.Contains(CallHelper.HasAudioColumnName), "HasAudio column has not been added");
            var row = (from DataRow r in _callList.Rows where (int)r["InterviewID"] == 123 select r).Single();
            Assert.IsFalse((bool)row[CallHelper.HasAudioColumnName]);
            row = (from DataRow r in _callList.Rows where (int)r["InterviewID"] == 43 select r).Single();
            Assert.IsTrue((bool)row[CallHelper.HasAudioColumnName]);
            row = (from DataRow r in _callList.Rows where (int)r["InterviewID"] == 34243 select r).Single();
            Assert.IsFalse((bool)row[CallHelper.HasAudioColumnName]);
        }
    }
}
