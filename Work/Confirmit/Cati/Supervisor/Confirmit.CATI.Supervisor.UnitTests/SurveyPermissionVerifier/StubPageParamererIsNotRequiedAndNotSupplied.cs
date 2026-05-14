using System.Web;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.UnitTests.SurveyPermissionVerifier
{
    [CheckSurveyPermission(RequestParameterName = "Id", IsRequired = false)]
    internal class StubPageParamererIsNotRequiedAndNotSupplied : IBaseForm
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