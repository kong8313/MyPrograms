using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Validators.Interfaces;

namespace Confirmit.CATI.Core.Validators
{
    public class PersonGroupValidator : IPersonGroupValidator
    {
        public bool IsNameValid(string name)
        {
            var wrongSimbols = new[] { ',' };

            return !name.Any(wrongSimbols.Contains);
        }

        public bool IsValid(BvPersonGroupEntity personGroup)
        {
            return IsNameValid(personGroup.Name);
        }
    }
}
