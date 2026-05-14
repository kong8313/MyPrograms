using System;
using System.IO;
using System.Windows.Forms;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;
using DotNetXmlToAdocConverter.Interfaces;

namespace DotNetXmlToAdocConverter
{
    public class Program
    {
        private readonly IXmlEngine _xmlEngine;
        private readonly IAsciiDocGenerator _asciiDocGenerator;

        public Program(IXmlEngine xmlEngine, IAsciiDocGenerator asciiDocGenerator)
        {
            _xmlEngine = xmlEngine;
            _asciiDocGenerator = asciiDocGenerator;
        }

        public static int Main(string[] args)
        {
            if (args.Length == 1 &&
                (args[0] == "/?" || args[0] == "-?" || args[0] == "/help" ||
                 args[0] == "-help" || args[0] == "/h" || args[0] == "-h"))
            {
                Console.WriteLine(ParametersParser.HelpString);
                return 1;
            }

            ILogger logger;
            try
            {
                logger = new FileAndConsoleLogger(Path.Combine(Application.StartupPath, "DotNetXmlToAdocConverter.txt"));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Logging error:\r\n" + ex);
                return 1;
            }

            try
            {
                ParametersParser parametersParser;

                try
                {
                    parametersParser = new ParametersParser(args);
                }
                catch (ArgumentException ex)
                {
                    logger.WriteLog(true, ex.Message + ParametersParser.HelpString);
                    return 1;
                }

                IXmlEngine xmlEngine = new XmlEngine(parametersParser.InputFile);
                IAsciiDocGenerator asciiDocGenerator = new AsciiDocGenerator(parametersParser.OutputFolder);
                new Program(xmlEngine, asciiDocGenerator).StartProgram(logger);

                return 0;
            }
            catch (Exception ex)
            {
                logger.WriteLog(true, ex.Message);

                logger.WriteLog("Global error:\r\n" + ex);
                return 1;
            }
        }

        private void StartProgram(ILogger logger)
        {
            logger.WriteLog(true, "Start generation of adoc files");
            
            var classes = _xmlEngine.GetClasses();
            logger.WriteLog("Xml file was parsed successfully");

            _asciiDocGenerator.GenerateClassesListFile(classes);
            logger.WriteLog("File with list of classes was generated successfully");

            foreach (var classInfo in classes)
            {
                _asciiDocGenerator.GenerateClassFile(classInfo);
            }
            logger.WriteLog("Files with information for all classes were generated successfully");

            _asciiDocGenerator.GenerateNavigationFile();
            logger.WriteLog("File with navigation information was generated successfully");

            logger.WriteLog(true, "Adoc files were generated successfully");
        }
    }
}
