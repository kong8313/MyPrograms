using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;
using Confirmit.CATI.DatabaseUpdateLibrary.PowerShellApi;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace Confirmit.CATI.DatabaseUpdateLibrary
{
    public class PowerShellScriptExecutor : IPowerShellScriptExecutor
    {
        private readonly IConfiguration _configuration;

        public PowerShellScriptExecutor(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private const string HeaderScript = @"
Param($API)
$VerbosePreference = ""Continue""
$ErrorActionPreference = ""Stop""
Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process -Force
";

        public string Execute(ILogger logger, string script)
        {
            /*
            There are different versions of System.Management.Automation assembly on deploy servers. 
            Potentially we can execute PS script through power shell infrustructure which uses old version 
            the assembly which doesn't support shell.Streams.Information strem to get script output. 
            To support an execution of PS on such servers/systems we should avoid to use 
            shell.Streams.Information( it is needed to get script output ) through using of verbouse 
            stream for output purpose.
             */
            script = script.Replace("Write-Host", "Write-Verbose");
            var sb = new StringBuilder();

            using (var shell = PowerShell.Create())
            {
                var api = new ApiHost(new ApiConfiguration(_configuration, ConnectionScope.Current.Connection, DatabaseTransactionScope.Current?.Transaction));

                shell.Streams.Verbose.DataAdding += CreateStreamLogger(logger, sb);
                shell.Streams.Warning.DataAdding += CreateStreamLogger(logger, sb);
                shell.Streams.Error.DataAdding += CreateStreamLogger(logger, sb);


                shell.AddScript(HeaderScript + script)
                    .AddParameter("Api", api)
                     .Invoke();
            }

            return sb.ToString();
        }

        private EventHandler<DataAddingEventArgs> CreateStreamLogger(ILogger logger, StringBuilder outputBuilder)
        {
            return (sender, args) =>
            {
                var message = $"{DateTime.Now.ToString("T")} {args.ItemAdded}";

                logger.WriteLog(true, message);
                outputBuilder.AppendLine(message);
            };
        }
    }
}
