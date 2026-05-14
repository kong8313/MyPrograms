using System.Collections.Generic;
using System.IO;
using System.Text;
using DotNetXmlToAdocConverter.Interfaces;

namespace DotNetXmlToAdocConverter
{
    public class AsciiDocGenerator : IAsciiDocGenerator
    {
        private readonly string _outputFolder;

        public AsciiDocGenerator(string outputFolder)
        {
            _outputFolder = outputFolder;
        }

        public void GenerateClassesListFile(List<ClassInfo> classes)
        {
            var contents = new Dictionary<string, StringBuilder>();

            foreach (var classInfo in classes)
            {
                if (!contents.ContainsKey(classInfo.Namespace))
                {
                    contents.Add(classInfo.Namespace, new StringBuilder($"\r\n= {classInfo.Namespace}\r\n\r\n[options=\"header\", cols=\".^3a,.^11a\"]\r\n|===\r\n|Name|Description\r\n"));
                }

                contents[classInfo.Namespace].AppendLine($"|xref:{classInfo.Namespace}/{classInfo.Name}.adoc[**{classInfo.Name}**]|{classInfo.Description}");
            }

            foreach (var contentsKey in contents.Keys)
            {
                contents[contentsKey].AppendLine("|===");

                SaveFile(Path.Combine(_outputFolder, "pages", $"{contentsKey}.adoc"), contents[contentsKey].ToString());
            }
        }

        public void GenerateClassFile(ClassInfo classInfo)
        {
            var content = new StringBuilder($"\r\n== {classInfo.Name}\r\n{classInfo.Description}\r\n\r\n");

            if (classInfo.Constructors.Count > 0)
            {
                content.Append("=== Constructors\r\n");
            }

            foreach (var constructor in classInfo.Constructors)
            {
                content.Append($"\r\n---\r\n{constructor.Name}:: {constructor.Description}\r\n\r\n");

                if (constructor.Parameters.Count > 0)
                {
                    content.Append("==== *Parameters*\r\n");
                }

                foreach (var parameter in constructor.Parameters)
                {
                    content.Append($"{parameter.Name}:: {parameter.Description}\r\n");
                }
            }

            if (classInfo.Properties.Count > 0)
            {
                content.Append("\r\n=== Properties\r\n");

                foreach (var property in classInfo.Properties)
                {
                    content.Append($"{property.Name}:: {property.Description}\r\n");
                }
            }

            if (classInfo.Methods.Count > 0)
            {
                content.Append("\r\n=== Methods\r\n");

                foreach (var method in classInfo.Methods)
                {
                    content.Append($"\r\n---\r\n{method.Name}:: {method.Description}\r\n\r\n");

                    if (method.Parameters.Count > 0)
                    {
                        content.Append("==== *Parameters*\r\n");
                    }

                    foreach (var parameter in method.Parameters)
                    {
                        content.Append($"{parameter.Name}:: {parameter.Description}\r\n");
                    }

                    if (!string.IsNullOrEmpty(method.Returns))
                    {
                        content.Append($"\r\n.*Returns*\r\n{method.Returns}\r\n\r\n");
                    }
                }
            }

            SaveFile(Path.Combine(_outputFolder, "pages", classInfo.Namespace, classInfo.Name + ".adoc"), content.ToString());
        }

        public void GenerateNavigationFile()
        {
            var content = new StringBuilder("* CATI Rest SDK\r\n** xref:Manual.adoc[Manual]\r\n");

            AddNavInfoForFolder(content, Path.Combine(_outputFolder, "pages"), "**");

            content.Append("* CATI Rest API\r\n** xref:RestApi/overview.adoc[Overview]\r\n** xref:RestApi/paths.adoc[Resources]\r\n** xref:RestApi/definitions.adoc[Definitions]");

            SaveFile(Path.Combine(_outputFolder, "nav.adoc"), content.ToString());
        }

        private void AddNavInfoForFolder(StringBuilder content, string folderPath, string stars)
        {
            foreach (var fileInfo in new DirectoryInfo(folderPath).GetFiles("*.adoc"))
            {
                if (SkipFile(fileInfo.FullName.ToLower()))
                {
                    continue;
                }

                var relativePath = fileInfo.FullName.Replace(Path.Combine(_outputFolder, "pages") + "\\", "").Replace("\\", "/");
                var fileNameWithoutExtension = fileInfo.Name.Replace(".adoc", "");

                content.AppendLine($"{stars} xref:{relativePath}[{fileNameWithoutExtension}]");

                string childFolderPath = Path.Combine(fileInfo.Directory.FullName, fileNameWithoutExtension);
                if (Directory.Exists(childFolderPath))
                {
                    AddNavInfoForFolder(content, childFolderPath, stars + "*");
                }
            }
        }

        private bool SkipFile(string filePath)
        {
            return filePath.EndsWith("manual.adoc") || filePath.EndsWith("index.adoc") || filePath.StartsWith("restapi\\");
        }

        private void SaveFile(string path, string content)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            string directoryPath = Path.GetDirectoryName(path);
            if (directoryPath!= null && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            File.WriteAllText(path, content);
        }
    }
}