using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Xml;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Telephony;
using DialerCommon;

namespace DialerConfigurationUtility
{
    public class DialersConfigurator
    {
        private const string XmlHeader = "<?xml version=\"1.0\" ?>";

        private readonly IDialerAuthorizationKeyEncryptor _encryptor;
        private readonly ISqlTableUpdatedPublisher _sqlTableUpdatedPublisher;

        public DialersConfigurator(IDialerAuthorizationKeyEncryptor encryptor)
        {
            _encryptor = encryptor;
            _sqlTableUpdatedPublisher = ServiceLocator.Resolve<ISqlTableUpdatedPublisher>();
        }

        internal void UpdateDialerConfigurationParametersFromConfigurationFile(
            string configFile,
            int action,
            IEnumerable<int> dialerIds,
            int companyId,
            bool isAlwaysYes)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(configFile);

            using (var databaseTransactionScope = new DatabaseTransactionScope("UpdateDialersConfiguration"))
            {
                UpdateDialerConfigurationParametersFromConfigurationFile(xmlDocument, databaseTransactionScope, action, dialerIds, companyId, isAlwaysYes);
            }
        }

        internal void UpdateDialerConfigurationParametersFromConfigurationFile(
            XmlDocument xmlDocument,
            IDatabaseTransactionScope databaseTransactionScope,
            int action,
            IEnumerable<int> dialerIds,
            int companyId,
            bool isAlwaysYes)
        {
            try
            {
                var dialersConfigurationNode = xmlDocument.SelectSingleNode("DialersConfiguration");
                var dialerType = dialersConfigurationNode.SelectSingleNode("DialerType").InnerText;
                string siteDialerType = dialerType;

                // Generic dialer types with specific parameters ("GenericPROTS", "GenericSytel", ...) 
                // are still Generic dialer types, so BvSite must still contain "Generic".
                if (dialerType.StartsWith("Generic"))
                {
                    siteDialerType = "Generic";
                }

                // Save dialer type to DB
                string previousDialerType = ServiceLocator.Resolve<ISystemSettings>().Dialer.DialerType;
                ServiceLocator.Resolve<ISystemSettings>().Dialer.DialerType = siteDialerType;

                if (dialerType.Equals("NoDialler"))
                {
                    Console.WriteLine(@"Dialer type is set to 'NoDialler'.");
                    databaseTransactionScope.Commit();
                    return;
                }

                var emptyDialerObject = CreateEmptyDialerObject();

                var selectedDialerRootNode = dialersConfigurationNode.SelectSingleNode(dialerType);

                foreach (var dialerId in dialerIds)
                {
                    ProcessParametersForDialer(dialerId, dialerType, siteDialerType.Equals("Generic"), action,
                        selectedDialerRootNode, companyId);
                }

                // Get default survey parameters from the config file
                var surveyDefaultParametersNode = selectedDialerRootNode.SelectSingleNode("DialerSurveyParameters");
                var surveyDefaultParametersXml = (surveyDefaultParametersNode != null)
                    ? XmlHeader + surveyDefaultParametersNode.OuterXml
                    : null;
                emptyDialerObject.ValidateCampaignParameters(surveyDefaultParametersXml);
                ServiceLocator.Resolve<ISystemSettings>().Dialer.DefaultSurveyParameters = surveyDefaultParametersXml;

                databaseTransactionScope.Commit();

                Console.WriteLine(@"Dialer parameters have been successfully updated.");

                RequestApplyDefaultSurveyDialerParametersToAllSurveys(surveyDefaultParametersXml, siteDialerType,
                    previousDialerType, isAlwaysYes);
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"Exception occured while applying dialers parameters:");
                Console.Write(ex);
            }
        }

        public IDialerAPI CreateEmptyDialerObject()
        {
            try
            {
                var dialerType = ServiceLocator.Resolve<IDialerType>();

                return dialerType.CreateInstance<IDialerAPI>();
            }
            catch (DialerIsNotConfiguredException e)
            {
                Trace.TraceError("Create empty dialer object failed: {0}", e);
                return null;
            }
        }
        private void RequestApplyDefaultSurveyDialerParametersToAllSurveys(
            string surveyDefaultParametersXml, string newDialerType, string previousDialerType, bool isAlwaysYes)
        {
            try
            {
                string action;
                if (!newDialerType.Equals(previousDialerType) && !newDialerType.Equals("NoDialler"))
                {
                    Console.WriteLine(@"Dialer type was changed from " + previousDialerType + @" to " + newDialerType + @".");
                    Console.WriteLine(@"Do you want set default dialer parameters to the surveys which still do not have them? (Y/N)");

                    if (isAlwaysYes)
                    {
                        action = @"/yes";
                        Console.WriteLine(action);
                    }
                    else
                    {
                        do
                        {
                            action = Console.ReadLine();
                        }
                        while (!(action != null && (action.ToLower().Equals("y") || action.ToLower().Equals("n"))));

                        if (action.ToLower().Equals("n"))
                        {
                            return;
                        }
                    }

                    using (var databaseTransactionScope = new DatabaseTransactionScope("UpdateDialerSurveyParameters"))
                    {
                        SetDialerSurveyParametersWhereIsNullAdapter.ExecuteNonQuery(surveyDefaultParametersXml);
                        databaseTransactionScope.Commit();
                        _sqlTableUpdatedPublisher.PublishSurveyUpdated();
                        Console.WriteLine(@"Default dialer parameters were applied successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"Exception occured while applying default dialer parameters:");
                Console.Write(ex);
            }
        }

        private void ProcessParametersForDialer(int dialerId, string dialerType, bool isGenericDialerType, int action, XmlNode selectedDialerRootNode, int companyId)
        {
            if (action == ActionType.RemoveDialer)
            {
                BvDialersAdapter.DeleteByCondition("[Id] = @Id", new SqlParameter("@Id", dialerId));
                return;
            }

            var dialerNode =
                selectedDialerRootNode.SelectSingleNode(String.Format("Dialer[Id='{0}']", dialerId));
            if (dialerNode == null)
            {
                throw new Exception(
                    String.Format(
                        @"Dialer config file does not contain section for dialer {0} of type {1}.",
                        dialerId,
                        dialerType));
            }

            var dialerName = dialerNode.SelectSingleNode("Name").InnerText;

            // TODO: Check that connectionParametersXml is correct, it will require IDialerAPI enhancement
            var connectionParametersNode = dialerNode.SelectSingleNode("DialerConnectionParameters");

            if (isGenericDialerType)
            {
                EncryptAuthorizationKeyForOutgoingRequests(connectionParametersNode);
            }

            var connectionParametersXml = (connectionParametersNode != null) ? (XmlHeader + connectionParametersNode.OuterXml) : null;

            // TODO: Check that configurationParametersXml is correct, it will require IDialerAPI enhancement
            var configurationParametersNode = dialerNode.SelectSingleNode("DialerConfigurationParameters");
            var configurationParametersXml = (configurationParametersNode != null) ? (XmlHeader + configurationParametersNode.OuterXml) : null;

            var dialerEntity = GetBvDialersEntity(dialerId, action);

            dialerEntity.Id = dialerId;
            dialerEntity.Name = dialerName;
            dialerEntity.ConnectionParameters = connectionParametersXml;
            dialerEntity.ConfigurationParameters = configurationParametersXml;
            dialerEntity.TenantId = companyId; // That's Ok for BvTci and PRO-T-S, as for MN - we do not support it any more.
            BvDialersAdapter.Update(dialerEntity);
        }

        private void EncryptAuthorizationKeyForOutgoingRequests(XmlNode connectionParametersNode)
        {
            var authorizationKeyForOutgoingRequests = connectionParametersNode.SelectSingleNode("AuthorizationKeyForOutgoingRequests");

            if (authorizationKeyForOutgoingRequests != null)
            {
                //Encrypt it here
                var enryptedAuthorizationKeyForOutgoingRequests = _encryptor.EncryptString(authorizationKeyForOutgoingRequests.InnerText);
                authorizationKeyForOutgoingRequests.InnerText = enryptedAuthorizationKeyForOutgoingRequests;
            }
            else
            {
                Console.WriteLine(string.Empty);
                Console.WriteLine(@"Warning: Connection parameters node does not contain 'AuthorizationKeyForOutgoingRequests' item. ");
            }
        }

        private BvDialersEntity GetBvDialersEntity(int dialerId, int action)
        {
            var dialerEntity = new DialersRepository().GetById(dialerId);
            switch (action)
            {
                case ActionType.AddDialer:
                    if (dialerEntity != null)
                    {
                        throw new Exception(String.Format("Dialer with Id {0} already exists.", dialerId));
                    }

                    dialerEntity = new BvDialersEntity
                        {
                            Id = dialerId,
                            Name = String.Empty,
                            ConnectionParameters = String.Empty,
                            ConfigurationParameters = String.Empty,
                            IsActive = true
                        };
                    BvDialersAdapter.Insert(dialerEntity);
                    break;
                case ActionType.UpdateDialer:
                case ActionType.RemoveDialer:
                    if (dialerEntity == null)
                    {
                        throw new Exception(String.Format("Dialer with Id {0} does not exist.", dialerId));
                    }

                    break;
            }

            return dialerEntity;
        }
    }
}
