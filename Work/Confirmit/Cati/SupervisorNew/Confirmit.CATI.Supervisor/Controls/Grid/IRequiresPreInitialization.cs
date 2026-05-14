using System.Web.UI;

namespace Confirmit.CATI.Supervisor.Controls.Grid
{
    public interface IRequiresPreInitialization
    {
        void PreInitialize(Control owner);
    }
}