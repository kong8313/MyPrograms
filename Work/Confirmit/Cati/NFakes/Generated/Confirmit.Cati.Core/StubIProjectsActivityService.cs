using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;

namespace Confirmit.CATI.Core.Services.ReplicationServiceImplementation.Fakes
{
    public class StubIProjectsActivityService : IProjectsActivityService 
    {
        private IProjectsActivityService _inner;

        public StubIProjectsActivityService()
        {
            _inner = null;
        }

        public IProjectsActivityService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IEnumerable<string> GetActiveProjectIdsIEnumerableOfStringDelegate(IEnumerable<string> surveys);
        public GetActiveProjectIdsIEnumerableOfStringDelegate GetActiveProjectIdsIEnumerableOfString;

        IEnumerable<string> IProjectsActivityService.GetActiveProjectIds(IEnumerable<string> surveys)
        {


            if (GetActiveProjectIdsIEnumerableOfString != null)
            {
                return GetActiveProjectIdsIEnumerableOfString(surveys);
            } else if (_inner != null)
            {
                return ((IProjectsActivityService)_inner).GetActiveProjectIds(surveys);
            }

            return default(IEnumerable<string>);
        }

    }
}