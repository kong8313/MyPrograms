using System;
using System.Linq;
using BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache.FormDescValidators;

namespace BvDotNetScript.ScriptObjects
{
    public class JavaScriptExpObj : ExprObj
    {
        protected readonly string _category;

        internal JavaScriptExpObj(IInterviewFormDataService dataService, IFormDescValidator validator, FormDescBase desc, int interviewId, string[] loopQualifyer, string category = null)
            : base(dataService, validator, desc, interviewId, loopQualifyer)
        {
            _category = category;
        }

        public override string get()
        {
            if (FormDesc.IsReplicated)
            {
                return ReplicationService.GetReplicationValue(FormDesc.SurveyId, InterviewId, FormDesc.FormName);
            }
            else
            {
                if (FormDesc.COMPOUND)
                {
                    throw new SchedulingScriptExecutionException(
                                    String.Format("Cannot get value of the survey variable '{0}'. The variable type is not supported.",
                                    FormDesc.FormName));

                }

                var result = DataService.GetFormValue(FormDesc, _category, LoopQualifyer);

                return result ?? String.Empty;
            }
        }

        public override string setValue(string sval)
        {
            if (FormDesc.IsReplicated)
            {
                throw new SchedulingScriptExecutionException(
                                String.Format("Replicated survey variable '{0}' could not be updated.", FormDesc.FormName));
            }

            if (FormDesc.COMPOUND)
            {
                throw new SchedulingScriptExecutionException(
                                String.Format("Survey variable '{0}' could not be updated.", FormDesc.FormName));

            }

            var validationResult = IsAllowableValue(sval);
            if (!validationResult.IsSuccess)
            {
                throw new SchedulingScriptExecutionException(
                                String.Format("Survey variable '{0}' could not be updated to '{1}' because of error: {2}", FormDesc.FormName, sval ?? "<null>", validationResult.ErrorMessage));

            }

            DataService.SetFormValue(FormDesc, _category, LoopQualifyer, sval);
            
            return sval;
        }

        public override string setValue(object sval)
        {
            if (FormDesc.COMPOUND)
            {
                throw new SchedulingScriptExecutionException(
                                String.Format("Survey variable '{0}' could not be updated.", FormDesc.FormName));
            }
            if (sval != null && sval != DBNull.Value)
            {
                Type t = sval.GetType();
                if (t == typeof(decimal))
                    return setValue(((decimal)sval).ToString("G28", NumberFormat));
                else if (t == typeof(double))
                    return setValue(((double)sval).ToString("G28", NumberFormat));
                else if (t == typeof(float))
                    return setValue(((float)sval).ToString("G28", NumberFormat));
                else if (t == typeof(bool))
                    return setValue(Convert.ToString((bool)sval));
                else
                    return setValue(Convert.ToString(sval, NumberFormat));
            }
            else
                return setValue(null);
        }
    }
}
