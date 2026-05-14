using Confirmit.CATI.Common;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DialType = Confirmit.CATI.Common.DialType;

namespace Confirmit.CATI.IntegrationTests.Tests.SampleTest
{
    [TestClass]
    public class SampleWithManualDialingTests : BaseMockedIntegrationTest
    {
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddSampleWithSimpleModel_TwoInterviewWithOneManualDialing_AddSuccessed()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData()
                    {
                        Tag = "S1", IsUseDb = true
                    }
                }
            }.Create();

            var interviews = new[]
            {
                new InterviewData() {Tag = "S1.I1", DialType = DialType.Landline},
                new InterviewData() {Tag = "S1.I2", DialType = DialType.Cellphone},
                new InterviewData() {Tag = "S1.I3", DialType = DialType.Assisted},
            };

            context.GetSurvey("S1").AddSample(SchedulingMode.Simple, interviews);

            Assert.AreEqual((byte)DialType.Landline, context.GetInterview("S1.I1").Model.DialTypeId);
            Assert.AreEqual((byte)DialType.Cellphone, context.GetInterview("S1.I2").Model.DialTypeId);
            Assert.AreEqual((byte)DialType.Assisted, context.GetInterview("S1.I3").Model.DialTypeId);

            Assert.AreEqual((byte)DialType.Landline, context.GetCall("S1.I1").Model.DialTypeId);
            Assert.AreEqual((byte)DialType.Cellphone, context.GetCall("S1.I2").Model.DialTypeId);
            Assert.AreEqual((byte)DialType.Assisted, context.GetCall("S1.I3").Model.DialTypeId);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddSampleWithFullSchedulingModel_TwoInterviewWithOneManualDialing_AddSuccessed()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData()
                    {
                        Tag = "S1", IsUseDb = true, SchedulingScript = AllHoursSchedule.Name,
                    }
                },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var interviews = new[]
            {
                new InterviewData() {Tag = "S1.I1", DialType = DialType.Landline},
                new InterviewData() {Tag = "S1.I2", DialType = DialType.Cellphone},
                new InterviewData() {Tag = "S1.I3", DialType = DialType.Assisted},
            };

            context.GetSurvey("S1").AddSample(SchedulingMode.Full, interviews);


            Assert.AreEqual((byte)DialType.Landline, context.GetInterview("S1.I1").Model.DialTypeId);
            Assert.AreEqual((byte)DialType.Cellphone, context.GetInterview("S1.I2").Model.DialTypeId);
            Assert.AreEqual((byte)DialType.Assisted, context.GetInterview("S1.I3").Model.DialTypeId);

            Assert.AreEqual((byte)DialType.Landline, context.GetCall("S1.I1").Model.DialTypeId);
            Assert.AreEqual((byte)DialType.Cellphone, context.GetCall("S1.I2").Model.DialTypeId);
            Assert.AreEqual((byte)DialType.Assisted, context.GetCall("S1.I3").Model.DialTypeId);
        }
    }
}
