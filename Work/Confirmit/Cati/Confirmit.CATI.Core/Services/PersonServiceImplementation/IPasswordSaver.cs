using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Services.PersonServiceImplementation
{
    public interface IPasswordSaver
    {
        void Save(int personId, string password);
        void Save(BvPersonEntity person, string password);
    }
}
