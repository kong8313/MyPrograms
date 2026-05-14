using System;
using Confirmit.CATI.Common.Random;

namespace Confirmit.CATI.IntegrationTests.Framework.Tools
{
    public class DataGenerator
    {
        private int _uniqueId = 1;

        public string NewName(string prefix)
        {
            return $"{prefix} {NewId()}";
        }

        public int NewId()
        {
            return _uniqueId++;
        }

        public string NewProjectId()
        {
            var compaign = Int32.MaxValue + (long) Randomizer.Next(10000000);
            return $"p{compaign}";
        }
    }
}
