using System;
using Confirmit.CATI.Core.Services.PersonServiceImplementation;

namespace Confirmit.CATI.Core.Services.PersonServiceImplementation.Fakes
{
    public class StubIPersonPwdSetDateSetter : IPersonPwdSetDateSetter 
    {
        private IPersonPwdSetDateSetter _inner;

        public StubIPersonPwdSetDateSetter()
        {
            _inner = null;
        }

        public IPersonPwdSetDateSetter Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void SetPwdSetDateToAllPersonsDateTimeDelegate(DateTime pwdSetDate);
        public SetPwdSetDateToAllPersonsDateTimeDelegate SetPwdSetDateToAllPersonsDateTime;

        void IPersonPwdSetDateSetter.SetPwdSetDateToAllPersons(DateTime pwdSetDate)
        {

            if (SetPwdSetDateToAllPersonsDateTime != null)
            {
                SetPwdSetDateToAllPersonsDateTime(pwdSetDate);
            } else if (_inner != null)
            {
                ((IPersonPwdSetDateSetter)_inner).SetPwdSetDateToAllPersons(pwdSetDate);
            }
        }

    }
}