using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.Text;
using Confirmit.CATI.Common.Exceptions;

namespace Confirmit.CATI.Common.WcfTools.ErrorContextHandler
{
    public class ErrorContextLogger
    {
        /// <summary>
        /// Gets or sets the additional text to be included in the error message.
        /// </summary>
        /// <value>The additional text.</value>
        public string AdditionalText { get; set; }

        /// <summary>
        /// Log the extended error information including Service Name, Action, User Identity Name, Method name and parameter values.
        /// </summary>
        /// <param name="error">The exception thrown during processing.</param>
        /// <param name="errorContext">The error context.</param>
        public void LogError(Exception error, ErrorContext errorContext)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(error.GetType().Name + ": " + error.Message);
            builder.AppendLine();
            builder.AppendLine("WCF extended error trace results:");
            builder.AppendLine();
            builder.AppendLine("Service Name: " + errorContext.ServiceName);
            if (!string.IsNullOrEmpty(errorContext.ToHeader))
            {
                builder.AppendLine("Endpoint URI: " + errorContext.ToHeader);
            }

            builder.AppendLine("Action: " + errorContext.Action);
            if (!string.IsNullOrEmpty(errorContext.IdentityName))
            {
                builder.AppendLine("User Identity Name: " + errorContext.IdentityName);
            }

            string methodParameters = GetMethodParameters(errorContext);

            builder.AppendLine();
            builder.AppendLine("Method:");
            builder.AppendFormat("{0}({1})", errorContext.MethodName, methodParameters);
            builder.AppendLine();

            builder.AppendLine();
            builder.AppendLine("Exception details:");
            builder.AppendLine(error.ToString());

            if (!string.IsNullOrEmpty(AdditionalText))
            {
                builder.AppendLine();
                builder.AppendLine(AdditionalText);
            }

            if (error is UserMessageException || error is FaultException<UserMessageException>)
            {
                Trace.TraceWarning(builder.ToString());
            }
            else
            {
                Trace.TraceError(builder.ToString());
            }
        }

        /// <summary>
        /// Gets the method parameters as a string.
        /// </summary>
        /// <param name="errorContext">The error context.</param>
        /// <returns>The string contains method parameters and their values.</returns>
        private string GetMethodParameters(ErrorContext errorContext)
        {
            if (errorContext.Parameters == null)
            {
                return "<Parameters are not available>";
            }

            var methodInfo = errorContext.ServiceType.GetMethod(errorContext.MethodName);

            var values = errorContext.Parameters.Select(x => GetSerializedParameter(x)).ToArray();

            if (methodInfo != null)
            {
                var parameters = methodInfo.GetParameters().Where(x => !x.IsOut).ToArray();
                if (parameters.Length == values.Length)
                {
                    var parametersString = new StringBuilder();
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        parametersString.AppendFormat(
                            "{0} = {1}",
                            parameters[i].Name,
                            HasHideParameterValueWhileLoggingAttribute(parameters[i])
                                ? "***hidden information***"
                                : values[i]);

                        if (i != parameters.Length - 1)
                        {
                            parametersString.Append(", ");
                        }
                    }

                    return parametersString.ToString();
                }
            }

            return String.Join(", ", values);
        }

        private bool HasHideParameterValueWhileLoggingAttribute(ParameterInfo parameterInfo)
        {
            return Attribute.GetCustomAttributes(parameterInfo).Any(attr => attr.GetType() == typeof(HideParameterValueWhileLoggingAttribute));
        }

        /// <summary>
        /// Gets the serialized parameter value. 
        /// JSON serialization is used to make text more readable.
        /// </summary>
        /// <param name="parameter">The parameter value.</param>
        /// <returns>The serialized parameter value.</returns>
        private static string GetSerializedParameter(object parameter)
        {
            if (parameter == null)
            {
                return "(null)";
            }

            try
            {
                using (Stream stream = new MemoryStream())
                {
                    new DataContractJsonSerializer(parameter.GetType()).WriteObject(stream, parameter);
                    stream.Seek(0, SeekOrigin.Begin);
                    return new StreamReader(stream).ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Cannot serialize parameter value. Exception {0}", e);

                return "(exception occured, see log)";
            }
        }
    }
}