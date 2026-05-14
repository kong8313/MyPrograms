using System;
using Confirmit.CATI.Supervisor.Surveys;
using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Surveys.Fakes
{
    public class StubIDiallingModeNameProvider : IDiallingModeNameProvider 
    {
        private IDiallingModeNameProvider _inner;

        public StubIDiallingModeNameProvider()
        {
            _inner = null;
        }

        public IDiallingModeNameProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<DialingModeEntity> GetAllDelegate();
        public GetAllDelegate GetAll;

        List<DialingModeEntity> IDiallingModeNameProvider.GetAll()
        {


            if (GetAll != null)
            {
                return GetAll();
            } else if (_inner != null)
            {
                return ((IDiallingModeNameProvider)_inner).GetAll();
            }

            return default(List<DialingModeEntity>);
        }

    }
}