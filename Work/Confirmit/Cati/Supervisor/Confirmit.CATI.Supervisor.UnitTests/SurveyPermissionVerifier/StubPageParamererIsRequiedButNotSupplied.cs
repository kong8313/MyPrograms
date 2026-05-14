using System.Web;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.UnitTests.SurveyPermissionVerifier
{
    [CheckSurveyPermission(RequestParameterName = "Id", IsRequired = true)]
    internal class StubPageParamererIsRequiedButNotSupplied : IBaseForm
    {
        public HttpRequest Request
        {
            get
            {
                return new HttpRequest("", "http://localhost/test", "ProjectId=p1234567");
            }
        }
    }
}