using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StartStopTest
{
    public static class Globals
    {
        public static int Period = 60 * 1000;
        public static bool RandomStart = false;
        private static Random rnd = new Random((int)(DateTime.UtcNow.Ticks & (long)int.MaxValue));
        public static int GetStartDelay()
        {
            if (RandomStart)
            {
                return (rnd.Next(Period));
            }
            return (0);
        }
    }
}
