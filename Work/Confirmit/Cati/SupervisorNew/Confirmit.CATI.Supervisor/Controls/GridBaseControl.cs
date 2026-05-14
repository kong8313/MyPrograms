using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.Controls
{
    /// <summary>
    /// Base class for GeneralGrid and HierarchicalGrid
    /// </summary>
    public abstract class GridBaseControl : BaseWUC
    {        
       public abstract string ClientGetCurrentRow();
       public abstract string ClientGetSelectedRows();
    }    
}
