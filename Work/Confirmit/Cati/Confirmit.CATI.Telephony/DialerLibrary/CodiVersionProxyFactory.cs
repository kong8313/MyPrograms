extern alias CodiV30;
extern alias CodiV32;
extern alias CodiV33;
extern alias CodiV34;
extern alias CodiV35;
extern alias CodiV36;

using System;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Common.WcfTools;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Telephony.DialerService.Contract;
using DialerCommon;
using IDialerService30 = CodiV30::Confirmit.CATI.Telephony.DialerService.Contract.IDialerService;
using IDialerService32 = CodiV32::Confirmit.CATI.Telephony.DialerService.Contract.IDialerService;
using IDialerService33 = CodiV33::Confirmit.CATI.Telephony.DialerService.Contract.IDialerService;
using IDialerService34 = CodiV34::Confirmit.CATI.Telephony.DialerService.Contract.IDialerService;
using IDialerService35 = CodiV35::Confirmit.CATI.Telephony.DialerService.Contract.IDialerService;
using IDialerService36 = CodiV36::Confirmit.CATI.Telephony.DialerService.Contract.IDialerService;

namespace Confirmit.CATI.Telephony.DialerLibrary
{
    public class CodiVersionProxyFactory
    {
        private const string CodiCurrentVersion = "3.7";
        private const string CodiVersion36 = "3.6";
        private const string CodiVersion35 = "3.5";
        private const string CodiVersion34 = "3.4";
        private const string CodiVersion33 = "3.3";
        private const string CodiVersion32 = "3.2";
        private const string CodiVersion30 = "3.0";

        public ICodiVersionCoreProxy Create(
            string codiMajorVersion,
            IChannelFactoryWrapper<IDialerService> dialerChannel,
            string dialerServiceEndpoint,
            string dialerServiceAddress,
            string authorizationKeyForOutgoingRequests,
            CatiCommonILoggerToCodiILogger catiCommonILoggerToCodiILogger)
        {
            if (codiMajorVersion == CodiCurrentVersion)
            {
                return new CodiVersion37CoreProxy(dialerChannel);
            }

            dialerChannel.Release();

            if (codiMajorVersion == CodiVersion36)
            {
                return new CodiVersion36CoreProxy(
                    ConfigureDialerChannelFactory<IDialerService36>(
                        dialerServiceEndpoint,
                        dialerServiceAddress,
                        authorizationKeyForOutgoingRequests,
                        catiCommonILoggerToCodiILogger));
            }


            if (codiMajorVersion == CodiVersion35)
            {
                return new CodiVersion35CoreProxy(
                    ConfigureDialerChannelFactory<IDialerService35>(
                        dialerServiceEndpoint,
                        dialerServiceAddress,
                        authorizationKeyForOutgoingRequests,
                        catiCommonILoggerToCodiILogger));
            }

            if (codiMajorVersion == CodiVersion34)
            {
                return new CodiVersion34CoreProxy(
                    ConfigureDialerChannelFactory<IDialerService34>(
                        dialerServiceEndpoint,
                        dialerServiceAddress,
                        authorizationKeyForOutgoingRequests,
                        catiCommonILoggerToCodiILogger));
            }

            if (codiMajorVersion == CodiVersion33)
            {
                return new CodiVersion33CoreProxy(
                    ConfigureDialerChannelFactory<IDialerService33>(
                        dialerServiceEndpoint,
                        dialerServiceAddress,
                        authorizationKeyForOutgoingRequests,
                        catiCommonILoggerToCodiILogger));
            }

            if (codiMajorVersion == CodiVersion32)
            {
                return new CodiVersion32CoreProxy(
                    ConfigureDialerChannelFactory<IDialerService32>(
                    dialerServiceEndpoint,
                    dialerServiceAddress,
                    authorizationKeyForOutgoingRequests,
                    catiCommonILoggerToCodiILogger));
            }

            if (codiMajorVersion == CodiVersion30)
            {
                return new CodiVersion30CoreProxy(
                    ConfigureDialerChannelFactory<IDialerService30>(
                    dialerServiceEndpoint,
                    dialerServiceAddress,
                    authorizationKeyForOutgoingRequests,
                    catiCommonILoggerToCodiILogger));
            }

            throw new Exception("Unknown CODI version: [" + codiMajorVersion + "]");
        }

        public ICodiVersionRecordingProxy CreateRecordingProxy(
            string codiMajorVersion,
            IChannelFactoryWrapper<IDialerService> dialerChannel,
            string dialerServiceEndpoint,
            string dialerServiceAddress,
            string authorizationKeyForOutgoingRequests,
            CatiCommonILoggerToCodiILogger catiCommonILoggerToCodiILogger)
        {
            if (codiMajorVersion == CodiCurrentVersion)
            {
                return new CodiVersion37RecordingProxy(dialerChannel);
            }

            dialerChannel.Release();

            if (codiMajorVersion == CodiVersion36)
            {
                return new CodiVersion36RecordingProxy(
                    ConfigureDialerChannelFactory<IDialerService36>(
                        dialerServiceEndpoint,
                        dialerServiceAddress,
                        authorizationKeyForOutgoingRequests,
                        catiCommonILoggerToCodiILogger));
            }

            if (codiMajorVersion == CodiVersion35)
            {
                return new CodiVersion35RecordingProxy(
                    ConfigureDialerChannelFactory<IDialerService35>(
                        dialerServiceEndpoint,
                        dialerServiceAddress,
                        authorizationKeyForOutgoingRequests,
                        catiCommonILoggerToCodiILogger));
            }

            if (codiMajorVersion == CodiVersion34)
            {
                return new CodiVersion34RecordingProxy(
                    ConfigureDialerChannelFactory<IDialerService34>(
                        dialerServiceEndpoint,
                        dialerServiceAddress,
                        authorizationKeyForOutgoingRequests,
                        catiCommonILoggerToCodiILogger));
            }

            if (codiMajorVersion == CodiVersion33)
            {
                return new CodiVersion33RecordingProxy(
                    ConfigureDialerChannelFactory<IDialerService33>(
                        dialerServiceEndpoint,
                        dialerServiceAddress,
                        authorizationKeyForOutgoingRequests,
                        catiCommonILoggerToCodiILogger));
            }

            if (codiMajorVersion == CodiVersion32)
            {
                return new CodiVersion32RecordingProxy(
                    ConfigureDialerChannelFactory<IDialerService32>(
                    dialerServiceEndpoint,
                    dialerServiceAddress,
                    authorizationKeyForOutgoingRequests,
                    catiCommonILoggerToCodiILogger));
            }

            if (codiMajorVersion == CodiVersion30)
            {
                return new CodiVersion30RecordingProxy(
                    ConfigureDialerChannelFactory<IDialerService30>(
                    dialerServiceEndpoint,
                    dialerServiceAddress,
                    authorizationKeyForOutgoingRequests,
                    catiCommonILoggerToCodiILogger));
            }

            throw new Exception("Unknown CODI version: [" + codiMajorVersion + "]");
        }

        public ICodiVersionFacilitiesProxy CreateFacilitiesProxy(
            string codiMajorVersion,
            IChannelFactoryWrapper<IDialerService> dialerChannel,
            string dialerServiceEndpoint,
            string dialerServiceAddress,
            string authorizationKeyForOutgoingRequests,
            CatiCommonILoggerToCodiILogger catiCommonILoggerToCodiILogger)
        {
            if (codiMajorVersion == CodiCurrentVersion)
            {
                return new CodiVersion37FacilitiesProxy(dialerChannel);
            }

            dialerChannel.Release();

            if (codiMajorVersion == CodiVersion36)
            {
                return new CodiVersion36FacilitiesProxy(
                    ConfigureDialerChannelFactory<IDialerService36>(
                        dialerServiceEndpoint,
                        dialerServiceAddress,
                        authorizationKeyForOutgoingRequests,
                        catiCommonILoggerToCodiILogger));
            }

            if (codiMajorVersion == CodiVersion35)
            {
                return new CodiVersion35FacilitiesProxy(
                    ConfigureDialerChannelFactory<IDialerService35>(
                        dialerServiceEndpoint,
                        dialerServiceAddress,
                        authorizationKeyForOutgoingRequests,
                        catiCommonILoggerToCodiILogger));
            }

            if (codiMajorVersion == CodiVersion34)
            {
                return new CodiVersion34FacilitiesProxy(
                    ConfigureDialerChannelFactory<IDialerService34>(
                        dialerServiceEndpoint,
                        dialerServiceAddress,
                        authorizationKeyForOutgoingRequests,
                        catiCommonILoggerToCodiILogger));
            }

            if (codiMajorVersion == CodiVersion33)
            {
                return new CodiVersion33FacilitiesProxy(
                    ConfigureDialerChannelFactory<IDialerService33>(
                        dialerServiceEndpoint,
                        dialerServiceAddress,
                        authorizationKeyForOutgoingRequests,
                        catiCommonILoggerToCodiILogger));
            }

            if (codiMajorVersion == CodiVersion32)
            {
                return new CodiVersion32FacilitiesProxy(
                    ConfigureDialerChannelFactory<IDialerService32>(
                    dialerServiceEndpoint,
                    dialerServiceAddress,
                    authorizationKeyForOutgoingRequests,
                    catiCommonILoggerToCodiILogger));
            }

            if (codiMajorVersion == CodiVersion30)
            {
                return new CodiVersion30FacilitiesProxy(
                    ConfigureDialerChannelFactory<IDialerService30>(
                    dialerServiceEndpoint,
                    dialerServiceAddress,
                    authorizationKeyForOutgoingRequests,
                    catiCommonILoggerToCodiILogger));
            }

            throw new Exception("Unknown CODI version: [" + codiMajorVersion + "]");
        }

        private IChannelFactoryWrapper<T> ConfigureDialerChannelFactory<T>(
            string endpointName,
            string dialerServiceAddress,
            string authorizationKeyForOutgoingRequests,
            CatiCommonILoggerToCodiILogger catiCommonILoggerToCodiILogger) where T : class
        {
            var keepAlive = ServiceLocator.Resolve<IToggleSettings>().EnableHttpKeepAliveForDialer;
            var configuration = new DialerChannelFactoryWrapperConfiguration(
                endpointName,
                dialerServiceAddress,
                authorizationKeyForOutgoingRequests,
                keepAlive);
            
            var channelFactoryWrapperFactory = new ChannelFactoryWrapperFactory<T>();

            return channelFactoryWrapperFactory.Create(configuration, catiCommonILoggerToCodiILogger);
        }
    }
}