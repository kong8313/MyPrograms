using BvCallHandlerLibrary;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.BvCallHandlerLibrary
{
    public interface IDialerInstanceFactory
    {
        IDialerInstance Create(BvDialersEntity dialerEntity);
    }
}