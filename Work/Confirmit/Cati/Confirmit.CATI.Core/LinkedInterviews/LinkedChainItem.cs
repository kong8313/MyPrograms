using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Confirmit.CATI.Core.LinkedInterviews
{
    [Serializable]
    public class LinkedChainItem
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public int InterviewId { get; set; }
    }
}
