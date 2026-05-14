using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Validators.Interfaces
{
    public interface IPersonGroupValidator
    {
        bool IsNameValid(string name);

        bool IsValid(BvPersonGroupEntity personGroup);


    }
}