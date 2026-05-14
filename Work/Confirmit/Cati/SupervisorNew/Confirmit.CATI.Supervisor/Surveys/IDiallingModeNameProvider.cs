using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Confirmit.CATI.Supervisor.Surveys
{
    public interface IDiallingModeNameProvider
    {
        List<DialingModeEntity> GetAll();
    }
}
