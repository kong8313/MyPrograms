using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.ServerControls.Commands
{
    public class OverlayCommandClientFunctionGenerator
    {
        public OverlayCommandClientFunctionGenerator(string functionName,
                                                     string url,
                                                     string title,
                                                     NameValueCollection parentParameters,
                                                     string staticInlineInLineParameters,
                                                     Dictionary<string, string> dynamicParameters,
                                                     Dictionary<string, string> dynamicClientParameters,
                                                     int height,
                                                     int width,
                                                     int? top,
                                                     bool showInCurrentFrame)
        {            
            Url = url;
            Title = title;
            ParentParameters = parentParameters;
            StaticInlineInLineParameters = staticInlineInLineParameters;
            DynamicParameters = dynamicParameters;
            DynamicClientParameters = dynamicClientParameters;
            Height = height;
            Width = width;
            Top = top;
            ShowInCurrentFrame = showInCurrentFrame;
            FunctionName = functionName;            
        }
        
        public string Url { get; private set; }

        public string Title { get; private set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public int? Top { get; set; }

        public bool ShowInCurrentFrame { get; set; }
        
        public string FunctionName { get; private set; }

        public string StaticInlineInLineParameters { get; private set; }

        public Dictionary<string, string> DynamicParameters { get; private set; }

        public Dictionary<string, string> DynamicClientParameters { get; private set; }
        
        public NameValueCollection ParentParameters { get; private set; }
        
        public string GetFunctionBody()
        {
            var functionBuilder = new StringBuilder();

            functionBuilder.AppendFormat("function {0}(parameters)", FunctionName);
            functionBuilder.AppendLine("{");

            foreach (var parameterName in DynamicClientParameters.Keys)
            {
                functionBuilder.AppendFormat("parameters['{0}'] = {1};", parameterName, DynamicClientParameters[parameterName]);
            }

            var settings = String.Format("height:{0}, width:{1}, calledWindow: window", Height, Width);
            if (Top.HasValue) settings += String.Format(", top:{0}", Top.Value);
            settings = "{" + settings + "}";
            var prefix = ShowInCurrentFrame ? "" : "top.";
            functionBuilder.AppendLine(String.Format(@"{0}overlay.show('{1}','{2}', parameters, {3}); ", prefix, Title, Url, settings));

            functionBuilder.AppendLine(String.Format("return {0}overlay;", prefix));

            functionBuilder.AppendLine("}");

            return functionBuilder.ToString();
        }

        public string GetParametersAsJsonObject()
        {
            var parameters = ParentParameters.Keys.Cast<string>().ToDictionary(name => name, name => ParentParameters[name]);

            foreach (var item in ParseInlineParameters(StaticInlineInLineParameters).Where(item => parameters.ContainsKey(item.Key) == false))
            {
                parameters.Add(item.Key, item.Value);
            }

            foreach (var item in DynamicParameters.Where(item => parameters.ContainsKey(item.Key) == false))
            {
                parameters.Add(item.Key, item.Value);
            }
            
            var s = new JavaScriptSerializer();

            parameters = new EscapeHelper().EscapeParameters(parameters);

            return s.Serialize(parameters);
        }

        private Dictionary<string, string> ParseInlineParameters(string inlineParameters)
        {
            var parameters = new Dictionary<string, string>();

            if (String.IsNullOrEmpty(inlineParameters))
            {
                return parameters;
            }

            foreach (var p in inlineParameters.Split(new[] { "&" }, StringSplitOptions.RemoveEmptyEntries))
            {
                var pp = p.Split(new[] { "=" }, StringSplitOptions.None);

                parameters.Add(pp[0], pp[1]);
            }

            return parameters;
        }
    }
}