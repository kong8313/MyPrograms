using System;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;
using System.Collections.Generic;
using Confirmit.CATI.DatabaseUpdateLibrary;

namespace Confirmit.CATI.DatabaseUpdateLibrary.Interfaces.Fakes
{
    public class StubIUpdateScriptsProvider : IUpdateScriptsProvider 
    {
        private IUpdateScriptsProvider _inner;

        public StubIUpdateScriptsProvider()
        {
            _inner = null;
        }

        public IUpdateScriptsProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<UpdateScriptInfo> GetScriptsToValidateStringDelegate(string databaseName);
        public GetScriptsToValidateStringDelegate GetScriptsToValidateString;

        List<UpdateScriptInfo> IUpdateScriptsProvider.GetScriptsToValidate(string databaseName)
        {


            if (GetScriptsToValidateString != null)
            {
                return GetScriptsToValidateString(databaseName);
            } else if (_inner != null)
            {
                return ((IUpdateScriptsProvider)_inner).GetScriptsToValidate(databaseName);
            }

            return default(List<UpdateScriptInfo>);
        }

        public delegate List<UpdateScriptInfo> GetScriptsToApplyStringDelegate(string databaseName);
        public GetScriptsToApplyStringDelegate GetScriptsToApplyString;

        List<UpdateScriptInfo> IUpdateScriptsProvider.GetScriptsToApply(string databaseName)
        {


            if (GetScriptsToApplyString != null)
            {
                return GetScriptsToApplyString(databaseName);
            } else if (_inner != null)
            {
                return ((IUpdateScriptsProvider)_inner).GetScriptsToApply(databaseName);
            }

            return default(List<UpdateScriptInfo>);
        }

    }
}