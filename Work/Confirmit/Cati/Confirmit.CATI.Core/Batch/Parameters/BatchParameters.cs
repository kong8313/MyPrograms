using System.Xml.Serialization;

namespace Confirmit.CATI.Core.Batch
{
    [XmlInclude(typeof(FilteredByCellsBatchParameters))]
    [XmlInclude(typeof(FilteredByMultipleCellsBatchParameters))]
    [XmlInclude(typeof(FilteredByClosedQuotaCellBatchParameters))]
    [XmlInclude(typeof(FilteredByOpenedQuotaCellBatchParameters))]
    [XmlInclude(typeof(FilteredBatchParameters))]
    [XmlInclude(typeof(SelectedBatchParameters))]
    [XmlInclude(typeof(QueriedBatchParameters))]
    public abstract class BatchParameters
    {
        public abstract BatchType Type { get; }
    }
}