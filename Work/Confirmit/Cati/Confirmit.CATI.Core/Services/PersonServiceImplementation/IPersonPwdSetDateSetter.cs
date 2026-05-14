using System;

namespace Confirmit.CATI.Core.Services.PersonServiceImplementation
{
    public interface IPersonPwdSetDateSetter
    {
        void SetPwdSetDateToAllPersons(DateTime pwdSetDate);
    }
}
