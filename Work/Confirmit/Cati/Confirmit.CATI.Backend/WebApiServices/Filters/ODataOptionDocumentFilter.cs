using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace Confirmit.CATI.Backend.WebApiServices.Filters
{
    public class ODataOptionDocumentFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.parameters == null)
            {
                return;
            }

            var allowedParameters = new List<Parameter>();
            foreach (var parameter in operation.parameters)
            {
                if (IsParameterAllowed(parameter))
                {
                    allowedParameters.Add(parameter);
                }
            }

            operation.parameters.Clear();
            operation.parameters = allowedParameters;
        }

        private bool IsParameterAllowed(Parameter parameter)
        {
            var notAllowedParameters = new List<string> { "$select", "$expand", "$count" };

            if (notAllowedParameters.Any(x => x == parameter.name.ToLowerInvariant()))
            {
                return false;
            }

            return true;
        }
    }
}