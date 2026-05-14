using System;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;
using Confirmit.CATI.DatabaseUpdateLibrary;

namespace Confirmit.CATI.DatabaseUpdateLibrary.Interfaces.Fakes
{
    public class StubIUpdateScriptDatabaseWorker : IUpdateScriptDatabaseWorker 
    {
        private IUpdateScriptDatabaseWorker _inner;

        public StubIUpdateScriptDatabaseWorker()
        {
            _inner = null;
        }

        public IUpdateScriptDatabaseWorker Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate UpdateScriptInfo[] GetAppliedUpdateScriptInfosStringDelegate(string databaseName);
        public GetAppliedUpdateScriptInfosStringDelegate GetAppliedUpdateScriptInfosString;

        UpdateScriptInfo[] IUpdateScriptDatabaseWorker.GetAppliedUpdateScriptInfos(string databaseName)
        {


            if (GetAppliedUpdateScriptInfosString != null)
            {
                return GetAppliedUpdateScriptInfosString(databaseName);
            } else if (_inner != null)
            {
                return ((IUpdateScriptDatabaseWorker)_inner).GetAppliedUpdateScriptInfos(databaseName);
            }

            return default(UpdateScriptInfo[]);
        }

        public delegate void AddAppliedUpdateScriptInfoStringUpdateScriptInfoDelegate(string databaseName, UpdateScriptInfo updateScriptInfo);
        public AddAppliedUpdateScriptInfoStringUpdateScriptInfoDelegate AddAppliedUpdateScriptInfoStringUpdateScriptInfo;

        void IUpdateScriptDatabaseWorker.AddAppliedUpdateScriptInfo(string databaseName, UpdateScriptInfo updateScriptInfo)
        {

            if (AddAppliedUpdateScriptInfoStringUpdateScriptInfo != null)
            {
                AddAppliedUpdateScriptInfoStringUpdateScriptInfo(databaseName, updateScriptInfo);
            } else if (_inner != null)
            {
                ((IUpdateScriptDatabaseWorker)_inner).AddAppliedUpdateScriptInfo(databaseName, updateScriptInfo);
            }
        }

    }
}