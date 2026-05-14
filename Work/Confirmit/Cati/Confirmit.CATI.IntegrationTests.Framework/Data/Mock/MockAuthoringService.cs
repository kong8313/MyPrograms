using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.WcfServices.Clients;

namespace Confirmit.CATI.IntegrationTests.Framework.Data.Mock
{
    class MockAuthoringService : IAuthoringService
    {
        private readonly TestDataContext _context;

        public MockAuthoringService(TestDataContext context)
        {
            _context = context;
        }

        public int GetDBVersion(string projectId)
        {
            throw new NotImplementedException();
        }

        public FormBase[] GetFormInfos(string projectId, IEnumerable<string> formNames, SchemaSourceType schemaSourceType)
        {
            var survey = SurveyRepository.GetByName(projectId);
            var controller = _context.Surveys.Single(x => x.Id == survey.SID);

            return formNames.Select(
                x => controller.Data.Forms.SingleOrDefault(
                    f => f.Name == x && f.TableName != "respondent")).
                    Where(fd => fd != null).Select(CreateFormInfo).ToArray();
        }

        public FormBase[] GetFormInfosWithText(string projectId, IEnumerable<string> formNames, SchemaSourceType schemaSourceType)
        {
            throw new NotImplementedException();
        }

        public InterviewHistoryEntry[] GetInterviewHistoryWithValidBackTo(string projectId, string respondentIdentity, int languageId, string domainOverride = null)
        {
            throw new NotImplementedException();
        }

        public int GetCatiCompanyId(string companyAlias)
        {
            throw new NotImplementedException();
        }

        public CatiSupervisorInfo GetCatiSupervisorInfo(string xConfirmitApiKey)
        {
            return new CatiSupervisorInfo
            {
                CompanyId = BackendInstance.Current.CompanyId,
                Id = 1,
                Name = "TestUser",
                Roles = new CatiSupervisorRoles { SystemApiAccess = true, SystemCatiAdministrate = true }
            };
        }

        public IEnumerable<CatiSupervisor> GetCompanyCatiSupervisorsNames(int companyId)
        {
            throw new NotImplementedException();
        }

        public int GetMaximumCatiInterviewers(int companyId)
        {
            return 150;
        }

        public string[] GetProjectsWithSuperviseCATIProjectPermissionForUser(string userName, int companyId)
        {
            throw new NotImplementedException();
        }

        public SurveySchema GetQuestionnaire(string projectId, bool projectSpecific)
        {
            throw new NotImplementedException();
        }

        public FormBase[] GetQuotaForms(string projectId, string quotaName)
        {
            throw new NotImplementedException();
        }

        public QuotaList GetQuotaList(string projectId, string quotaName, QuotaMode quotaMode)
        {
            throw new NotImplementedException();
        }

        public string[] GetQuotaNames(string projectId, QuotaMode quotaMode)
        {
            throw new NotImplementedException();
        }

        public Language[] GetSurveyLanguages(string projectId)
        {
            throw new NotImplementedException();
        }

        public Tabs GetTabPermissions(string loginName, int companyId)
        {
            throw new NotImplementedException();
        }

        public Tabs GetTabPermissions(string loginName, int companyId, string clientKey)
        {
            throw new NotImplementedException();
        }

        public bool HasCatiAddon(int companyId)
        {
            throw new NotImplementedException();
        }

        public bool IsCompanyTelephonyEnabled(int companyId)
        {
            throw new NotImplementedException();
        }

        public void SendMailHtml(string[] addressesTo, string addressBcc, string messageSubject, string messageBody, string messageBodyHtml, byte[] attachment,
            string attachmentName)
        {
            throw new NotImplementedException();
        }

        public void SynchronizeQuota(string projectId, string quotaName, DatabaseType databaseType)
        {
            throw new NotImplementedException();
        }

        public void UpdateQuotaList(string projectId, string quotaName, QuotaList quotaList, DatabaseType databaseType)
        {
            throw new NotImplementedException();
        }

        public CatiIdentityValidationResult ValidateCatiIdentity(
            string confirmitCookieData,
            string catiUserName,
            string catiClientKey,
            bool isCatiHttps)
        {
            throw new NotImplementedException();
        }

        private FormBase CreateFormInfo(FormData formData)
        {
            var single = formData as SingleFormData;
            var multi = formData as MultiFormData;

            FormBase result = new OpenForm();

            if (single != null)
            {
                result = new SingleForm
                {
                    SingleAnswers = new SingleAnswers
                    {
                        Items = single.Precodes.Select(x => (AnswerBase)new Answer { Precode = x }).ToArray()
                    }
                };
            }

            if (multi != null)
            {
                result = new MultiForm
                {
                    MultiAnswers = new MultiAnswers
                    {
                        Items = multi.Precodes.Select(x => (AnswerBase)new Answer { Precode = x }).ToArray()
                    }
                };
            }

            result.Name = formData.Name;
            result.VariableType = VariableDataType.Normal;
            result.FormTexts = new FormText[] { };

            return result;
        }
    }
}
