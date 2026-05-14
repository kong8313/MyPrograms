
namespace Confirmit.CATI.Core.Services.PersonServiceImplementation
{
    public interface ICatiPersonItem
    {
        int Id
        {
            get;
        }

        string Name
        {
            get;
            set;
        }

        string Description
        {
            get;
            set;
        }

        void Init();
    }
}