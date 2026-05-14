using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Supervisor.Classes.Filters
{
    public class FilterData
    {
        public BvFiltersEntity Filter { get; set; }
        public IEnumerable<BvFilterFieldsEntity> Fields { get; set; }
    }
}