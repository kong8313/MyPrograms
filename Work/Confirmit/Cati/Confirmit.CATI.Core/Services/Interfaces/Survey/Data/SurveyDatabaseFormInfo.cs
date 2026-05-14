using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Core.Services.Interfaces.Survey.Data
{
    public class SurveyDatabaseFormInfo
    {
        public string Name;
        public SurveyDatabaseFieldInfo[] Fields;
        public string[] LoopPath;
    }

    
}
