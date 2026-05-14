using System;
using System.Collections.Generic;
using Confirmit.CATI.Common;
using DialerCommon.DialerParameters;

namespace Confirmit.CATI.Supervisor.Classes.DialerConfiguration
{
    [Serializable]
    public class DialerSettingTemplate
    {
        public DialerSettingTemplate()
        {
            DialerConfigurationParameters = new List<DialerParameter>();
            DialerConnectionParameters = new List<DialerParameter>();
            DialerSurveyParameters = new List<DialerParameter>();
        }

        public string Name { get; set; }
        public DiallerType DialerType { get; set; }
        public List<DialerParameter> DialerConnectionParameters { get; set; }
        public List<DialerParameter> DialerConfigurationParameters { get; set; }
        public List<DialerParameter> DialerSurveyParameters { get; set; }
    }

    public class DialerConfigurationList
    {
        public List<DialerSettingTemplate> DialerSettingTemplates { get; set; }
    }
}