using System;
using System.Globalization;
using BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache.FormDescValidators;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;

namespace BvDotNetScript.ScriptObjects
{
    public abstract class ExprObj
    {
        private static readonly NumberFormatInfo numberFormat = new NumberFormatInfo();
        protected readonly IFormDescValidator Validator;

        public static NumberFormatInfo NumberFormat
        {
            get { return numberFormat; }
        }

        protected ExprObj(IInterviewFormDataService dataService, IFormDescValidator validator, FormDescBase desc, int interviewId, string[] loopQualifyer)
        {
            DataService = dataService;
            FormDesc = desc;
            InterviewId = interviewId;
            LoopQualifyer = loopQualifyer;
            Validator = validator;
        }

        protected readonly IInterviewFormDataService DataService;
        protected readonly FormDescBase FormDesc;
        protected readonly int InterviewId;
        protected readonly string[] LoopQualifyer;

        #region IJavaScriptExpObj Members

        public bool CODED
        {
            get { return FormDesc.CODED; }
        }

        public bool DICHOTOMY
        {
            get { return FormDesc.DICHOTOMY; }
        }

        public bool COMPOUND
        {
            get { return FormDesc.COMPOUND; }
        }

        public bool OPEN
        {
            get { return FormDesc.OPEN; }
        }

        public bool DATE
        {
            get { return FormDesc.DATE; }
        }

        public bool BOOL
        {
            get { return FormDesc.BOOL; }
        }

        public bool EXTERNAL
        {
            get { return FormDesc.EXTERNAL; }
        }

        public bool NUMERIC
        {
            get { return FormDesc.NUMERIC; }
        }

        public string label()
        {
            return FormDesc.Label;
        }

        public string text()
        {
            return FormDesc.Text;
        }

        public string instruction()
        {
            return FormDesc.Instruction;
        }

        public abstract string get();
        public abstract string setValue(string sval);
        public abstract string setValue(object sval);

        public double toNumber()
        {
            double result = Double.NaN;
            
            string val = get();

            if (val != null)
                Double.TryParse(val, out result);

            return result;
        }

        public bool increment()
        {
            if (!FormDesc.NUMERIC)
                return false;

            double value = toNumber();

            if (double.IsNaN(value))
            {
                reset();
                value = toNumber();
            }

            value += 1;

            if (IsAllowableValue(value.ToString()).IsSuccess)
                setValue(value);
            else
                setValue(null);

            return true;
        }

        protected ValidationResult IsAllowableValue(string s)
        {
            return Validator.Validate(FormDesc.ValidationData, s);
        }

        public bool decrement()
        {
            if (!FormDesc.NUMERIC)
                return false;

            double value = toNumber();

            if (double.IsNaN(value))
            {
                reset();
                value = toNumber();
            }

            value -= 1;

            if (IsAllowableValue(value.ToString()).IsSuccess)
                setValue(value);
            else
                setValue(null);

            return true;
        }

        public bool reset()
        {
            if (!FormDesc.NUMERIC)
                return false;

            double value = 0;

            if (IsAllowableValue(value.ToString()).IsSuccess)
                setValue(value);
            else
                setValue(null);

            return true;
        }

        public DateTime? toDate()
        {
            string val = get();
            if (val != null)
                return DateTime.Parse(val, NumberFormat);
            return null;
        }

        public int toInt()
        {
            string val = get();
            
            int result = 0;
            if (int.TryParse(val, out result))
            {
                return (result);                
            }
            throw new SchedulingScriptExecutionException(
                String.Format("Survey variable '{0}' ({1}) could not be converted to an integer.", FormDesc.FormName, val ?? "null"));
        }

        public bool toBoolean()
        {
            string val = get();
            return !string.IsNullOrEmpty(val);
        }

        public string toString()
        {
            string val = get();
            return val;
        }

        public string value()
        {
            return get();
        }

        #endregion

        public virtual object this[string member]
        {
            get { throw new NotSupportedException("Objects of this type are not indexable"); }
            set { throw new NotSupportedException("Objects of this type are not indexable"); }
        }
    }
}