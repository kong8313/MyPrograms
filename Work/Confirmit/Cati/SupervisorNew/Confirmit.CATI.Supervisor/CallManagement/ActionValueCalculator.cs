using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Supervisor.CallManagement
{
    public class ActionValueCalculator
    {
        private ICallProvider _provider;

        public ActionValueCalculator(ICallProvider provider)
        {
            _provider = provider;
        }

        public T Calculate<T>(int surveySid, List<int> ids, Func<BvCallEntity, T> callFunc, T defaultValue)
        {
            if (!ids.Any())
            {
                throw new ArgumentOutOfRangeException("ids");
            }

            if (ids.Count > 1) return defaultValue;

            var firstInterviewId = ids.First();
            var firstInterview = _provider.GetCallAndNoLock(surveySid, firstInterviewId);

            return callFunc(firstInterview);
        }
    }
}