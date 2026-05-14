using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.SystemSettings;
using DialerCommon;
using DialerCommon.DialerParameters;
using Newtonsoft.Json;

namespace Confirmit.CATI.Supervisor.Classes.DialerConfiguration
{
    public class DialerConfigurationConverter
    {
        private readonly IDialerSettings _settings;
        private IDialerAuthorizationKeyEncryptor _encryptor;

        public DialerConfigurationConverter(IDialerSettings settings, IDialerAuthorizationKeyEncryptor encryptor)
        {
            _settings = settings;
            _encryptor = encryptor;
        }

        public static Dictionary<DialerConfigurationType, DiallerType> DialerTypesMap = new Dictionary<DialerConfigurationType, DiallerType>()
        {
            {DialerConfigurationType.Sytel, DiallerType.Generic},
            {DialerConfigurationType.InVade,DiallerType.Generic},
            {DialerConfigurationType.ProTs,DiallerType.PROTS},
            {DialerConfigurationType.Tci,DiallerType.BvTCI},
            {DialerConfigurationType.Simulator,DiallerType.Generic},
            {DialerConfigurationType.AmazonConnect, DiallerType.Generic},
        };

        public DialerSettingTemplate FromXmlToDialerSettingTemplate(
            string configParams,
            string connectionParams)
        {
            var result = new DialerSettingTemplate();

            result.DialerConfigurationParameters = GetParams(configParams);
            result.DialerConnectionParameters = GetParams(connectionParams);

            return result;
        }

        private static List<DialerParameter> GetParams(string configParams)
        {
            var result = new List<DialerParameter>();

            var document = XDocument.Parse(configParams);

            foreach (var element in document.Root.Elements())
            {
                result.Add(new DialerParameter
                {
                    Id = element.Name.ToString(),
                    Value = element.Value
                });
            }

            return result;
        }

        public bool TryGetDialerType(DialerSettingTemplate unknownTemplate, out DiallerType? type, out DialerConfigurationType? configurationType)
        {
            type = null;
            configurationType = null;

            var dialerTemplates = JsonConvert.DeserializeObject<DialerConfigurationList>(_settings.SettingsTemplatesJson);

            DiallerType? byConnectionParams = null;
            foreach (var template in dialerTemplates.DialerSettingTemplates)
            {
                //We have everything from result in template -> template >= result
                var allFromResultContainsInTemplate = unknownTemplate.DialerConnectionParameters.All(x => template.DialerConnectionParameters.Exists(y => y.Id == x.Id));

                if (allFromResultContainsInTemplate)
                {
                    byConnectionParams = template.DialerType;
                    break;
                }
            }

            DiallerType? byConfigParams = null;
            foreach (var template in dialerTemplates.DialerSettingTemplates)
            {
                //We have everything from result in template -> template >= result
                var allFromResultContainsInTemplate = unknownTemplate.DialerConfigurationParameters.All(x => template.DialerConfigurationParameters.Exists(y => y.Id == x.Id));

                if (allFromResultContainsInTemplate)
                {
                    byConfigParams = template.DialerType;
                    break;
                }
            }

            if (byConfigParams == byConnectionParams && byConfigParams.HasValue)
            {
                type = byConfigParams;
                switch (type)
                {
                    case DiallerType.PROTS:
                        configurationType = DialerConfigurationType.ProTs;
                        break;
                    case DiallerType.BvTCI:
                        configurationType = DialerConfigurationType.Tci;
                        break;
                    default:
                        configurationType = DialerConfigurationType.Sytel;
                        break;
                }

                return true;
            }

            return false;
        }

        private void MergeTemplate(DialerSettingTemplate unknownTemplate, Func<DialerSettingTemplate, bool> filterCriteria)
        {
            var dialerTemplates = JsonConvert.DeserializeObject<DialerConfigurationList>(_settings.SettingsTemplatesJson);
        
            var template = dialerTemplates.DialerSettingTemplates.FirstOrDefault(filterCriteria);
        
            if (template == null) return;
        
            MergeParameters(unknownTemplate.DialerConnectionParameters, template.DialerConnectionParameters);
            MergeParameters(unknownTemplate.DialerConfigurationParameters, template.DialerConfigurationParameters);
        }
                
        public void MergeWithTemplate(DialerSettingTemplate unknownTemplate, DialerConfigurationType dialerConfigurationType)
        {
            MergeTemplate(unknownTemplate, x => x.Name.StartsWith(dialerConfigurationType.ToString()));
        }
        
        public void MergeWithTemplate(DialerSettingTemplate unknownTemplate, DiallerType type)
        {
            MergeTemplate(unknownTemplate, x => x.DialerType == type);
        }
        
        private static void MergeParameters(List<DialerParameter> unknownTemplateParams, List<DialerParameter> templateParams)
        {
            foreach (var config in templateParams)
            {
                var existingConfig = unknownTemplateParams.FirstOrDefault(x => x.Id == config.Id);
                if (existingConfig != null)
                {
                    //set name,description and type values from template
                    existingConfig.Name = config.Name;
                    existingConfig.Description = config.Description;
                    existingConfig.Type = config.Type;
                }
                else
                {
                    unknownTemplateParams.Add(config);
                }
            }
        }

        public string GetDialerConnectionParametersXml(DialerSettingTemplate record, bool isNew = false)
        {
            XDocument document = new XDocument(
                new XDeclaration("1.0", "UTF-8", null)
            );
            document.AddFirst(new XElement("DialerConnectionParameters",
                record.DialerConnectionParameters.Select(x => GetXElement(x, isNew))
            ));
            return string.Concat(document.Declaration.ToString(), document.ToString(SaveOptions.DisableFormatting));
        }

        private XElement GetXElement(DialerParameter x, bool isNew)
        {
            var value = x.Value;

            if (isNew && x.Id.Equals("AuthorizationKeyForOutgoingRequests", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(value))
            {
                value = EncryptAuthorizationKeyForOutgoingRequests(value);
            }

            return new XElement(x.Id, value);
        }

        private string EncryptAuthorizationKeyForOutgoingRequests(string key)
        {
            return _encryptor.EncryptString(key);
        }

        public string GetDialerConfigurationParametersXml(DialerSettingTemplate record)
        {
            XDocument document = new XDocument(
                new XDeclaration("1.0", "UTF-8", null)
            );
            document.AddFirst(new XElement("DialerConfigurationParameters",
                record.DialerConfigurationParameters.Select(x => new XElement(x.Id, x.Value))
            ));

            return string.Concat(document.Declaration.ToString(), document.ToString(SaveOptions.DisableFormatting));
        }

        public string GetDialerSurveyParametersXml(DialerSettingTemplate record)
        {
            XDocument document = new XDocument(new XDeclaration("1.0", "UTF-8", null));

            var serializer = new XmlSerializer(typeof(List<DialerParameter>), new XmlRootAttribute("DialerSurveyParameters"));
            
            using (var writer = document.CreateWriter())
            {
                serializer.Serialize(writer, record.DialerSurveyParameters);
                writer.Close();
            }

            return string.Concat(document.Declaration.ToString(), document.ToString(SaveOptions.DisableFormatting));
        }

        public DialerSettingTemplate FromXmlToDialerSettingTemplate(BvDialersEntity dialer)
        {
            return FromXmlToDialerSettingTemplate(dialer.ConfigurationParameters, dialer.ConnectionParameters);
        }
    }
}