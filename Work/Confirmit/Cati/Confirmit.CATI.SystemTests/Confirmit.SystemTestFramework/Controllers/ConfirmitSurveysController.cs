using System;
using System.IO;
using Confirmit.SystemTestFramework.Settings;

namespace Confirmit.SystemTestFramework.Controllers
{
    public class ConfirmitSurveysController : TestController
    {
        private readonly Authoring.Authoring _authoring;

        public ConfirmitSurveysController(UserInfo userInfo)
        {
            UserInfo = userInfo;
            _authoring = new Authoring.Authoring();
        }

        public string ImportFromFile(string path)
        {
            var surveyXml = File.ReadAllText(path);

            return Import(surveyXml);
        }

        public string Import(string surveyXml)
        {
            try
            {
                var projectId = _authoring.ImportSurvey(
                    UserInfo.ClientKey, surveyXml.Replace("Company=\"Confirmit\" CompanyId=\"1\"", $"Company=\"{Properties.Settings.Default.CompanyName}\" CompanyId=\"{Properties.Settings.Default.CompanyId}\""));
                return projectId;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        public ConfirmitSurveyController this[string pid]
        {
            get
            {
                return new ConfirmitSurveyController(UserInfo, pid);
            }
        }

        public void Delete(string pid)
        {
            _authoring.DeleteProject(UserInfo.ClientKey, pid);
        }
    }
}
