using System;
using DialerWsLogParserLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DialerWsLogParserTest
{
    [TestClass]
    public class RegexStringParserTests
    {
        [TestMethod]
        public void ExtractName_Standart()
        {
            var regexStringParser = new RegexParser("DialerService Information: 0 : +1 2017-10-05 16:30:18.076	" +
                "DialerService.Initialize	Execute [7, 1] /// companyId=17, dialerId=1, configurationParametersXml=<?xml version=\"1.0\" ?>" +
                "<DialerConfigurationParameters><SupportedPersonModes>Manual,CampaignAssignment</SupportedPersonModes><IsReloginNeededOnCampaignChange>" +
                "True</IsReloginNeededOnCampaignChange><IsHangUpSupported>True</IsHangUpSupported><IsPauseOrResumePlaybackSupported>False" +
                "</IsPauseOrResumePlaybackSupported><IsToggleAgentListensToPlaybackOrRespondentSupported>False</IsToggleAgentListensToPlaybackOrRespondentSupported>" +
                "<IsDynamicExtensionNumberAllowedForLocalAgents>False</IsDynamicExtensionNumberAllowedForLocalAgents>" +
                "<IsDynamicExtensionNumberAllowedForRemoteAgents>True</IsDynamicExtensionNumberAllowedForRemoteAgents></DialerConfigurationParameters>");

            Assert.AreEqual(regexStringParser.ExtractName(), "DialerService.Initialize");
        }

        [TestMethod]
        public void ExtractName_Error()
        {
            var regexStringParser = new RegexParser("System.ServiceModel Error: 131075 : <Description>Throwing an exception." +
                "</Description><AppDomain>/LM/W3SVC/1/ROOT/GenericDialerService.Rel-1-131516946176245393</AppDomain>" +
                "<Exception><ExceptionType>System.ServiceModel.FaultException, System.ServiceModel, Version=4.0.0.0, Culture=neutral," +
                " PublicKeyToken=b77a5c561934e089</ExceptionType><Message>The message with Action  cannot be processed at the receiver," +
                " due to a ContractFilter mismatch at the EndpointDispatcher. This may be because of either a contract mismatch (mismatched" +
                " Actions between sender and receiver) or a binding/security mismatch between the sender and the receiver.  Check that sender" +
                " and receiver have the same contract and the same binding (including security requirements, e.g. Message, Transport, None).</Message><StackTrace>" +
                "   at System.ServiceModel.Dispatcher.ErrorBehavior.ThrowAndCatch(Exception e, Message message)");

            Assert.AreEqual(regexStringParser.ExtractName(), "Error");
        }

        [TestMethod]
        public void ExtractName_TypeErrorButNameNotError()
        {
            var regexStringParser = new RegexParser("DialerService Error: 0 :  2018-02-05 15:11:13.686	InvadeConfirmitDialler.Hangup" +
                "	Agent not in call agentID=806199");

            Assert.AreEqual(regexStringParser.ExtractName(), "InvadeConfirmitDialler.Hangup (Error)");
        }

        [TestMethod]
        public void ExtractName_Empty()
        {
            var regexStringParser = new RegexParser("at Confirmit.CATI.Telephony.SimulatorDialerDriver.SimulatorDialerDriver.CheckIfInitialized()" +
                @" in C:\Git\cati\Confirmit.CATI.Telephony\SimulatorDialerDriver\SimulatorDialerDriver.cs:line 136");

            Assert.AreEqual(regexStringParser.ExtractName(), string.Empty);
        }

        [TestMethod]
        public void FindTimeByRegex_Standart()
        {
            var regexStringParser = new RegexParser("DialerService Error: 0 :  2018-02-05 15:11:13.686	InvadeConfirmitDialler.Hangup" +
                "	Agent not in call agentID=806199");

            Assert.AreEqual(regexStringParser.FindTimeByRegex(), "2018-02-05 15:11:13.686");
        }

        [TestMethod]
        public void FindTimeByRegex_Empty()
        {
            var regexStringParser = new RegexParser("w3wp.exe Error: 0 : FaultException`1: Simulator Dialer is not Initialized WCF" +
                " extended error trace results: Service Name: DialerService" +
                "Endpoint URI: http://co-osl-devhv34.firmglobal.com/LTUSimulator(G)DialerService.Rel/DialerService.svc" +
                "Action: http://tempuri.org/IDialerServiceCore/GetState" +
                "Method:" +
                "GetState(companyId = 1, dialerId = 1)" +
                "Exception details:" + 
                "System.ServiceModel.FaultException`1[Confirmit.CATI.Telephony.DialerService.Contract.DialerExceptionDetail]:" +
                " Simulator Dialer is not Initialized (Fault Detail is equal to DialerExceptionDetail: ErrorCode = UnknownError," +
                " ErrorString = ConfirmitDialerInterface.DialerIsNotInitializedException: Simulator Dialer is not Initialized" +
                "at Confirmit.CATI.Telephony.SimulatorDialerDriver.SimulatorDialerDriver.CheckIfInitialized() in" + 
                @"C:\git\cati\Confirmit.CATI.Telephony\SimulatorDialerDriver\SimulatorDialerDriver.cs:line 134" +
                "at Confirmit.CATI.Telephony.SimulatorDialerDriver.SimulatorDialerDriver.GetState(Int32 companyId, Int32 dialerId)" +
                 @"in C:\git\cati\Confirmit.CATI.Telephony\SimulatorDialerDriver\SimulatorDialerDriver.cs:line 268" + 
                 "at Confirmit.CATI.Telephony.DialerService.DialerService.<> c__DisplayClass34_0.< GetState > b__1() in" +
                 @" C:\git\cati\Confirmit.CATI.Telephony\OpenDialer\v.3.6\DialerService\DialerService.cs:line 617" +
                 "at Confirmit.CATI.Telephony.DialerService.DialerService.DoDialerCall[T](Func`1 delegatedCall, Int64 requestId)" +
                 @"in C:\git\cati\Confirmit.CATI.Telephony\OpenDialer\v.3.6\DialerService\DialerService.cs:line 480" + 
                 "at Confirmit.CATI.Telephony.DialerService.DialerS...).");

            Assert.AreEqual(regexStringParser.FindTimeByRegex(), String.Empty);
        }

        [TestMethod]
        public void FindParameterByRegex_Standart()
        {
            var regexStringParser = new RegexParser("DialerService Information: 0 : +3 2019-01-15 11:08:02.085" +
                "	<GetState>b__1	Eof DoDialerCall [2]. Result: ConfirmitDialerInterface.DialerIsNotInitializedException (re-thrown). Duration: 0.");

            Assert.AreEqual(regexStringParser.FindParameterByRegex(@"duration: \d+"), "0");
        }

        [TestMethod]
        public void FindParameterByRegex_Empty()
        {
            var regexStringParser = new RegexParser("DialerService Information: 0 : +3 2019-01-15 11:08:02.085" +
                "	<GetState>b__1	DoDialerCall [2]");

            Assert.AreEqual(regexStringParser.FindParameterByRegex(@"duration: \d+"), String.Empty);
        }

        [TestMethod]
        public void FindManyParametersByRegex_ManyParametersAreEqual()
        {
            var regexStringParser = new RegexParser("DialerService Information: 0 :  2018-02-05 15:11:14.014	DialerService.SendNumbers" +
                "	Execute [81425, 1] /// requestId=161, companyId=517, dialerId=1, campaignId=1863157448, campaignDialingMode=Predictive," +
                " callAgingTimeout=15, numberOfCalls=20, callList=(CallInfo[agentId=0, contactId=5156, callId=14125965, agentGroupId=0," +
                " phoneNumber=01243842832, timeToCall=05/02/2018 15:10:00, diallingMode=Preview, wasAbandoned=False, dialingAttemptsMade=0," +
                " previousConnects=0, numberOfNoAnswer=0, isRecording=True,  agingTimeout=15, dialerSpecificAccompanyInfo=(), callerId = ]," +
                " CallInfo[agentId=0, contactId=763, callId=14115838, agentGroupId=0, phoneNumber=01761414499, timeToCall=05/02/2018 15:10:00," +
                " diallingMode=Preview, wasAbandoned=False, dialingAttemptsMade=0, previousConnects=0, numberOfNoAnswer=0, isRecording=True," +
                "  agingTimeout=15, dialerSpecificAccompanyInfo=(), callerId = ], CallInfo[agentId=0, contactId=2287, callId=14117362," +
                " agentGroupId=0, phoneNumber=01900822328, timeToCall=05/02/2018 15:10:00, diallingMode=Preview, wasAbandoned=False," +
                " dialingAttemptsMade=0, previousConnects=0, numberOfNoAnswer=0, isRecording=True,  agingTimeout=15, dialerSpecificAccompanyInfo=()," +
                " callerId = ], CallInfo[agentId=0, contactId=7620, callId=14128429, agentGroupId=0, phoneNumber=01273686970," +
                " timeToCall=05/02/2018 15:10:00, diallingMode=Preview, wasAbandoned=False, dialingAttemptsMade=0, previousConnects=0," +
                " numberOfNoAnswer=0, isRecording=True,  agingTimeout=15, dialerSpecificAccompanyInfo=(), callerId = ], CallInfo[agentId=0," +
                " contactId=1720, callId=14116795, agentGroupId=0, phoneNumber=01322229338, timeToCall=05/02/2018 15:10:00, diallingMode=Preview," +
                " wasAbandoned=False, dialingAttemptsMade=0, previousConnects=0, numberOfNoAnswer=0, isRecording=True,  agingTimeout=15," +
                " dialerSpecificAccompanyInfo=(), callerId = ], CallInfo[agentId=0, contactId=6016, callId=14126825, agentGroupId=0, " +
                "phoneNumber=08000325551, timeToCall=, diallingMode=Predictive, wasAbandoned=False, dialingAttemptsMade=0, previousConnects=0," +
                " numberOfNoAnswer=0, isRecording=True,  agingTimeout=15, dialerSpecificAccompanyInfo=(), callerId = ], CallInfo[agentId=0," +
                " contactId=9559, callId=14130368, agentGroupId=0, phoneNumber=08456443846, timeToCall=, diallingMode=Predictive, wasAbandoned=False," +
                " dialingAttemptsMade=0, previousConnects=0, numberOfNoAnswer=0, isRecording=True,  agingTimeout=15, dialerSpecificAccompanyInfo=()," +
                " callerId = ], CallInfo[agentId=0, contactId=2764, callId=14117839, agentGroupId=0, phoneNumber=01312783898, timeToCall=," +
                " diallingMode=Predictive, wasAbandoned=False, dialingAttemptsMade=0, previousConnects=0, numberOfNoAnswer=0, isRecording=True," +
                "  agingTimeout=15, dialerSpecificAccompanyInfo=(), callerId = ], CallInfo[agentId=0, contactId=10654, callId=14131463," +
                " agentGroupId=0, phoneNumber=01382732767, timeToCall=, diallingMode=Predictive, wasAbandoned=False, dialingAttemptsMade=0," +
                " previousConnects=0, numberOfNoAnswer=0, isRecording=True,  agingTimeout=15, dialerSpecificAccompanyInfo=(), callerId = ]," +
                " CallInfo[agentId=0, contactId=2948, callId=14118023, agentGroupId=0, phoneNumber=01239698363, timeToCall=, diallingMode=Predictive," +
                " wasAbandoned=False, dialingAttemptsMade=0, previousConnects=0, numberOfNoAnswer=0, isRecording=True,  agingTimeout=15," +
                " dialerSpecificAccompanyInfo=(), callerId = ], CallInfo[agentId=0, contactId=10260, callId=14131069, agentGroupId=0," +
                " phoneNumber=01412552093, timeToCall=, diallingMode=Predictive, wasAbandoned=False, dialingAttemptsMade=0," +
                " previousConnects=0, numberOfNoAnswer=0, isRecording=True,  agingTimeout=15, dialerSpecificAccompanyInfo=(), callerId = ]," +
                " CallInfo[agentId=0, contactId=10848, callId=14131657, agentGroupId=0, phoneNumber=07564956511, timeToCall=," +
                " diallingMode=Predictive, wasAbandoned=False, dialingAttemptsMade=0, previousConnects=0, numberOfNoAnswer=0," +
                " isRecording=True,  agingTimeout=15, dialerSpecificAccompanyInfo=(), callerId = ], CallInfo[agentId=0, contactId=10507," +
                " callId=14131316, agentGroupId=0, phoneNumber=01975563024, timeToCall=, diallingMode=Predictive, wasAbandoned=False," +
                " dialingAttemptsMade=0, previousConnects=0, numberOfNoAnswer=0, isRecording=True,  agingTimeout=15, dialerSpecificAccompanyInfo=()," +
                " callerId = ], CallInfo[agentId=0, contactId=7638, callId=14128447, agentGroupId=0, phoneNumber=01612572676, timeToCall=," +
                " diallingMode=Predictive, wasAbandoned=False, dialingAttemptsMade=0, previousConnects=0, numberOfNoAnswer=0, isRecording=True," +
                "  agingTimeout=15, dialerSpecificAccompanyInfo=(), callerId = ], CallInfo[agentId=0, contactId=2929, callId=14118004," +
                " agentGroupId=0, phoneNumber=01554751432, timeToCall=, diallingMode=Predictive, wasAbandoned=False, dialingAttemptsMade=0," +
                " previousConnects=0, numberOfNoAnswer=0, isRecording=True,  agingTimeout=15, dialerSpecificAccompanyInfo=(), callerId = ]," +
                " CallInfo[agentId=0, contactId=5767, callId=14126576, agentGroupId=0, phoneNumber=01438822042, timeToCall=, diallingMode=Predictive," +
                " wasAbandoned=False, dialingAttemptsMade=0, previousConnects=0, numberOfNoAnswer=0, isRecording=True,  agingTimeout=15," +
                " dialerSpecificAccompanyInfo=(), callerId = ], CallInfo[agentId=0, contactId=1947, callId=14117022, agentGroupId=0," +
                " phoneNumber=02073778430, timeToCall=, diallingMode=Predictive, wasAbandoned=False, dialingAttemptsMade=0, previousConnects=0," +
                " numberOfNoAnswer=0, isRecording=True,  agingTimeout=15, dialerSpecificAccompanyInfo=(), callerId = ], CallInfo[agentId=0," +
                " contactId=9626, callId=14130435, agentGroupId=0, phoneNumber=01923284675, timeToCall=, diallingMode=Predictive," +
                " wasAbandoned=False, dialingAttemptsMade=0, previousConnects=0, numberOfNoAnswer=0, isRecording=True,  agingTimeout=15," +
                " dialerSpecificAccompanyInfo=(), callerId = ], CallInfo[agentId=0, contactId=79, callId=14115154, agentGroupId=0," +
                " phoneNumber=01493780286, timeToCall=, diallingMode=Predictive, wasAbandoned=False, dialingAttemptsMade=0, previousConnects=0," +
                " numberOfNoAnswer=0, isRecording=True,  agingTimeout=15, dialerSpecificAccompanyInfo=(), callerId = ], CallInfo[agentId=0," +
                " contactId=6915, callId=14127724, agentGroupId=0, phoneNumber=01132564771, timeToCall=, diallingMode=Predictive, wasAbandoned=False," +
                " dialingAttemptsMade=0, previousConnects=0, numberOfNoAnswer=0, isRecording=True,  agingTimeout=15, dialerSpecificAccompanyInfo=()," +
                " callerId = ])");

            Assert.AreEqual(regexStringParser.FindParameterByRegex(@"agentId=\d+"), "0");
        }

        [TestMethod]
        public void FindManyParametersByRegex_Standart()
        {
            var regexStringParser = new RegexParser("DialerService Information: 0 :  2018-02-05 15:11:14.014	DialerService.SendNumbers" +
                "	Execute [81425, 1] /// requestId=161, companyId=517, dialerId=1, campaignId=1863157448, campaignDialingMode=Predictive," +
                " callAgingTimeout=15, numberOfCalls=20, callList=(CallInfo[agentId=0, contactId=5156, callId=14125965, agentGroupId=0," +
                " phoneNumber=01243842832, timeToCall=05/02/2018 15:10:00, diallingMode=Preview, wasAbandoned=False, dialingAttemptsMade=0," +
                " previousConnects=0, numberOfNoAnswer=0, isRecording=True,  agingTimeout=15, dialerSpecificAccompanyInfo=(), callerId = ]," +
                " CallInfo[agentId=0, contactId=763, callId=14115838, agentGroupId=0, phoneNumber=01761414499, timeToCall=05/02/2018 15:10:00," +
                " diallingMode=Preview, wasAbandoned=False, dialingAttemptsMade=0, previousConnects=0, numberOfNoAnswer=0, isRecording=True," +
                "  agingTimeout=15, dialerSpecificAccompanyInfo=(), callerId = ], CallInfo[agentId=0, contactId=2287, callId=14117362," +
                " agentGroupId=0, phoneNumber=01900822328, timeToCall=05/02/2018 15:10:00, diallingMode=Preview, wasAbandoned=False," +
                " dialingAttemptsMade=0, previousConnects=0, numberOfNoAnswer=0, isRecording=True,  agingTimeout=15, dialerSpecificAccompanyInfo=()," +
                " callerId = ], CallInfo[agentId=0, contactId=7620, callId=14128429, agentGroupId=0, phoneNumber=01273686970," +
                " timeToCall=05/02/2018 15:10:00, diallingMode=Preview, wasAbandoned=False, dialingAttemptsMade=0, previousConnects=0," +
                " numberOfNoAnswer=0, isRecording=True,  agingTimeout=15, dialerSpecificAccompanyInfo=(), callerId = ], CallInfo[agentId=0," +
                " contactId=1720, callId=14116795, agentGroupId=0, phoneNumber=01322229338, timeToCall=05/02/2018 15:10:00, diallingMode=Preview," +
                " wasAbandoned=False, dialingAttemptsMade=0, previousConnects=0, numberOfNoAnswer=0, isRecording=True,  agingTimeout=15," +
                " dialerSpecificAccompanyInfo=(), callerId = ], CallInfo[agentId=0, contactId=6016, callId=14126825, agentGroupId=0, " +
                "phoneNumber=08000325551, timeToCall=, diallingMode=Predictive, wasAbandoned=False, dialingAttemptsMade=0, previousConnects=0," +
                " numberOfNoAnswer=0, isRecording=True,  agingTimeout=15, dialerSpecificAccompanyInfo=(), callerId = ], CallInfo[agentId=0," +
                " contactId=9559, callId=14130368, agentGroupId=0, phoneNumber=08456443846, timeToCall=, diallingMode=Predictive, wasAbandoned=False," +
                " dialingAttemptsMade=0, previousConnects=0, numberOfNoAnswer=0, isRecording=True,  agingTimeout=15, dialerSpecificAccompanyInfo=()," +
                " callerId = ], CallInfo[agentId=0, contactId=2764, callId=14117839, agentGroupId=0, phoneNumber=01312783898, timeToCall=," +
                " diallingMode=Predictive, wasAbandoned=False, dialingAttemptsMade=0, previousConnects=0, numberOfNoAnswer=0, isRecording=True," +
                "  agingTimeout=15, dialerSpecificAccompanyInfo=(), callerId = ], CallInfo[agentId=0, contactId=10654, callId=14131463," +
                " agentGroupId=0, phoneNumber=01382732767, timeToCall=, diallingMode=Predictive, wasAbandoned=False, dialingAttemptsMade=0," +
                " previousConnects=0, numberOfNoAnswer=0, isRecording=True,  agingTimeout=15, dialerSpecificAccompanyInfo=(), callerId = ]," +
                " CallInfo[agentId=0, contactId=2948, callId=14118023, agentGroupId=0, phoneNumber=01239698363, timeToCall=, diallingMode=Predictive," +
                " wasAbandoned=False, dialingAttemptsMade=0, previousConnects=0, numberOfNoAnswer=0, isRecording=True,  agingTimeout=15," +
                " dialerSpecificAccompanyInfo=(), callerId = ], CallInfo[agentId=0, contactId=10260, callId=14131069, agentGroupId=0," +
                " phoneNumber=01412552093, timeToCall=, diallingMode=Predictive, wasAbandoned=False, dialingAttemptsMade=0," +
                " previousConnects=0, numberOfNoAnswer=0, isRecording=True,  agingTimeout=15, dialerSpecificAccompanyInfo=(), callerId = ]," +
                " CallInfo[agentId=0, contactId=10848, callId=14131657, agentGroupId=0, phoneNumber=07564956511, timeToCall=," +
                " diallingMode=Predictive, wasAbandoned=False, dialingAttemptsMade=0, previousConnects=0, numberOfNoAnswer=0," +
                " isRecording=True,  agingTimeout=15, dialerSpecificAccompanyInfo=(), callerId = ], CallInfo[agentId=0, contactId=10507," +
                " callId=14131316, agentGroupId=0, phoneNumber=01975563024, timeToCall=, diallingMode=Predictive, wasAbandoned=False," +
                " dialingAttemptsMade=0, previousConnects=0, numberOfNoAnswer=0, isRecording=True,  agingTimeout=15, dialerSpecificAccompanyInfo=()," +
                " callerId = ], CallInfo[agentId=0, contactId=7638, callId=14128447, agentGroupId=0, phoneNumber=01612572676, timeToCall=," +
                " diallingMode=Predictive, wasAbandoned=False, dialingAttemptsMade=0, previousConnects=0, numberOfNoAnswer=0, isRecording=True," +
                "  agingTimeout=15, dialerSpecificAccompanyInfo=(), callerId = ], CallInfo[agentId=0, contactId=2929, callId=14118004," +
                " agentGroupId=0, phoneNumber=01554751432, timeToCall=, diallingMode=Predictive, wasAbandoned=False, dialingAttemptsMade=0," +
                " previousConnects=0, numberOfNoAnswer=0, isRecording=True,  agingTimeout=15, dialerSpecificAccompanyInfo=(), callerId = ]," +
                " CallInfo[agentId=0, contactId=5767, callId=14126576, agentGroupId=0, phoneNumber=01438822042, timeToCall=, diallingMode=Predictive," +
                " wasAbandoned=False, dialingAttemptsMade=0, previousConnects=0, numberOfNoAnswer=0, isRecording=True,  agingTimeout=15," +
                " dialerSpecificAccompanyInfo=(), callerId = ], CallInfo[agentId=0, contactId=1947, callId=14117022, agentGroupId=0," +
                " phoneNumber=02073778430, timeToCall=, diallingMode=Predictive, wasAbandoned=False, dialingAttemptsMade=0, previousConnects=0," +
                " numberOfNoAnswer=0, isRecording=True,  agingTimeout=15, dialerSpecificAccompanyInfo=(), callerId = ], CallInfo[agentId=0," +
                " contactId=9626, callId=14130435, agentGroupId=0, phoneNumber=01923284675, timeToCall=, diallingMode=Predictive," +
                " wasAbandoned=False, dialingAttemptsMade=0, previousConnects=0, numberOfNoAnswer=0, isRecording=True,  agingTimeout=15," +
                " dialerSpecificAccompanyInfo=(), callerId = ], CallInfo[agentId=0, contactId=79, callId=14115154, agentGroupId=0," +
                " phoneNumber=01493780286, timeToCall=, diallingMode=Predictive, wasAbandoned=False, dialingAttemptsMade=0, previousConnects=0," +
                " numberOfNoAnswer=0, isRecording=True,  agingTimeout=15, dialerSpecificAccompanyInfo=(), callerId = ], CallInfo[agentId=0," +
                " contactId=6915, callId=14127724, agentGroupId=0, phoneNumber=01132564771, timeToCall=, diallingMode=Predictive, wasAbandoned=False," +
                " dialingAttemptsMade=0, previousConnects=0, numberOfNoAnswer=0, isRecording=True,  agingTimeout=15, dialerSpecificAccompanyInfo=()," +
                " callerId = ])");

            Assert.AreEqual(regexStringParser.FindManyParametersByRegex(@"callId=\d+"), "14125965, 14115838, 14117362, 14128429, 14116795, 14126825, 14130368, 14117839, 14131463, 14118023, 14131069, 14131657, 14131316, 14128447, 14118004, 14126576, 14117022, 14130435, 14115154, 14127724");
        }

        [TestMethod]
        public void FindManyParametersByRegex_OneParameter()
        {
            var regexStringParser = new RegexParser("DialerService Information: 0 :  2018-02-05 15:11:13.733	DialerService.CompletePreview" +
                "	Eof [81424, 0]. Result: Success. Duration: 6 /// companyId=517, dialerId=1, campaignId=1863157448, agentId=759776," +
                " interviewId=6690, callId=14127499, phoneNumber=01295258254, isRecording=True");

            Assert.AreEqual(regexStringParser.FindManyParametersByRegex(@"interviewId=\d+"), "6690");
        }

        [TestMethod]
        public void FindManyParametersByRegex_Empty()
        {
            var regexStringParser = new RegexParser("DialerService Information: 0 :  2018-02-05 15:11:14.669" +
                "	ConfirmitDiallerCallbacks.SendAgentState	Entry. campaignId=1863157448, agentId=730561, agentState=INCALL");

            Assert.AreEqual(regexStringParser.FindManyParametersByRegex(@"interviewId=\d+"), String.Empty);
        }

        [TestMethod]
        public void FindRequestId_Standart()
        {
            var regexStringParser = new RegexParser("<GetState>b__1	[rid=1]	Eof DoDialerCall. Result: ConfirmitDialerInterface.DialerIsNotInitializedException" +
                " (re-thrown). Duration: 1.");

            Assert.AreEqual(regexStringParser.FindRequestId(), 1);
        }

        [TestMethod]
        public void FindRequestId_NoRId()
        {
            var regexStringParser = new RegexParser("<GetState>b__1	Eof DoDialerCall. Result: ConfirmitDialerInterface.DialerIsNotInitializedException" +
                " (re-thrown). Duration: 1.");

            Assert.AreEqual(regexStringParser.FindRequestId(), -1);
        }
    }
}
