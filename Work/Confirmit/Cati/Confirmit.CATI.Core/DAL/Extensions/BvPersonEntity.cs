using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Core.DAL.Generated.Entity.Table
{
    public partial class BvPersonEntity
    {
        public string LogInfo
        {
            get { return string.Format("({0})-'{1}'", Name, SID); }
        }

        public DialType DialType
        {
            get { return (DialType)DialTypeId;  }
        }
    }
}
