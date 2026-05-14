using System.Linq;
using BvCallHandlerLibrary.Tools.Fakes;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.IntegrationTests.Framework.Controllers;
using Confirmit.CATI.IntegrationTests.Framework.Dialer;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Newtonsoft.Json;

namespace Confirmit.CATI.IntegrationTests.Framework.Data.Builders
{
    public class DialerDataBuilder : BaseObjectBuilder<DialerData>
    {
        public DialerDataBuilder(
            TestDataContext context,
            DialerData data,
            DataGenerator dataGenerator)
            : base(context, data, dataGenerator)
        {
        }

        public override void Create()
        {
            CheckAndInitData();

            var dialerHelper = new DialerBehaviorController(new TestDialer(), Context);
            dialerHelper.SetFakeDialerId((int)Data.Id);

            var stubMnTciTools = new StubIMnTciTools
            {
                CreateDialerRecordingInt32 = id => null,
            };

            Stubs.ExtendExistingIMnTciToolsStub(stubMnTciTools);
            Stubs.SetNewIDialerApiStub((d) => Context.Dialers.Single(x => x.Id == d.Id).DialerHelper.FakeDialer);

            var dialer = IntegrationTestingFramework.CreateAndSetupDialer(Data.Id ?? 0, Data.Type, Data.Name);

            dialer.TenantId = Data.TenantId;
            dialer.DialerOperationalStateNotification = true;
            dialer.IsActive = Data.IsActive;
            dialer.DialTypeId = (byte)Data.DialType;
            dialer.ReconnectionDuration = Data.ReconnectionDuration;
            dialer.ExpectedState = Data.ExpectedState;
            if (Data.Features != null)
            {
                dialer.Features = JsonConvert.SerializeObject(Data.Features);
            }

            BvDialersAdapter.Update(dialer);

            var controller = new DialerController(Context, Data.Tag, dialer.Id, dialerHelper);

            dialerHelper.Methods.StartCampaign.Init();
            dialerHelper.Methods.Login.Init(DialerMethodBehaviors.SendLoggedAgentState);
            dialerHelper.Methods.SetGroups.Init();
            dialerHelper.Methods.GoReady.Init();
            dialerHelper.SetAutoResponseOnFlushNumbers(calls => controller.FlushedCalls.AddRange(calls));
            dialerHelper.Methods.SendNumberToAgent.Init(DialerMethodBehaviors.SendOutcomeConnected);
            dialerHelper.Methods.IvrRenderVoiceXml.Init(DialerMethodBehaviors.SendIvrSubmit);
            dialerHelper.Methods.CompleteCall.Init();
            dialerHelper.Methods.TransferStart.Init();
            dialerHelper.Methods.TransferSetTarget.Init();
            dialerHelper.Methods.TransferSetConnectionState.Init();
            dialerHelper.Methods.TransferCancel.Init();
            dialerHelper.Methods.TransferComplete.Init();
            dialerHelper.Methods.CompletePreview.Init(DialerMethodBehaviors.SendOutcomeConnected);
            dialerHelper.NotificationReplyType = Data.ReplyType;
            dialerHelper.Methods.GetDialerVersion.Init(Data.DialerVersion);
            dialerHelper.Methods.GetFeatures.Init(Data.Features);

            Context.Dialers.Add(controller);
        }

        public override void Setup()
        {
            var dialer = ServiceLocator.Resolve<DialersRepository>().GetById((int)Data.Id);
            dialer.DialerOperationalStateNotification = Data.IsConnected;
            BvDialersAdapter.Update(dialer);
        }

        private void CheckAndInitData()
        {
            if (Data.Id == null)
            {
                Data.Id = DataGenerator.NewId();
            }

            if (Data.Name == null)
            {
                Data.Name = DataGenerator.NewName(Data.Tag ?? "Dialer name");
            }
        }
    }
}
