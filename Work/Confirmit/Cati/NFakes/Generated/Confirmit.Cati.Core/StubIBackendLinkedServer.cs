using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.Misc.Fakes
{
    public class StubIBackendLinkedServer : IBackendLinkedServer 
    {
        private IBackendLinkedServer _inner;

        public StubIBackendLinkedServer()
        {
            _inner = null;
        }

        public IBackendLinkedServer Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetBackendLinkedServerNameBvSurveyEntityDelegate(BvSurveyEntity survey);
        public GetBackendLinkedServerNameBvSurveyEntityDelegate GetBackendLinkedServerNameBvSurveyEntity;

        string IBackendLinkedServer.GetBackendLinkedServerName(BvSurveyEntity survey)
        {


            if (GetBackendLinkedServerNameBvSurveyEntity != null)
            {
                return GetBackendLinkedServerNameBvSurveyEntity(survey);
            } else if (_inner != null)
            {
                return ((IBackendLinkedServer)_inner).GetBackendLinkedServerName(survey);
            }

            return default(string);
        }

    }
}