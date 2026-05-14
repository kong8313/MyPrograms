using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Telephony.SimulatorDialerDriver
{
    public class SimulatorScenario
    {
        [XmlElement]
        public bool GenerateRequestCalls { get; set; }

        [XmlElement]
        public CallOutcomeDistributionScenario CallOutcomeDistributionScenario { get; set; }

        [XmlIgnore]
        public TimeSpan RequestFrequency
        {
            get { return TimeSpan.Parse(XmlRequestFrequency); }
        }

        [XmlElement("RequestFrequency")]
        public string XmlRequestFrequency { get; set; }

        [XmlIgnore]
        public TimeSpan MaxRequestTime
        {
            get { return TimeSpan.Parse(XmlMaxRequestTime); }
        }

        [XmlElement("MaxRequestTime")]
        public string XmlMaxRequestTime { get; set; }

        [XmlElement]
        public int CallsCountPerInterviewer { get; set; }

        [XmlElement]
        public DialerErrorCode LoginResultCode { get; set; }

        [XmlElement]
        public DialerErrorCode SetCampaignResultCode { get; set; }

        [XmlElement]
        public int GoNotReadyNotificationDelay { get; set; }

        [XmlElement("GenerateInboundCalls")]
        public bool GenerateInboundCalls { get; set; }

        [XmlIgnore]
        public TimeSpan InboundCallRequestFrequency
        {
            get { return TimeSpan.Parse(XmlInboundCallRequestFrequency); }
        }

        [XmlElement("InboundCallsQueueLimit")]
        public int InboundCallsQueueLimit { get; set; }

        [XmlElement("InboundCallRequestFrequency")]
        public string XmlInboundCallRequestFrequency { get; set; }

        [XmlElement("DdiNumber")]
        public string DdiNumber { get; set; }

        [XmlElement("CliNumber")]
        public string CliNumber { get; set; }
        
        [XmlElement("DialerCallerId")]
        public string DialerCallerId { get; set; }

        [XmlElement("IvrAnswerDelayInSeconds")]
        public int IvrAnswerDelayInSeconds { get; set; }

        [XmlElement]
        public bool SendNotReadyNotificationOnLogin { get; set; }

        [XmlElement]
        public IvrStaticAnswers IvrStaticAnswers { get; set; }

        [XmlElement("AudioFolderName")]
        public string AudioFolderName { get; set; }

        [XmlElement("AudioServerName")]
        public string AudioServerName { get; set; }

        [XmlElement("AudioServerSchema")]
        public string AudioServerSchema { get; set; }

        [XmlIgnore]
        public Dictionary<string, string> IvrQuestionIdToAnswer { get; private set; }

        public SimulatorScenario()
        {
            // Set default values
            GenerateRequestCalls = true;
            GenerateInboundCalls = false;
            SendNotReadyNotificationOnLogin = false;
            InboundCallsQueueLimit = 100;
            IvrAnswerDelayInSeconds = 1;
        }

        public void OnDeserialized()
        {
            IvrQuestionIdToAnswer = new Dictionary<string, string>();

            if (!HaveIvrStaticAnswers())
            {
                return;
            }

            foreach (var questionAnswerPair in IvrStaticAnswers.Items)
            {
                if (IvrQuestionIdToAnswer.ContainsKey(questionAnswerPair.QuestionId))
                {
                    Trace.TraceWarning(
                        "SimulatorScenario.OnDeserialized: Key ['{0}'] is duplicated. " +
                        "Existing pair is ['{1}', '{2}']. New pair is ['{3}', '{4}']. " +
                        "The existing pair will be overwritten by the new one.",
                        questionAnswerPair.QuestionId,
                        questionAnswerPair.QuestionId, IvrQuestionIdToAnswer[questionAnswerPair.QuestionId],
                        questionAnswerPair.QuestionId, questionAnswerPair.Answer);
                }

                IvrQuestionIdToAnswer[questionAnswerPair.QuestionId] = questionAnswerPair.Answer;
            }
        }

        private bool HaveIvrStaticAnswers()
        {
            return ((IvrStaticAnswers != null) && (IvrStaticAnswers.Items.Count > 0));
        }
    }

    [XmlRoot(ElementName="IvrStaticAnswers")]
    public class IvrStaticAnswers
    {
        [XmlElement(ElementName = "item")]
        public List<QuestionAnswerPair> Items;
    }

    [XmlRoot(ElementName = "item")]
    public class QuestionAnswerPair
    {
        [XmlAttribute]
        public string QuestionId;
        [XmlAttribute]
        public string Answer;
    }
}