using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace Confirmit.CATI.Installation.Common
{
    public class AssemblyComparer : IComparer
    {
        private readonly IExternalInvoker _externalInvoker;
        private readonly string _ildasmPath;
        private readonly string _outPath;

        public AssemblyComparer(IExternalInvoker externalInvoker, string ildasmPath)
            : this(externalInvoker, ildasmPath, null)
        {
        }

        public AssemblyComparer(IExternalInvoker externalInvoker, string ildasmPath, string outPath)
        {
            _externalInvoker = externalInvoker;
            _ildasmPath = ildasmPath;
            _outPath = outPath;
        }

        public int Compare(object assemblyPathWithoutExtention, object scriptAssemblyPathWithoutExtention)
        {
            string extention = File.Exists(assemblyPathWithoutExtention + ".dll") ? "dll" : "exe";
            string assemblyIlPath;
            string scriptAssemblyIlPath;
            if (string.IsNullOrEmpty(_outPath))
            {
                assemblyIlPath = assemblyPathWithoutExtention.ToString();
                scriptAssemblyIlPath = scriptAssemblyPathWithoutExtention.ToString();
            }
            else
            {
                string assemblyDirectoryPath = Path.Combine(_outPath, "1");
                string scriptAssemblyDirectoryPath = Path.Combine(_outPath, "2");

                if (!Directory.Exists(assemblyDirectoryPath))
                {
                    Directory.CreateDirectory(assemblyDirectoryPath);
                }

                if (!Directory.Exists(scriptAssemblyDirectoryPath))
                {
                    Directory.CreateDirectory(scriptAssemblyDirectoryPath);
                }

                assemblyIlPath = Path.Combine(assemblyDirectoryPath, Path.GetFileName(assemblyPathWithoutExtention.ToString()));
                scriptAssemblyIlPath = Path.Combine(scriptAssemblyDirectoryPath, Path.GetFileName(assemblyPathWithoutExtention.ToString()));
            }
            _externalInvoker.Invoke(_ildasmPath, string.Format("\"{0}.{1}\" /out=\"{2}.il\"", assemblyPathWithoutExtention, extention, assemblyIlPath));
            _externalInvoker.Invoke(_ildasmPath, string.Format("\"{0}.{1}\" /out=\"{2}.il\"", scriptAssemblyPathWithoutExtention, extention, scriptAssemblyIlPath));

            string assemblyContent = RemoveVariableStrings(File.ReadAllText(assemblyIlPath + ".il"));
            string scriptAssemblyContent = RemoveVariableStrings(File.ReadAllText(scriptAssemblyIlPath + ".il"));

            int compareRes = string.Compare(assemblyContent, scriptAssemblyContent, StringComparison.InvariantCulture);
            if (compareRes != 0)
            {
                Trace.TraceInformation("!!!assemblyContent={0}", assemblyContent);
                Trace.TraceInformation("!!!scriptAssemblyContent={0}", scriptAssemblyContent);
            }

            CleanMemory();

            return compareRes;
        }

        /// <summary>
        /// Remove 3 strings:
        /// // MVID: {BB3FB0AF-29F3-4575-A8CA-D7C8E596E59C}
        /// // Image base: 0x00390000
        /// // WARNING: Created Win32 resource file BvSqlCallQueue.res
        /// </summary>
        /// <param name="ilFileContent">Content of .il file</param>
        /// <returns></returns>
        private string RemoveVariableStrings(string ilFileContent)
        {
            var regEx = new Regex(@"// MVID: .*");
            ilFileContent = regEx.Replace(ilFileContent, string.Empty);
            CleanMemory();

            regEx = new Regex(@"// Image base: .*");
            ilFileContent = regEx.Replace(ilFileContent, string.Empty);
            CleanMemory();

            regEx = new Regex(@"// WARNING: Created Win32 resource file .*");
            ilFileContent = regEx.Replace(ilFileContent, string.Empty);
            CleanMemory();
            
            ilFileContent = RemoveBvSqlCallQueueVersion(ilFileContent);
            CleanMemory();

            ilFileContent = RemoveAssemblyFileVersionAttribute(ilFileContent);
            CleanMemory();
            ilFileContent = RemoveAssemblyProductAttribute(ilFileContent);
            CleanMemory();
            ilFileContent = RemoveAssemblyTitleAttribute(ilFileContent);
            CleanMemory();
            ilFileContent = RemoveTrademarkAttribute(ilFileContent);
            CleanMemory();
            ilFileContent = RemoveInternalsVisibleAttribute(ilFileContent);
            CleanMemory();
            ilFileContent = RemoveAssemblyCopyrightAttribute(ilFileContent);
            CleanMemory();
            ilFileContent = RemovePrivateImplementationDetailsGuid(ilFileContent);
            CleanMemory();

            return ilFileContent;
        }

        private string RemoveBvSqlCallQueueVersion(string ilFileContent)
        {
            // Remove lines with version for BvSqlCallQueue like 
            // .ver 18:0:123:12345
            int bvSqlCallQueueIndex = ilFileContent.IndexOf(".assembly BvSqlCallQueue", StringComparison.Ordinal);
            if (bvSqlCallQueueIndex == -1)
            {
                return ilFileContent;
            }

            int verIndex = ilFileContent.IndexOf(" .ver ", bvSqlCallQueueIndex, StringComparison.Ordinal);
            if (verIndex == -1)
            {
                return ilFileContent;
            }

            int endLineIndex = ilFileContent.IndexOf("\r\n", verIndex, StringComparison.Ordinal);
            return ilFileContent.Substring(0, verIndex) + ilFileContent.Substring(endLineIndex);
        }

        private void CleanMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            Thread.Sleep(10);
        }

        private string RemovePrivateImplementationDetailsGuid(string ilFileContent)
        {
            /*
            Remove GUID after PrivateImplementationDetails string
            <PrivateImplementationDetails>{448A9FFC-E5BD-4811-8EA6-77926DD4FD6F} 
            replace to
            <PrivateImplementationGUIDDetails>
            */
            int firstIndex;
            while ((firstIndex = ilFileContent.IndexOf("<PrivateImplementationDetails>{", StringComparison.Ordinal)) > 0)
            {
                int secondIndex = ilFileContent.IndexOf("}", firstIndex, StringComparison.Ordinal);
                ilFileContent = ilFileContent.Substring(0, firstIndex) + "<PrivateImplementationGUIDDetails>" + ilFileContent.Substring(secondIndex + 1);
                CleanMemory();                
            }

            return ilFileContent;
        }

        private string RemoveAssemblyProductAttribute(string ilFileContent)
        {
            /*
             Remove AssemblyProductAttribute information
               .custom instance void [mscorlib]System.Reflection.AssemblyProductAttribute::.ctor(string) = ( 01 00 16 43 6F 6E 66 69 72 6D 69 74 20 48 6F 72   // ...Confirmit Hor
                                                                                                69 7A 6F 6E 73 20 52 65 6C 00 00 )                // izons Rel..
             or
                .custom instance void [mscorlib]System.Reflection.AssemblyProductAttribute::.ctor(string) = ( 01 00 17 43 6F 6E 66 69 72 6D 69 74 20 48 6F 72   // ...Confirmit Hor
                                                                                                69 7A 6F 6E 73 20 4D 61 69 6E 00 00 )             // izons Main..       
             */

            int assemblyProductAttributeIndex = ilFileContent.IndexOf("  .custom instance void [mscorlib]System.Reflection.AssemblyProductAttribute::.ctor(string)", StringComparison.Ordinal);
            int nextAssemblyAttributeIndex = GetLastAttributeIndex(ilFileContent, assemblyProductAttributeIndex + 1);

            if (assemblyProductAttributeIndex > 0 && nextAssemblyAttributeIndex > assemblyProductAttributeIndex)
            {
                return ilFileContent.Substring(0, assemblyProductAttributeIndex) + ilFileContent.Substring(nextAssemblyAttributeIndex);
            }

            return ilFileContent;
        }

        private string RemoveAssemblyFileVersionAttribute(string ilFileContent)
        {
            /*
             Remove AssemblyFileVersionAttribute information
            .custom instance void [mscorlib]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = ( 01 00 08 31 37 2E 35 2E 30 2E 30 00 00 )          // ...17.5.0.0..
             or 
            .custom instance void [mscorlib]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = ( 01 00 0F 31 37 2E 35 2E 35 36 32 38 2E 36 31 36   // ...17.5.5628.616
                                                                                                    39 31 00 00 )                                     // 91..
             */

            int assemblyFileVersionAttribute = ilFileContent.IndexOf("  .custom instance void [mscorlib]System.Reflection.AssemblyFileVersionAttribute::.ctor(string)", StringComparison.Ordinal);
            int nextAssemblyAttributeIndex = GetLastAttributeIndex(ilFileContent, assemblyFileVersionAttribute + 1);

            if (assemblyFileVersionAttribute > 0 && nextAssemblyAttributeIndex > assemblyFileVersionAttribute)
            {
                return ilFileContent.Substring(0, assemblyFileVersionAttribute) + ilFileContent.Substring(nextAssemblyAttributeIndex);
            }

            return ilFileContent;
        }   

        private string RemoveAssemblyTitleAttribute(string ilFileContent)
        {
            /*
             Remove AssemblyTitleAttribute information
             .custom instance void [mscorlib]System.Reflection.AssemblyTitleAttribute::.ctor(string) = ( 01 00 13 53 78 53 3A 20 52 65 6C 2E 20 43 53 3A   // ...SxS: Rel. CS:
                                                                                              20 37 30 33 30 38 00 00 )                         //  70308..
             or
             .custom instance void [mscorlib]System.Reflection.AssemblyTitleAttribute::.ctor(string) = ( 01 00 18 53 78 53 3A 20 47 72 69 67 6F 72 79 4B   // ...SxS: GrigoryK
                                                                                              2E 20 43 53 3A 20 36 35 35 33 36 00 00 )          // . CS: 65536..
            */

            int assemblyTitleAttributeIndex = ilFileContent.IndexOf("  .custom instance void [mscorlib]System.Reflection.AssemblyTitleAttribute::.ctor(string)", StringComparison.Ordinal);
            int nextAssemblyAttributeIndex = GetLastAttributeIndex(ilFileContent, assemblyTitleAttributeIndex + 1);

            if (assemblyTitleAttributeIndex > 0 && nextAssemblyAttributeIndex > assemblyTitleAttributeIndex)
            {
                return ilFileContent.Substring(0, assemblyTitleAttributeIndex) + ilFileContent.Substring(nextAssemblyAttributeIndex);
            }

            return ilFileContent;
        }

        private string RemoveTrademarkAttribute(string ilFileContent)
        {
            /*
             Remove AssemblyTrademarkAttribute information
             .custom instance void [mscorlib]System.Reflection.AssemblyTrademarkAttribute::.ctor(string) = ( 01 00 12 43 6F 6E 66 69 72 6D 69 74 20 48 6F 72   // ...Confirmit Hor
                                                                                69 7A 6F 6E 73 00 00 )                            // izons..
            */

            int assemblyTrademarkAttributeIndex = ilFileContent.IndexOf("  .custom instance void [mscorlib]System.Reflection.AssemblyTrademarkAttribute::.ctor(string)", StringComparison.Ordinal);
            int nextAssemblyTrademarkAttributeIndex = GetLastAttributeIndex(ilFileContent, assemblyTrademarkAttributeIndex + 1);

            if (assemblyTrademarkAttributeIndex > 0 && nextAssemblyTrademarkAttributeIndex > assemblyTrademarkAttributeIndex)
            {
                return ilFileContent.Substring(0, assemblyTrademarkAttributeIndex) + ilFileContent.Substring(nextAssemblyTrademarkAttributeIndex);
            }

            return ilFileContent;
        }
        
        private string RemoveInternalsVisibleAttribute(string ilFileContent)
        {
            /*
             Remove InternalsVisibleToAttribute information
             .custom instance void [mscorlib]System.Runtime.CompilerServices.InternalsVisibleToAttribute::.ctor(string) = ( 01 00 26 43 6F 6E 66 69 72 6D 69 74 2E 43 41 54   // ..&amp;Confirmit.CAT
                                                                                                                 49 2E 44 61 74 61 62 61 73 65 2E 43 6F 72 65 2E   // I.Database.Core.
                                                                                                                 55 6E 69 74 54 65 73 74 73 00 00 )                // UnitTests..
            */

            int assemblyInternalsVisibleAttributeIndex = ilFileContent.IndexOf("  .custom instance void [mscorlib]System.Runtime.CompilerServices.InternalsVisibleToAttribute::.ctor(string)", StringComparison.Ordinal);
            int nextAssemblyInternalsVisibleAttributeIndex = GetLastAttributeIndex(ilFileContent, assemblyInternalsVisibleAttributeIndex + 1);

            if (assemblyInternalsVisibleAttributeIndex > 0 && nextAssemblyInternalsVisibleAttributeIndex > assemblyInternalsVisibleAttributeIndex)
            {
                return ilFileContent.Substring(0, assemblyInternalsVisibleAttributeIndex) + ilFileContent.Substring(nextAssemblyInternalsVisibleAttributeIndex);
            }

            return ilFileContent;
        }

        private string RemoveAssemblyCopyrightAttribute(string ilFileContent)
        {
            /*
             Remove AssemblyCopyrightAttribute information
             .custom instance void [mscorlib]System.Reflection.AssemblyCopyrightAttribute::.ctor(string) = ( 01 00 32 43 6F 70 79 72 69 67 68 74 20 32 30 31   // ..2Copyright 201
                                                                                                  33 20 43 6F 6E 66 69 72 6D 69 74 20 41 53 41 2E   // 3 Confirmit ASA.
                                                                                                  20 41 6C 6C 20 72 69 67 68 74 73 20 72 65 73 65   //  All rights rese
                                                                                                  72 76 65 64 2E 00 00 )                            // rved...
             */

            int assemblyProductAttributeIndex = ilFileContent.IndexOf("  .custom instance void [mscorlib]System.Reflection.AssemblyCopyrightAttribute::.ctor(string)", StringComparison.Ordinal);
            int nextAssemblyAttributeIndex = GetLastAttributeIndex(ilFileContent, assemblyProductAttributeIndex + 1);

            if (assemblyProductAttributeIndex > 0 && nextAssemblyAttributeIndex > assemblyProductAttributeIndex)
            {
                return ilFileContent.Substring(0, assemblyProductAttributeIndex) + ilFileContent.Substring(nextAssemblyAttributeIndex);
            }

            return ilFileContent;
        }

        private int GetLastAttributeIndex(string ilFileContent, int assemblyAttributeIndex)
        {
            int index = ilFileContent.IndexOf("= (", assemblyAttributeIndex + 1, StringComparison.Ordinal);
            index = ilFileContent.IndexOf(")", index + 1, StringComparison.Ordinal);
            return ilFileContent.IndexOf("\r\n", index + 1, StringComparison.Ordinal) + 2;
        }
    }
}