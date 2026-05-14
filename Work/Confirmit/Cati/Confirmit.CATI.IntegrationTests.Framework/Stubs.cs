using System;
using BvCallHandlerLibrary.Tools;
using BvCallHandlerLibrary.Tools.Fakes;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Core.Telephony.Fakes;
using Confirmit.CATI.Core.WcfServices.Clients;
using Confirmit.CATI.Core.WcfServices.Clients.Fakes;
using Confirmit.CATI.Telephony;
using Confirmit.CATI.Telephony.Fakes;

namespace Confirmit.CATI.IntegrationTests.Framework
{
    public static class Stubs
    {
        public static void ExtendExistingIDialerApiStub(IDialerAPI inner, StubIDialerAPI stub)
        {
            stub.Inner = inner;

            SetNewIDialerApiStub((dialer) => stub);
        }

        public static void SetNewIDialerApiStub(Func<BvDialersEntity, IDialerAPI> stubFunc)
        {
            var stubIDialerInitializer = new StubIDialerInitializer
            {
                InitializeDialerInt32IDialerAPIBooleanInt32OutStringOutDialTypeOut = (int id, IDialerAPI api, bool b, out int tenantId, out string name, out DialType dialType) =>
                {
                    var dialersEntity = ServiceLocator.Resolve<IDialersRepository>().GetById(id);

                    tenantId = dialersEntity.TenantId;
                    name = dialersEntity.Name;
                    dialType = (DialType) dialersEntity.DialTypeId;
                    return stubFunc(dialersEntity);
                }
            };

            ServiceLocator.RegisterInstance<IDialerInitializer>(stubIDialerInitializer);
        }

        public static void SetNewIDialerApiStub(IDialerAPI stub)
        {
            var stubIDialerInitializer = new StubIDialerInitializer
            {
                InitializeDialerInt32IDialerAPIBooleanInt32OutStringOutDialTypeOut = (int id, IDialerAPI api, bool b, out int tenantId, out string name, out DialType dialType) =>
                {
                    tenantId = 0;
                    name = "";
                    dialType = DialType.Landline;
                    return stub;
                }
            };

            ServiceLocator.RegisterInstance<IDialerInitializer>(stubIDialerInitializer);
        }

        public static void SetNewIAuthoringServiceStub(bool isTelephonyEnabled)
        {
            var stub = new StubIAuthoringService
            {
                GetMaximumCatiInterviewersInt32 = id => 150,
                GetFormInfosStringIEnumerableOfStringSchemaSourceType = (id, names, type) => new FormBase[] { null },
                IsCompanyTelephonyEnabledInt32 = id => isTelephonyEnabled,
                HasCatiAddonInt32 = id => true
            };

            SetNewIAuthoringServiceStub(stub);
        }

        public static void ExtendExistingIAuthoringServiceStub(StubIAuthoringService stub)
        {
            var inner = ServiceLocator.Resolve<IAuthoringService>();

            ExtendExistingIAuthoringServiceStub(inner, stub);
        }

        public static void ExtendExistingIAuthoringServiceStub(IAuthoringService inner, StubIAuthoringService stub)
        {
            stub.Inner = inner;

            SetNewIAuthoringServiceStub(stub);
        }

        public static void SetNewIAuthoringServiceStub(IAuthoringService stub)
        {
            ServiceLocator.RegisterInstance(stub);
        }

        public static void ExtendExistingIMnTciToolsStub(bool doesCompanyUseTelephony)
        {
            var inner = ServiceLocator.Resolve<IMnTciTools>();

            var stub = new StubIMnTciTools
            {
                Inner = inner,
                DoesCompanyUseTelephony = () => doesCompanyUseTelephony
            };

            ExtendExistingIMnTciToolsStub(inner, stub);
        }

        public static void ExtendExistingIMnTciToolsStub(IDialerRecordingAPI dialerRecordingApi)
        {
            var inner = ServiceLocator.Resolve<IMnTciTools>();

            var stub = new StubIMnTciTools
            {
                Inner = inner,
                CreateDialerRecordingInt32 = id => dialerRecordingApi
            };

            ExtendExistingIMnTciToolsStub(inner, stub);
        }

        public static void ExtendExistingIMnTciToolsStub(StubIMnTciTools stub)
        {
            var inner = ServiceLocator.Resolve<IMnTciTools>();
            ExtendExistingIMnTciToolsStub(inner, stub);
        }

        public static void ExtendExistingIMnTciToolsStub(IMnTciTools inner, StubIMnTciTools stub)
        {
            stub.Inner = inner;
            SetNewIMnTciToolsStub(stub);
        }

        public static void SetNewIMnTciToolsStub(IDialerRecordingAPI dialerRecordingApi)
        {
            var stub = new StubIMnTciTools
            {
                CreateDialerRecordingInt32 = id => dialerRecordingApi
            };

            ServiceLocator.RegisterInstance(stub);
        }

        public static void SetNewIMnTciToolsStub(IMnTciTools stub)
        {
            ServiceLocator.RegisterInstance(stub);
        }

        public static void ExtendExistingITelephonyStub(StubITelephony stub)
        {
            var inner = ServiceLocator.Resolve<ITelephony>();
            ExtendExistingITelephonyStub(inner, stub);
        }

        public static void ExtendExistingITelephonyStub(ITelephony inner, StubITelephony stub)
        {
            stub.Inner = inner;
            SetNewITelephonyStub(stub);
        }

        public static void SetNewITelephonyStub(ITelephony stub)
        {
            ServiceLocator.RegisterInstance(stub);
        }
    }
}
