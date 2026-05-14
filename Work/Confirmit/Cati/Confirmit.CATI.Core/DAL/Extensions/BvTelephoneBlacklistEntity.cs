using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Core.DAL.Generated.Entity.Table
{
    partial class BvTelephoneBlacklistEntity
    {
        public BlacklistPatternType PatternType
        {
            get { return (BlacklistPatternType) Type; }
            set { Type = (byte) value; }
        }

        public string DisplayPattern
        {
            get
            {
                switch (PatternType)
                {
                    case BlacklistPatternType.Equal:
                        return TelephoneNumber;
                    case BlacklistPatternType.StartWith:
                        return TelephoneNumber + "*";
                    default:
                        throw new Exception(String.Format("Unknown blacklist pattern type {0}.", PatternType));
                }
            }
            set
            {
                if (value.EndsWith("*"))
                {
                    TelephoneNumber = value.TrimEnd('*');
                    PatternType = BlacklistPatternType.StartWith;
                }
                else
                {
                    TelephoneNumber = value;
                    PatternType = BlacklistPatternType.Equal;
                }
            }
        }
    }
}
