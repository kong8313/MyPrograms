using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Core.Services;

namespace Confirmit.CATI.IntegrationTests.Tests.CallDelivering.CallDeliveringTools
{
    public class Tools
    {
        public const int AmountOfCallsPerGroup = 20;

        public static IEnumerable<BvInterviewEntity> CreateInterviewWithCalls(int surveyId, int amount, DateTime timeToCall)
        {
            var result = new List<BvInterviewEntity>();
            for (int i = 0; i < amount; ++i)
            {
                var interview = CreateInterviewWithCall(surveyId, 1, timeToCall);
                result.Add(interview);
            }

            return result;
        }

        public static BvInterviewEntity CreateInterviewWithCall(int surveyId, short priority, DateTime timeToCall)
        {
            var interview = BackendTools.NewInterview(surveyId);
            BackendTools.CreateInterview(interview);
            var priorityCall1 = BackendTools.NewCall(interview);
            priorityCall1.Priority = priority;
            priorityCall1.TimeInShift = timeToCall;
            BackendTools.CreateCall(priorityCall1);

            return interview;
        }

        public static void AssignPersonToInterviews(int surveyId, int personId, IEnumerable<int> interviewIds)
        {
            foreach (var interviewId in interviewIds)
                BackendTools.AssignResourceToInterview(surveyId, interviewId, personId);
        }

        public static IEnumerable<BvTasksEntity> GetAllAccessibleTasks(int personId) //should be at least one task
        {
            while (true)
            {
                BvTasksEntity task = TaskService.LookupByPersonSid(personId, 0);

                if (task == null)
                    yield break;

                yield return task;
            }
        }
    }
}
