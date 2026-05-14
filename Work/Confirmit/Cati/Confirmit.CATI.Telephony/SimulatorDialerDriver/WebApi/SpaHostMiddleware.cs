using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.StaticFiles;

namespace SimulatorDialerDriver.WebApi
{
    public class SpaHostMiddleware
    {
        private readonly StaticFileMiddleware _innerMiddleware;
        private readonly Func<IDictionary<string, object>, Task> _next;
        private readonly SpaHostOptions _options;

        public SpaHostMiddleware(Func<IDictionary<string, object>, Task> next, SpaHostOptions options)
        {
            _options = options;
            _next = next;
            _innerMiddleware = new StaticFileMiddleware(next, options.FileServerOptions.StaticFileOptions);
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            IOwinContext context = new OwinContext(environment);
            Trace.WriteLine("Entered SpaHostMiddleware Invoke");
            Trace.WriteLine(string.Format("Route: {0}, Path: {1}", _options.Route, context.Request.Path));

            if (context.Request.Path.StartsWithSegments(_options.Route))
            {
                Trace.WriteLine("Handling Route");
                var fileName = Path.GetFileName(context.Request.Path.Value);
                if (!_options.FileServerOptions.FileSystem.TryGetFileInfo(fileName, out var _))
                {
                    context.Request.Path = _options.EntryPath;

                }
                else
                {
                    context.Request.Path = new PathString("/" + fileName);
                }
                await _innerMiddleware.Invoke(context.Environment);
            }
            else
            {
                await _next.Invoke(environment);
            }
        }
    }
}