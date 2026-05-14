using System;
using Confirmit.CATI.Core.Services.ApiClients.Models;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubIRespondentsClient : IRespondentsClient 
    {
        private IRespondentsClient _inner;

        public StubIRespondentsClient()
        {
            _inner = null;
        }

        public IRespondentsClient Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate int AddRespondentStringRespondentsInfoDelegate(string projectId, RespondentsInfo importDefinition);
        public AddRespondentStringRespondentsInfoDelegate AddRespondentStringRespondentsInfo;

        int IRespondentsClient.AddRespondent(string projectId, RespondentsInfo importDefinition)
        {


            if (AddRespondentStringRespondentsInfo != null)
            {
                return AddRespondentStringRespondentsInfo(projectId, importDefinition);
            } else if (_inner != null)
            {
                return ((IRespondentsClient)_inner).AddRespondent(projectId, importDefinition);
            }

            return default(int);
        }

    }
}