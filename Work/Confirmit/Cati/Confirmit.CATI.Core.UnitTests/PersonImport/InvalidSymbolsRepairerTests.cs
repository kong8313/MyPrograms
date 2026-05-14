using Confirmit.CATI.Common.Validators;
using Confirmit.CATI.Core.Services.PersonImport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.PersonImport
{
    [TestClass]
    public class InvalidSymbolsRepairerTests
    {        
        private readonly IInvalidSymbolsRepairer _invalidSymbolsRepairer  = new InvalidSymbolsRepairer(new InputParameterValidator());

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void ParceStationId_StationIdIsParcedCorrectly()
        {
            var assignmentData = new AssignmentData
                {
                    GroupName = "Group'Name",
                    PersonName = "Interviewer'Name",
                    PersonDescription = "Interviewer'Description",
                    PersonLocation = "Interviewer'Location"
                };

            var r = _invalidSymbolsRepairer.Repair(assignmentData, new ImportResult());

            Assert.AreEqual("Group Name", r.GroupName);
            Assert.AreEqual("Interviewer Name", r.PersonName);
            Assert.AreEqual("Interviewer Description", r.PersonDescription);
            Assert.AreEqual("Interviewer Location", r.PersonLocation);            
        }       
    }
}
