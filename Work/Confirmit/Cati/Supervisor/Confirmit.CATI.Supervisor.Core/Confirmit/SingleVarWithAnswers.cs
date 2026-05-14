using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.AuthoringService;

namespace Confirmit.CATI.Supervisor.Core.Confirmit
{
    public class SingleVarWithAnswers : VariableInfo
    { 
        public List<string> Precodes { get; set; }
        public List<string> AnswersList { get; set; }

        public SingleVarWithAnswers(string name, VariableTypes variableType, TableTypes tableType)
            : base(name, variableType, tableType, name, "", ConfirmitVariableType.Single)
        {
            
        }
    }
}
