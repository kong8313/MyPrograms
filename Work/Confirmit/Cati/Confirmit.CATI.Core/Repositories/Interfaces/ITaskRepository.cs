using System;
using System.Threading.Tasks;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface ITaskRepository
    {
        //TODO: GetById should be renamed to TryGetById
        [CanBeNull]
        BvTasksEntity GetById(int surveySid, int interviewId);

        //TODO: GetByIdWithCheck should be renamed to GetById
        [NotNull]
        BvTasksEntity GetByIdWithCheck(int surveySid, int interviewId);

        [CanBeNull]
        BvTasksEntity GetByPerson(int personSid);

        [CanBeNull]
        BvTasksEntity GetByPersonWithCheck(int personSid);

        void Insert([NotNull] BvTasksEntity task);

        void Update([NotNull] BvTasksEntity task);

        [CanBeNull]
        BvTasksEntity DeleteByPerson(int personSid);

        void Merge([NotNull] BvTasksEntity task);

        Task UpdateActiveQuestion(string projectId, int catiInterviewerId, string questionId, DateTime showTime);

        IEnumerable<int> GetPersonIdsFromBBCC();

        [CanBeNull]
        BvTasksEntity GetByPersonNotLocked(int personSid);

        IEnumerable<BvTasksEntity> GetBySurveyNotLocked(int surveySid);
    }
}
