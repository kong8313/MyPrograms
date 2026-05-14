using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.IntegrationTests.Framework.Tools
{
    public class TestCatiConsole
    {
        public string PersonName { get; set; }
        public string Password { get; set; }
        public int PersonId { get; set; }
        public AgentTaskChoiceMode Mode { get; set; }
        public int SurveyId { get; set; }

        
        public static TestCatiConsole CreateAndLogin(int surveySid, string userName, AgentTaskChoiceMode mode)
        {
            var personId = PersonTools.CreateAssignAndLoginPersonOnSurvey(surveySid, userName, mode);

            return new TestCatiConsole()
                   {
                       SurveyId = surveySid,
                       PersonId = personId,
                       PersonName = userName,
                       Password = "p1",
                       Mode = mode,
                   };
        }

        public static TestCatiConsole CreateAndLoginAsSA(int surveySid, string userName)
        {
            return CreateAndLogin(surveySid, userName, AgentTaskChoiceMode.CampaignAssignment);
        }



        public BvInterviewEntity Start()
        {
            switch (Mode)
            {
                case AgentTaskChoiceMode.CampaignAssignment:
                    var task = TaskService.LookupByPersonSid(PersonId, SurveyId);
                    if (task == null)
                        return null;
                    BvTasksAdapter.Update(task);
                    return InterviewRepository.GetById(SurveyId, task.InterviewID);
                default:
                    throw new NotImplementedException();
            }
            
        }

        
    }
}
