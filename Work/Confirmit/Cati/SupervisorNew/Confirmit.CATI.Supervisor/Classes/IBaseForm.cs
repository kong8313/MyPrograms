using System.Web;

namespace Confirmit.CATI.Supervisor.Classes
{
    public interface IBaseForm
    {
        HttpRequest Request { get; }
    }
}
