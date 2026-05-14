namespace Confirmit.CATI.Core.Batch
{
    public enum BatchType
    {
        FilteredByCells,
        FilteredByClosedQuotaCell,
        FilteredByOpenedQuotaCell,
        Filtered,
        Selected,
        Queried,
        FilteredByBlacklist,
        FilteredByMultipleCells
    }
}