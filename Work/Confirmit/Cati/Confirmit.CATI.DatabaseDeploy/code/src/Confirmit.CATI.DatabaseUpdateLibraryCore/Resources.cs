using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using Confirmit.CATI.DatabaseUpdateLibraryCore.Interfaces;

namespace Confirmit.CATI.DatabaseUpdateLibraryCore
{
    public class Resources : IResources
    {
        private static readonly ZipArchive ScriptsArchive =
            new ZipArchive(Assembly.GetExecutingAssembly().GetManifestResourceStream(Assembly.GetExecutingAssembly().GetName().Name+".scripts.zip"));

        public string GetByName(string name)
        {
            var archiveEntry = ScriptsArchive.GetEntry(name) ?? ScriptsArchive.GetEntry(name.Replace('\\', '/'));
            if (archiveEntry == null)
            {
                throw new Exception($"Resource '{name}' not found");
            }

            using (var entry = archiveEntry.Open())
            using (var reader = new StreamReader(entry))
            {
                return reader.ReadToEnd();
            }
        }

        private UpdateScriptInfo[] _updateScriptInfos;
        public UpdateScriptInfo[] UpdateScriptInfos
        {
            get
            {
                if (_updateScriptInfos != null)
                    return _updateScriptInfos;

                string scriptsDefinitionFileContent = GetByName("ScriptsDefinitionFile.txt");
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
                            x.ScriptText = GetByName(x.FileName);
                            return x;
                        }).ToArray();
                }

                return _updateScriptInfos;
            }
        }

        public string BaseCreationScript => GetByName("Base.Confirmit.CATI.Database.sql");
        public string NewCompanyUpdateScript => GetByName("UpdateScriptForNewCompany.sql");
    }
}