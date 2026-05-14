using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Confirmit.CATI.Core.DAL.Framework
{
    public interface IDatabaseAttachService
    {
        bool IsSurveyDatabaseAttached(string projectId);
        void AttachSurveyDatabase(string projectId);
        void DetachSurveyDatabase(string projectId);
    }
}
