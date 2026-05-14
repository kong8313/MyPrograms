using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using NFakes;

namespace NFakesConsole
{
    class Program
    {
        static int Main(string[] args)
        {
            var timer = new Stopwatch();
            timer.Start();

            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) =>
                {
                    var requestedAssemblyName = new AssemblyName(eventArgs.Name);

                    var assembly =
                        (from x in AppDomain.CurrentDomain.GetAssemblies()
                        where x.GetName().Name == requestedAssemblyName.Name
                        select x).FirstOrDefault();

                    return assembly;
                };
                
                var generator = new Generator();

                var assemblyPath = Path.GetFullPath(args[0]);

                var projectPath = args[1] == Generator.NoProj ? Generator.NoProj : Path.GetFullPath(args[1]);

                var outputDirectory = Path.GetFullPath(args[2]);

                var strongNameKeyFile = "";

                if (args.Length == 4)
                {
                    strongNameKeyFile = Path.GetFullPath(args[3]);
                }

                generator.ProcessAssembly(assemblyPath, projectPath, outputDirectory, strongNameKeyFile);

                Console.WriteLine("Finished Successfully in {0} milliseconds", timer.ElapsedMilliseconds);

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                
                Console.WriteLine("\r\nFinished with error.");

                return 1;
            }
        }
    }
}
