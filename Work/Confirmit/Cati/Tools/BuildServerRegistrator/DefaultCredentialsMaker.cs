using Confirmit.Security.Crypto;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace BuildServerRegistrator
{
    public class DefaultCredentialsMaker
    {
        private readonly ConfirmQueryExecutor _confirmQueryExecutor;
        private readonly ConfigParameters _serverConfigurations;

        public DefaultCredentialsMaker(ConfirmQueryExecutor confirmQueryExecutor, ConfigParameters serverConfigurations)
        {
            _confirmQueryExecutor = confirmQueryExecutor;
            _serverConfigurations = serverConfigurations;
        }

        public void SetDefaultCfgServerConfigValues()
        {
            var cfgServerConfigValues = new List<CfgServerConfigValue>();

            cfgServerConfigValues.Add(GetCfgServerConfigValue("DeployC"));
            cfgServerConfigValues.Add(GetCfgServerConfigValue("DeployN"));
            cfgServerConfigValues.Add(GetCfgServerConfigValue("AuthorC"));
            cfgServerConfigValues.Add(GetCfgServerConfigValue("AuthorN"));
            cfgServerConfigValues.Add(GetCfgServerConfigValue("ReportC"));
            cfgServerConfigValues.Add(GetCfgServerConfigValue("ReportN"));
            cfgServerConfigValues.Add(GetCfgServerConfigValue("SurveyN"));

            _serverConfigurations.CfgServerConfigValues = cfgServerConfigValues;
        }

        private CfgServerConfigValue GetCfgServerConfigValue(string configName)
        {
            return new CfgServerConfigValue { ConfigId = GetConfigId(configName), EncryptedConfigValue = GetEncryptedConfigValue(configName) };
        }

        private string GetConfigId(string configName)
        {
            try
            {
                string query = $"SELECT [ConfigId] FROM [CfgConfig] WHERE [ConfigName] = @configName";
                return _confirmQueryExecutor.ExecuteScalar<int>(query, new SqlParameter("@configName", configName)).ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Can't read config id for '{configName}' in CfgConfig table", ex);
            }
        }

        private string GetEncryptedConfigValue(string configName)
        {
            string configValue = null;
            switch (configName)
            {
                case "DeployC":
                    configValue = "UID=ConfirmitDeploy;PWD=%1confdep;";
                    break;
                case "DeployN":
                    configValue = "NConfirmitDeploy";
                    break;
                case "AuthorC":
                    configValue = "UID=ConfirmitAuthor;PWD=%1confaut;";
                    break;
                case "AuthorN":
                    configValue = "NConfirmitAuthor";
                    break;
                case "SurveyC":
                    configValue = "UID=ConfirmitSurvey;PWD=%1confsur;";
                    break;
                case "SurveyN":
                    configValue = "NConfirmitSurvey";
                    break;
                case "ReportC":
                    configValue = "UID=ConfirmitReport;PWD=%1confrep;";
                    break;
                case "ReportN":
                    configValue = "NConfirmitDeploy";
                    break;
                    
            }

            return new CryptComp().Encrypt(configValue);
        }

        public string GetEncryptedSurveyCCredentials()
        {
            return GetEncryptedConfigValue("SurveyC");
        }
    }
}
