using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface IInterviewQuotaCellRepository
    {
        void Insert(List<BvInterviewQuotaCellEntity> cells);

        void Delete(int surveyId, List<int> interviewIds);

        void Delete(int surveyId);
        List<BvInterviewQuotaCellEntity> GetByInterviewId(int surveyId, int interviewId);
    }
}
