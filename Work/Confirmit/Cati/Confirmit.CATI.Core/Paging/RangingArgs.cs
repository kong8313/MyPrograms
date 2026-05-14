namespace Confirmit.CATI.Core.Paging
{
    public class RangingArgs
    {
        public RangingArgs(int start, int count, SortingArgs sorting)
        {
            Start = start;
            Count = count;
            Sorting = sorting;
        }

        public RangingArgs(int start, int count, string orderField, bool isAscending)
        {
            Start = start;
            Count = count;
            Sorting = new SortingArgs(orderField, isAscending);
        }

        public int Start { get; private set; }

        public int Count { get; private set; }

        public SortingArgs Sorting { get; set; }
    }
}