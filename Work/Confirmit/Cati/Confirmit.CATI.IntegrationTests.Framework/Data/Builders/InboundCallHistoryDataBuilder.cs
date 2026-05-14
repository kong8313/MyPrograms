using System;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

namespace Confirmit.CATI.IntegrationTests.Framework.Data.Builders
{
    public class InboundCallHistoryDataBuilder : BaseObjectBuilder<InboundCallHistoryData>
    {
        public InboundCallHistoryDataBuilder(TestDataContext context, InboundCallHistoryData data, DataGenerator dataGenerator) : base(context, data, dataGenerator)
        {
        }

        public override void Create()
        {
            var model = new BvInboundCallsHistoryEntity
            {
                InboundTelNumber = Data.InboundTelNumber,
                OperationType = Data.OperationType,
                RespondentTelNumber = Data.RespondentTelNumber,
                InboundCallId = Data.InboundCallId,
                InterviewId = Data.InterviewId,
                SurveyId = Data.SurveyId,
                FiredTime = DateTime.UtcNow
            };

            ServiceLocator.Resolve<IInboundCallsHistoryRepository>().Insert(model);

            Context.InboundCallHistories.Add(new InboundCallHistoryController(Data.Tag, model.Id, Context, Data));
        }
    }
}