using System;
using System.Linq;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;

namespace Confirmit.CATI.DatabaseUpdateLibrary
{
    public class Resources : IResources
    {
        public string GetByName(string name)
        {
            return Properties.Resources.ResourceManager.GetString(name);
        }

        private UpdateScriptInfo[] _updateScriptInfos;
        public UpdateScriptInfo[] UpdateScriptInfos
        {
            get
            {
                if (_updateScriptInfos != null)
                    return _updateScriptInfos;

                string scriptsDefinitionFileContent = Properties.Resources.ResourceManager.GetString("ScriptsDefinitionFile");
                if (string.IsNullOrEmpty(scriptsDefinitionFileContent))
                {
                    _updateScriptInfos = new UpdateScriptInfo[0];
                }
                else
                {
                    _updateScriptInfos = scriptsDefinitionFileContent.Split(
                        new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries).Select(line => new UpdateScriptInfo(line))
                        .Select(x =>
                        {
                            x.ScriptText = GetByName(x.Name);
                            return x;
                        }).ToArray();
                }

                return _updateScriptInfos;
            }
        }
    }
}