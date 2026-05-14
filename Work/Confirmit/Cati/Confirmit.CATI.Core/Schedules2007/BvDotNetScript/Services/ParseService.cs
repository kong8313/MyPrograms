using System;
using System.Linq;

namespace Confirmit.CATI.Core.Schedules2007.BvDotNetScript.Services
{
    public class ParseService
    {
        public int[] StringToIntArray(string value, string separator)
        {
            return value.Split(new[] {separator}, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
        }
    }
}
