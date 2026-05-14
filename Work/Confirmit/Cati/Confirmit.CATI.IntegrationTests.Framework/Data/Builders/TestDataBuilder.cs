using System.Collections.Generic;
using System.Linq;
using BvCallHandlerLibrary;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

namespace Confirmit.CATI.IntegrationTests.Framework.Data.Builders
{
    public class TestDataBuilder
    {
        public TestDataContext Create(BaseTestData data)
        {
            var context = new TestDataContext();

            var objectBuilders = new List<IObjectBuilder>();
            var dataGenerator = new DataGenerator();

            objectBuilders.Add(new SystemSettingsBuilder(context, data.SystemSettings, dataGenerator));
            objectBuilders.AddRange(data.CallCenters.Select(x => new CallCenterDataBuilder(context, x, dataGenerator)));
            objectBuilders.AddRange(data.InboundCallHistories.Select(x => new InboundCallHistoryDataBuilder(context, x, dataGenerator)));
            objectBuilders.AddRange(data.Alerts.Select(x => new AlertDataBuilder(context, x, dataGenerator)));
            objectBuilders.AddRange(data.Dialers.Select(x => new DialerDataBuilder(context, x, dataGenerator)));
            objectBuilders.AddRange(data.CallGroups.Select(x => new CallGroupDataBuilder(context, x, dataGenerator)));
            objectBuilders.AddRange(data.Persons.Select(x => new PersonDataBuilder(context, x, dataGenerator)));
            objectBuilders.AddRange(data.PersonGroups.Select(x => new PersonGroupDataBuilder(context, x, dataGenerator)));
            objectBuilders.AddRange(data.Surveys.Select(x => new SurveyDataBuilder(context, x, dataGenerator)));
            objectBuilders.AddRange(data.Scripts.Select(x => new ScriptDataBuilder(context, x, dataGenerator)));
            objectBuilders.AddRange(data.Supervisors.Select(x => new SupervisorDataBuilder(context, x, dataGenerator)));
            objectBuilders.AddRange(data.Filters.Select(x => new FilterDataBuilder(context, x, dataGenerator)));
            objectBuilders.Add(new TelephoneBlackListBuilder(context, data.TelephoneBlacklist, dataGenerator));
            objectBuilders.AddRange(data.ExternalNumbers.Select(x => new ExternalNumberBuilder(context, x, dataGenerator)));
            objectBuilders.Add(new StateDataBuilder(context, data.StateData, dataGenerator));

            objectBuilders.ForEach(x => x.Create());

            ServiceLocator.Resolve<IBvCallHandlerRoot>().OnStartup();

            objectBuilders.ForEach(x => x.Setup());

            return context;
        }
    }
}