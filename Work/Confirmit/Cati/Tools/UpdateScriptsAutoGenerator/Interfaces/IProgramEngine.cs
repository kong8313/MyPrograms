namespace UpdateScriptsAutoGenerator.Interfaces
{
    public interface IProgramEngine
    {
        string GetUpdateScriptNamePath(string gitPath, string scriptFolderPath);

        void AddUpdateScriptNameToScriptsDefinitionFile(string scriptsDefinitionFilePath, string updateScriptNamePath);
    }
}