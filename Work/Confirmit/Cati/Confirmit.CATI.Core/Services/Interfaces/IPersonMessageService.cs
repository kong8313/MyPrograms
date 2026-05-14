using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface IPersonMessageService
    {
        void CleanMessages(TimeSpan expirationTime);
    }
}
