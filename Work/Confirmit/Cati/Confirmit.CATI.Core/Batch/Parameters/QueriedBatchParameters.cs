namespace Confirmit.CATI.Core.Batch
{
    public class QueriedBatchParameters: BatchParameters
    {
        public string SqlQuery { get; set; }

        public QueriedBatchParameters(){}

        public QueriedBatchParameters(string sqlQuery)
        {
            SqlQuery = sqlQuery;
        }

        public override BatchType Type { get { return BatchType.Queried; } }

    }
}
