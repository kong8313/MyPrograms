using System;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface ISystemStateRepository
    {
        [CanBeNull]
        string Get(string systemStateName);

        void Set(string systemStateName, string value);

        DateTime? GetReviewerLastInterviewStatusChange();

        void SetReviewerLastInterviewStatusChange(DateTime value);
    }
}