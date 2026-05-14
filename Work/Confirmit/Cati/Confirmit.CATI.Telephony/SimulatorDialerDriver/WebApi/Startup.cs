using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using Confirmit.CATI.Common;
using Microsoft.Owin.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using Swashbuckle.Application;

namespace SimulatorDialerDriver.WebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            SetupMediaFormatters(config);
            SetupSwagger(config);

            config.MapHttpAttributeRoutes();
            app.UseCors(CorsOptions.AllowAll);

            app.UseWebApi(config);

            var appPath =
                Path.Combine(
                    Confirmit.CATI.Telephony.SimulatorDialerDriver.SimulatorDialerDriver.GetServiceAppDataPath(),
                    "www");
            if (Directory.Exists(appPath))
            {
                app.UseSpaHost(appPath, "/app", "/index.html");

            }
            else
            {
                Trace.WriteLine($"Web App was not started, because '{appPath}' path doesn't exist");
            }
        }

        private void SetupMediaFormatters(HttpConfiguration config)
        {
            var xmlFormatter = config.Formatters.XmlFormatter;
            xmlFormatter.SupportedMediaTypes.Clear();

            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter
            {
                SerializerSettings = new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                }
            });
            config.Formatters.Add(xmlFormatter);
        }

        private void SetupSwagger(HttpConfiguration config)
        {
            config.EnableSwagger(
                    c =>
                    {
                        c.SingleApiVersion("v1", "Open Dialer Simulator API")
                            .Description("The API allows to use dialer simulator features")
                            .Contact(x => x.Name("CATI Team"));
                        c.RootUrl(GetRootUrlFromAppConfig);

                        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"bin\SimulatorDialerDriver.xml");
                        c.IncludeXmlComments(path);
                    })
                .EnableSwaggerUi(c => c.DisableValidator());
        }

        private static string GetRootUrlFromAppConfig(HttpRequestMessage req)
        {
            var scheme = GetHeaderValue(req, "X-Forwarded-Proto") ?? req.RequestUri.Scheme;
            var host = GetHeaderValue(req, "X-Forwarded-Host") ?? req.RequestUri.Host;
            var port = GetHeaderValue(req, "X-Forwarded-Port") ?? req.RequestUri.Port.ToString(CultureInfo.InvariantCulture);
            var virtualPathRoot = req.GetRequestContext().VirtualPathRoot.TrimEnd('/');

            if (port.Equals("80") || port.Equals("443"))
            {
                return $"{scheme}://{host}{virtualPathRoot}";
            }

            return $"{scheme}://{host}:{port}{virtualPathRoot}";
        }

        private static string GetHeaderValue(HttpRequestMessage request, string headerName)
        {
            return request.Headers.TryGetValues(headerName, out var list) ? list.FirstOrDefault() : null;
        }
    }
}
