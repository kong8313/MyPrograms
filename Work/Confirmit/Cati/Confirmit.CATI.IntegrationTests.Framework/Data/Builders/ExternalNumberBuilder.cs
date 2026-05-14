using System;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

namespace Confirmit.CATI.IntegrationTests.Framework.Data.Builders
{
    public class ExternalNumberBuilder : BaseObjectBuilder<ExternalNumberData>
    {
        ExternalNumberController _controller;
        public ExternalNumberBuilder(TestDataContext context, ExternalNumberData data, DataGenerator dataGenerator)
            : base(context, data, dataGenerator)
        {
        }

        public override void Create()
        {
            CheckAndInitData();

            var repository = ServiceLocator.Resolve<IExternalTransferTelephoneNumberRepository>();

            var entity = new BvExternalTransferTelephoneNumberEntity()
            {
                TelephoneNumber = Data.Phone,
                Description = Data.Description,
                Hidden = Data.Hidden
            };

            var id = repository.Insert(entity);

            _controller = new ExternalNumberController(Data.Tag, id, Context, Data);

            Context.ExternalNumbers.Add(_controller);
        }

        public override void Setup()
        {
            if (!string.IsNullOrWhiteSpace(Data.Assigns))
            {
                var surveyIds = Context.GetSurveys(Data.Assigns.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries))
                        .Select(x =>x.Id).ToArray();

                ServiceLocator.Resolve<IExternalTransferTelephoneNumberService>()
                        .SetAssignedSurveyIds(_controller.Id, surveyIds);
            }
        }

        private void CheckAndInitData()
        {
            if (Data.Phone == null)
            {
                Data.Phone = DataGenerator.NewId().ToString();
            }

            if (Data.Description == null)
            {
                Data.Description = $"Description for '{Data.Phone}'";
            }
        }
    }
}