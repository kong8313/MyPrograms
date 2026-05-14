using System.Collections.Generic;

namespace DotNetXmlToAdocConverter.Interfaces
{
    public interface IAsciiDocGenerator
    {
        void GenerateClassesListFile(List<ClassInfo> classes);

        void GenerateClassFile(ClassInfo classInfo);

        void GenerateNavigationFile();
    }
}