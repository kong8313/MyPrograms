using System;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;
using Confirmit.CATI.DatabaseUpdateLibrary;

namespace Confirmit.CATI.DatabaseUpdateLibrary.Interfaces.Fakes
{
    public class StubIResources : IResources 
    {
        private IResources _inner;

        public StubIResources()
        {
            _inner = null;
        }

        public IResources Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private UpdateScriptInfo[] _UpdateScriptInfos;
        public Func<UpdateScriptInfo[]> UpdateScriptInfosGet;
        public Action<UpdateScriptInfo[]> UpdateScriptInfosSetArrayOfUpdateScriptInfo;

        UpdateScriptInfo[] IResources.UpdateScriptInfos
        {
            get
            {
                if (UpdateScriptInfosGet != null)
                {
                    return UpdateScriptInfosGet();
                } else if (_inner != null)
                {
                    return ((IResources)_inner).UpdateScriptInfos;
                }

                if (UpdateScriptInfosSetArrayOfUpdateScriptInfo == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _UpdateScriptInfos;
                }

                return default(UpdateScriptInfo[]);
            }

        }

    }
}