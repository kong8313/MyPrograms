using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Confirmit.CATI.Core.Misc
{
    public static class TimeDiff
    {
        public static int Seconds(DateTime start, DateTime finish)
        {
            return (int)Math.Round((finish - start).TotalSeconds);
        }
    }
}
