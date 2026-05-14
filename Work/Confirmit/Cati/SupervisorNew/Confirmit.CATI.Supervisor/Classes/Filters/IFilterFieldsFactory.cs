using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Supervisor.Classes.Filters
{
    public interface IFilterFieldsFactory
    {
        IEnumerable<BvFilterFieldsEntity> Create(string fieldsXml);
    }
}