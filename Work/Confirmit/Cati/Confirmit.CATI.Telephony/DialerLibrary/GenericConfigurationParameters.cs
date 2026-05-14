using System;
using System.Collections.Generic;
using System.Xml;

using ConfirmitDialerInterface;
using DialerCommon.DialerParameters;

namespace Confirmit.CATI.Telephony.DialerLibrary
{
    public class GenericConfigurationParameters
    {
        public List<AgentTaskChoiceMode> SupportedPersonModes;

        public bool IsReloginNeededOnCampaignChange;

        public bool IsHangUpSupported;

        public bool IsPauseOrResumePlaybackSupported;

        public bool IsToggleAgentListensToPlaybackOrRespondentSupported;

        public bool IsDynamicExtensionNumberAllowedForLocalAgents;

        public bool IsDynamicExtensionNumberAllowedForRemoteAgents;

        /// <summary>
        /// See Open dialer API docimentation for details about configurationParametersXml structure
        /// </summary>
        /// <param name="configurationParametersXml"></param>
        public GenericConfigurationParameters(string configurationParametersXml)
        {
            SupportedPersonModes = new List<AgentTaskChoiceMode>();

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(configurationParametersXml);
            var commonParamsNode = xmlDocument.SelectSingleNode("DialerConfigurationParameters");

            if (commonParamsNode != null)
            {
                string supportedPersonModesStr = commonParamsNode.SelectSingleNode("SupportedPersonModes").InnerText;
                if (string.IsNullOrEmpty(supportedPersonModesStr))
                {
                    throw new DialerParametersException(new[]
                            {
                                new DialerParameterError("SupportedPersonModes", "SupportedPersonModes", DialerParameterError.NotSpecified)
                            });
                }

                string[] split = supportedPersonModesStr.Split(new[] { ',' });

                foreach (string s in split)
                {
                    AgentTaskChoiceMode result;
                    if (Enum.TryParse(s, true, out result))
                    {
                        SupportedPersonModes.Add(result);
                    }
                    else
                    {
                        throw new DialerParametersException(new[]
                                {
                                    new DialerParameterError("SupportedPersonModes", "SupportedPersonModes", DialerParameterError.InvalidValue)
                                });
                    }
                }

                this.IsReloginNeededOnCampaignChange = bool.Parse(commonParamsNode.SelectSingleNode("IsReloginNeededOnCampaignChange").InnerText); ;
                this.IsHangUpSupported = bool.Parse(commonParamsNode.SelectSingleNode("IsHangUpSupported").InnerText); ;
                this.IsPauseOrResumePlaybackSupported = bool.Parse(commonParamsNode.SelectSingleNode("IsPauseOrResumePlaybackSupported").InnerText); ;
                this.IsToggleAgentListensToPlaybackOrRespondentSupported = bool.Parse(commonParamsNode.SelectSingleNode("IsToggleAgentListensToPlaybackOrRespondentSupported").InnerText); ;
                this.IsDynamicExtensionNumberAllowedForLocalAgents = bool.Parse(commonParamsNode.SelectSingleNode("IsDynamicExtensionNumberAllowedForLocalAgents").InnerText); ;
                this.IsDynamicExtensionNumberAllowedForRemoteAgents = bool.Parse(commonParamsNode.SelectSingleNode("IsDynamicExtensionNumberAllowedForRemoteAgents").InnerText); ;
            }
        }

    }
}
