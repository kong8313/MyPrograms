using System.Collections.Generic;
using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace Confirmit.CATI.Backend.WebApiServices.Authorization
{
    public class AddAuthorizationHeaderParameterOperationFilter// : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.parameters == null)
            {
                operation.parameters = new List<Parameter>();
            }

            operation.parameters?.Add(new Parameter
            {
                name = "X-Confirmit-ApiKey",
                @in = "header",
                description = "X Confirmit ApiKey",
                required = false,
                type = "string"
            });
        }
    }
}