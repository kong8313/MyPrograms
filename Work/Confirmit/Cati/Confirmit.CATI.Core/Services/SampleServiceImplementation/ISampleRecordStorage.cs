using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;

namespace Confirmit.CATI.Core.Services.SampleServiceImplementation
{
    public interface ISampleRecordStorage
    {
        BvInterviewEntity Interview { get; set; }
        BvCallEntity Call { get; set; }
    }
}