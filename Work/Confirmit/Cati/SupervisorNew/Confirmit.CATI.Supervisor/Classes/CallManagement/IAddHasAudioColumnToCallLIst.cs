using System.Data;

namespace Confirmit.CATI.Supervisor.Classes.CallManagement
{
    internal interface IAddHasAudioColumnToCallList
    {
        void Add(DataTable list, int surveySid);
    }
}
