using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Confirmit.CATI.Common.Types
{
    public class Range<T> 
    {
        public T From { get; set; }
        public T To { get; set; }

        public Range(T @from, T to)
        {
            From = @from;
            To = to;
        }
    }
}
