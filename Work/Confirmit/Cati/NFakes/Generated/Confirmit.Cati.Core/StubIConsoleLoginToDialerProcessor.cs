using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Telephony.Console;

namespace Confirmit.CATI.Core.Telephony.Console.Fakes
{
    public class StubIConsoleLoginToDialerProcessor : IConsoleLoginToDialerProcessor 
    {
        private IConsoleLoginToDialerProcessor _inner;

        public StubIConsoleLoginToDialerProcessor()
        {
            _inner = null;
        }

        public IConsoleLoginToDialerProcessor Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvSurveyEntity LoginToDialerBvPersonEntityBvTasksEntityStringBvSurveyEntityBooleanOutDelegate(BvPersonEntity person, BvTasksEntity task, string extensionNumber, BvSurveyEntity survey, out bool isPredictive);
        public LoginToDialerBvPersonEntityBvTasksEntityStringBvSurveyEntityBooleanOutDelegate LoginToDialerBvPersonEntityBvTasksEntityStringBvSurveyEntityBooleanOut;

        BvSurveyEntity IConsoleLoginToDialerProcessor.LoginToDialer(BvPersonEntity person, BvTasksEntity task, string extensionNumber, BvSurveyEntity survey, out bool isPredictive)
        {
            isPredictive = default(bool);


            if (LoginToDialerBvPersonEntityBvTasksEntityStringBvSurveyEntityBooleanOut != null)
            {
                return LoginToDialerBvPersonEntityBvTasksEntityStringBvSurveyEntityBooleanOut(person, task, extensionNumber, survey, out isPredictive);
            } else if (_inner != null)
            {
                return ((IConsoleLoginToDialerProcessor)_inner).LoginToDialer(person, task, extensionNumber, survey, out isPredictive);
            }

            return default(BvSurveyEntity);
        }

    }
}