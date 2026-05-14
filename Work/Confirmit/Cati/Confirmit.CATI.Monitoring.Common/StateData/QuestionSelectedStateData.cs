using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{    
    [Serializable]
    public class QuestionSelectedStateData : BaseStateData
    {
        public string QuestionId { get; set; }

        public string QuestionText { get; set; }

        public string QuestionTitle { get; set; }

        public int QuestionIndex { get; set; }

        public int SubQuestionIndex { get; set; }

        public bool IsSubQuestion { get; set; }

        public int QuestionType { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
