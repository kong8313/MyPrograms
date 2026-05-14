using System;
using System.IO;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;

namespace SimulatorDialerDriver.WebApi
{
    public static class SpaHostExtension
    {
        public static IAppBuilder UseSpaHost(this IAppBuilder builder, string rootPath, string route, string entryPath)
        {
            var options = new SpaHostOptions
            {
                FileServerOptions = new FileServerOptions
                {
                    EnableDirectoryBrowsing = false,
                    FileSystem = new PhysicalFileSystem(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rootPath))
                },
                EntryPath = new PathString(entryPath),
                Route = new PathString(route)
            };

            builder.UseDefaultFiles(options.FileServerOptions.DefaultFilesOptions);

            return builder.Use<SpaHostMiddleware>(options);
        }
    }
}