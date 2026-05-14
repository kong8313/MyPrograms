using System.Collections.Generic;

namespace TeamCityBuildEngine.Interfaces
{
    public interface ICopyist
    {
        void CopyFile(string sourceFileName, string destFileName);

        void CopyDirectory(string sourceDirectoryName, string destDirectoryName);

        void CopyDirectory(string sourceDirectoryName, string destDirectoryName, List<string> exceptDirectoryPaths);

        void RemoveDirectory(string directoryPath);
    }
}
