using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Build.Utilities;
using StaticTeamCityBuildEngine.CommonEngines;

namespace StaticTeamCityBuildEngine
{
    public class DBUpdateResorceFileGenerator : Task
    {
        public override bool Execute()
        {
            try
            {
                Log.LogMessage("Generation of resx file was started");

                string executablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;                
                string scriptsfolderPath = Path.GetFullPath(Path.Combine(executablePath, @"..\Confirmit.CATI.Database.2012\Scripts\"));
                var resxContent = new StringBuilder();

                resxContent.Append(GetHeadOfFile());

                foreach (var scriptVersionFolderPath in Directory.GetDirectories(scriptsfolderPath))
                {
                    Log.LogMessage("Collecting in " + scriptVersionFolderPath);
                    foreach (var scriptFilePath in Directory.GetFiles(scriptVersionFolderPath))
                    {
                        string updateScriptNameInResourceFile = Path.GetFileNameWithoutExtension(scriptFilePath) ?? string.Empty;
                        updateScriptNameInResourceFile = "_" + updateScriptNameInResourceFile.Replace('.', '_');
                        string projectRootPath = Directory.GetParent(Directory.GetParent(Directory.GetParent(scriptVersionFolderPath).FullName).FullName).FullName;
                        string relaitedUpdateScriptNamePath = scriptFilePath.Replace(projectRootPath, @"..\..");
                        resxContent.AppendFormat("  <data name=\"{0}\" type=\"System.Resources.ResXFileRef, System.Windows.Forms\">\r\n", updateScriptNameInResourceFile);
                        resxContent.AppendFormat("    <value>{0};System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089;windows-1251</value>\r\n", relaitedUpdateScriptNamePath);
                        resxContent.AppendFormat("  </data>\r\n");
                    }
                }

                resxContent.Append("</root>");

                string resxPath = Path.GetFullPath(Path.Combine(executablePath, @"..\Confirmit.CATI.DatabaseUpdate.Library\Properties\Resources.resx"));

                var createBranchFileEngine = new CreateBranchFilesEngine(Log, new ExternalExecutor(Log));
                createBranchFileEngine.RecreateFileIfContentIsNew(resxPath, resxContent.ToString()); 

                Log.LogMessage("Execution has finished successfully");
            }
            catch (Exception ex)
            {
                Log.LogMessage(ex.ToString());

                throw;
            }

            return true;
        }

        private string GetHeadOfFile()
        {
            return
@"<?xml version=""1.0"" encoding=""utf-8""?>
<root>  
  <xsd:schema id=""root"" xmlns="""" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:msdata=""urn:schemas-microsoft-com:xml-msdata"">
    <xsd:import namespace=""http://www.w3.org/XML/1998/namespace"" />
    <xsd:element name=""root"" msdata:IsDataSet=""true"">
      <xsd:complexType>
        <xsd:choice maxOccurs=""unbounded"">
          <xsd:element name=""metadata"">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name=""value"" type=""xsd:string"" minOccurs=""0"" />
              </xsd:sequence>
              <xsd:attribute name=""name"" use=""required"" type=""xsd:string"" />
              <xsd:attribute name=""type"" type=""xsd:string"" />
              <xsd:attribute name=""mimetype"" type=""xsd:string"" />
              <xsd:attribute ref=""xml:space"" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name=""assembly"">
            <xsd:complexType>
              <xsd:attribute name=""alias"" type=""xsd:string"" />
              <xsd:attribute name=""name"" type=""xsd:string"" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name=""data"">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name=""value"" type=""xsd:string"" minOccurs=""0"" msdata:Ordinal=""1"" />
                <xsd:element name=""comment"" type=""xsd:string"" minOccurs=""0"" msdata:Ordinal=""2"" />
              </xsd:sequence>
              <xsd:attribute name=""name"" type=""xsd:string"" use=""required"" msdata:Ordinal=""1"" />
              <xsd:attribute name=""type"" type=""xsd:string"" msdata:Ordinal=""3"" />
              <xsd:attribute name=""mimetype"" type=""xsd:string"" msdata:Ordinal=""4"" />
              <xsd:attribute ref=""xml:space"" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name=""resheader"">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name=""value"" type=""xsd:string"" minOccurs=""0"" msdata:Ordinal=""1"" />
              </xsd:sequence>
              <xsd:attribute name=""name"" type=""xsd:string"" use=""required"" />
            </xsd:complexType>
          </xsd:element>
        </xsd:choice>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>
  <resheader name=""resmimetype"">
    <value>text/microsoft-resx</value>
  </resheader>
  <resheader name=""version"">
    <value>2.0</value>
  </resheader>
  <resheader name=""reader"">
    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <resheader name=""writer"">
    <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <assembly alias=""System.Windows.Forms"" name=""System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"" />
  <data name=""ScriptsDefinitionFile"" type=""System.Resources.ResXFileRef, System.Windows.Forms"">
    <value>..\..\Confirmit.CATI.Database.2012\Scripts\ScriptsDefinitionFile.txt;System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089;windows-1251</value>
  </data>
";
        }
    }
}
