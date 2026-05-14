using System.Collections;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.EmailReports
{
    public interface IReport
    {
        string Title { get; }
        string Name { get; }
        IEnumerable ReportDataSource { get; }

        ICollection<KeyValuePair<string, object>> ReportParametersCollection { get; }
    }
}