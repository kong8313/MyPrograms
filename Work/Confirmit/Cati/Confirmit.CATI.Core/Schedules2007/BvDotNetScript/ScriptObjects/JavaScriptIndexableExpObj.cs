using System;
using BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache.FormDescValidators;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;

namespace BvDotNetScript.ScriptObjects
{
    public class JavaScriptIndexableExpObj : ExprObj
    {
        public JavaScriptIndexableExpObj(IInterviewFormDataService dataService, IFormDescValidator validator, FormDescBase desc, int interviewId, string[] loopQualifyer)
            : base(dataService, validator, desc, interviewId, loopQualifyer)
        {
        }

        public override string get()
        {
            throw new NotImplementedException();
        }

        public override string setValue(string sval)
        {
            throw new NotImplementedException();
        }

        public override string setValue(object sval)
        {
            throw new NotImplementedException();
        }

        public override object this[string member]
        {
            get
            {
                if (FormDesc.Categories.Contains(member)) return new JavaScriptExpObj(DataService, Validator,FormDesc, InterviewId, LoopQualifyer, member);
                throw new Exception(string.Format("Question '{0}' does not contain category '{1}'", FormDesc.FormName, member));
            }
            set { throw new NotSupportedException("Objects of this type are not indexable"); }
        }
    }
}