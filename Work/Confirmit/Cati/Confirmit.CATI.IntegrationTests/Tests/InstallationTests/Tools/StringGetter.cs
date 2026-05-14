using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.InstallationTests.Tools
{
    public class StringGetter
    {
        private readonly string _rootFolderPath;

        public StringGetter(TestContext testContext)
        {
            _rootFolderPath = Path.GetFullPath(Path.Combine(testContext.TestDir, @"..\..\"));
        }

        public string[] GetAutorizationKeysFromConfigFile()
        {
            string dialerConfigXmlPath = Path.Combine(_rootFolderPath, @"assemblies\DialerConfig.xml");
            Trace.TraceInformation("dialerConfigXmlPath={0}", dialerConfigXmlPath);

            // Get
            // 0275E046-7FFF-495B-ACFE-09B439DB4902
            // from
            // <AuthorizationKeyForOutgoingRequests>0275E046-7FFF-495B-ACFE-09B439DB4902</AuthorizationKeyForOutgoingRequests>
            string fileContent = File.ReadAllText(dialerConfigXmlPath);

            var autorizationKeys = new List<string>();
            int firstNumber = 0;
            do
            {
                const string searchString = "<AuthorizationKeyForOutgoingRequests>";
                firstNumber = fileContent.IndexOf(searchString, firstNumber, System.StringComparison.InvariantCulture) + searchString.Length;
                if (firstNumber - searchString.Length == -1)
                {
                    break;
                }

                int secondNumber = fileContent.IndexOf("</AuthorizationKeyForOutgoingRequests>", firstNumber, System.StringComparison.InvariantCulture);

                autorizationKeys.Add(fileContent.Substring(firstNumber, secondNumber - firstNumber));
                firstNumber = secondNumber;
            } while (true);

            return autorizationKeys.ToArray();
        }

        public string GetAutorizationKeyFromProductFile()
        {
            string genericWsInstalltionProductWxsPath = Path.Combine(_rootFolderPath, @"Confirmit.CATI.Setup\Confirmit.CATI.GenericDialerWebService\Product.wxs");
            Trace.TraceInformation("genericWsInstalltionProductWxsPath={0}", genericWsInstalltionProductWxsPath);

            // Get 
            // 0275E046-7FFF-495B-ACFE-09B439DB4902
            //from
            //<Property Id="AUTHORIZATION_KEY" Value="0275E046-7FFF-495B-ACFE-09B439DB4902" />
            string fileContent = File.ReadAllText(genericWsInstalltionProductWxsPath);

            const string searchString = "<Property Id=\"AUTHORIZATION_KEY\" Value=\"";

            int firstNumber = fileContent.IndexOf(searchString, System.StringComparison.InvariantCulture) + searchString.Length;
            int secondNumber = fileContent.IndexOf("\"", firstNumber, System.StringComparison.InvariantCulture);

            return fileContent.Substring(firstNumber, secondNumber - firstNumber);
        }
    }
}
