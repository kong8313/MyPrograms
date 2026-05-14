using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Core.SystemSettings
{
    public partial interface IFCDSettings
    {

        /// <summary>
        /// Type of FCD's algorithm. Following types are allowed: 0-delete calls, 1-disable calls with reenabling on opening cell(s)
        /// </summary>
        FcdAlgorithmType AlgorithmType { get; set; }
    }

    public partial class FCDSettings
    {
        public FcdAlgorithmType AlgorithmType
        {
            get { return (FcdAlgorithmType) BehaviorType; }
            set { BehaviorType = (int) value; }
        }
    }
}
