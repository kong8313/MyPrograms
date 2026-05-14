using System;
using Confirmit.CATI.Core.Telephony;
using System.Collections.Generic;
using DialerCommon.DialerParameters;

namespace Confirmit.CATI.Core.Telephony.Fakes
{
    public class StubIDialerSurveyParametersManager : IDialerSurveyParametersManager 
    {
        private IDialerSurveyParametersManager _inner;

        public StubIDialerSurveyParametersManager()
        {
            _inner = null;
        }

        public IDialerSurveyParametersManager Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IEnumerable<DialerParameter> GetDialerDefaultSurveyParametersDelegate();
        public GetDialerDefaultSurveyParametersDelegate GetDialerDefaultSurveyParameters;

        IEnumerable<DialerParameter> IDialerSurveyParametersManager.GetDialerDefaultSurveyParameters()
        {


            if (GetDialerDefaultSurveyParameters != null)
            {
                return GetDialerDefaultSurveyParameters();
            } else if (_inner != null)
            {
                return ((IDialerSurveyParametersManager)_inner).GetDialerDefaultSurveyParameters();
            }

            return default(IEnumerable<DialerParameter>);
        }

        public delegate string GetDialerDefaultSurveyParametersAsXmlDelegate();
        public GetDialerDefaultSurveyParametersAsXmlDelegate GetDialerDefaultSurveyParametersAsXml;

        string IDialerSurveyParametersManager.GetDialerDefaultSurveyParametersAsXml()
        {


            if (GetDialerDefaultSurveyParametersAsXml != null)
            {
                return GetDialerDefaultSurveyParametersAsXml();
            } else if (_inner != null)
            {
                return ((IDialerSurveyParametersManager)_inner).GetDialerDefaultSurveyParametersAsXml();
            }

            return default(string);
        }

        public delegate IEnumerable<DialerParameter> GetDialerSurveyParametersInt32Delegate(int surveySid);
        public GetDialerSurveyParametersInt32Delegate GetDialerSurveyParametersInt32;

        IEnumerable<DialerParameter> IDialerSurveyParametersManager.GetDialerSurveyParameters(int surveySid)
        {


            if (GetDialerSurveyParametersInt32 != null)
            {
                return GetDialerSurveyParametersInt32(surveySid);
            } else if (_inner != null)
            {
                return ((IDialerSurveyParametersManager)_inner).GetDialerSurveyParameters(surveySid);
            }

            return default(IEnumerable<DialerParameter>);
        }

        public delegate void ResetSurveyDialerParametersToDefaultValuesInt32Delegate(int surveySid);
        public ResetSurveyDialerParametersToDefaultValuesInt32Delegate ResetSurveyDialerParametersToDefaultValuesInt32;

        void IDialerSurveyParametersManager.ResetSurveyDialerParametersToDefaultValues(int surveySid)
        {

            if (ResetSurveyDialerParametersToDefaultValuesInt32 != null)
            {
                ResetSurveyDialerParametersToDefaultValuesInt32(surveySid);
            } else if (_inner != null)
            {
                ((IDialerSurveyParametersManager)_inner).ResetSurveyDialerParametersToDefaultValues(surveySid);
            }
        }

        public delegate void SetDialerDefaultSurveyParametersIEnumerableOfDialerParameterDelegate(IEnumerable<DialerParameter> parameters);
        public SetDialerDefaultSurveyParametersIEnumerableOfDialerParameterDelegate SetDialerDefaultSurveyParametersIEnumerableOfDialerParameter;

        void IDialerSurveyParametersManager.SetDialerDefaultSurveyParameters(IEnumerable<DialerParameter> parameters)
        {

            if (SetDialerDefaultSurveyParametersIEnumerableOfDialerParameter != null)
            {
                SetDialerDefaultSurveyParametersIEnumerableOfDialerParameter(parameters);
            } else if (_inner != null)
            {
                ((IDialerSurveyParametersManager)_inner).SetDialerDefaultSurveyParameters(parameters);
            }
        }

        public delegate void SetDialerSurveyParametersInt32IEnumerableOfDialerParameterDelegate(int surveySid, IEnumerable<DialerParameter> parameters);
        public SetDialerSurveyParametersInt32IEnumerableOfDialerParameterDelegate SetDialerSurveyParametersInt32IEnumerableOfDialerParameter;

        void IDialerSurveyParametersManager.SetDialerSurveyParameters(int surveySid, IEnumerable<DialerParameter> parameters)
        {

            if (SetDialerSurveyParametersInt32IEnumerableOfDialerParameter != null)
            {
                SetDialerSurveyParametersInt32IEnumerableOfDialerParameter(surveySid, parameters);
            } else if (_inner != null)
            {
                ((IDialerSurveyParametersManager)_inner).SetDialerSurveyParameters(surveySid, parameters);
            }
        }

        public delegate void ValidateDialerSurveyParametersIEnumerableOfDialerParameterDelegate(IEnumerable<DialerParameter> parameters);
        public ValidateDialerSurveyParametersIEnumerableOfDialerParameterDelegate ValidateDialerSurveyParametersIEnumerableOfDialerParameter;

        void IDialerSurveyParametersManager.ValidateDialerSurveyParameters(IEnumerable<DialerParameter> parameters)
        {

            if (ValidateDialerSurveyParametersIEnumerableOfDialerParameter != null)
            {
                ValidateDialerSurveyParametersIEnumerableOfDialerParameter(parameters);
            } else if (_inner != null)
            {
                ((IDialerSurveyParametersManager)_inner).ValidateDialerSurveyParameters(parameters);
            }
        }

        private bool _DoesDialerHaveSurveyParameters;
        public Func<bool> DoesDialerHaveSurveyParametersGet;
        public Action<bool> DoesDialerHaveSurveyParametersSetBoolean;

        bool IDialerSurveyParametersManager.DoesDialerHaveSurveyParameters
        {
            get
            {
                if (DoesDialerHaveSurveyParametersGet != null)
                {
                    return DoesDialerHaveSurveyParametersGet();
                } else if (_inner != null)
                {
                    return ((IDialerSurveyParametersManager)_inner).DoesDialerHaveSurveyParameters;
                }

                if (DoesDialerHaveSurveyParametersSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DoesDialerHaveSurveyParameters;
                }

                return default(bool);
            }

        }

    }
}