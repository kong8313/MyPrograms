using ConfirmitDialerInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Confirmit.CATI.Core.Repositories
{
    public static class InterviewStatusRepository
    {
        public static InterviewStatus GetByItsAndStateGroupId(string itsString, int stateGroupId)
        {
            int its;
            if (int.TryParse(itsString, out its))
            {
                var bvEntity = StateRepository.GetByItsAndStateGroupId(its, stateGroupId);

                if (bvEntity != null)
                {
                    return new InterviewStatus { Code = bvEntity.StateID, Name = bvEntity.Name };
                }
            }

            return ConfirmitStatusRepository.GetByConfirmitStatus(itsString);
        }
    }
}
