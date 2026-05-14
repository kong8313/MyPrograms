using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.DAL.Handmade.Entity.Table
{
    public class BvInterviewWithOriginEntity : BvInterviewEntity
    {
        public BvInterviewWithOriginEntity(BvInterviewEntity interview)
        {
            interview.CopyTo(this);
            Origin = interview;
        }

        public BvInterviewEntity Origin { get; private set; }
    }
}
