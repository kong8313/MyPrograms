using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.Database.Interfaces
{
    public class DatabaseExpression
    {
        public string ColumnName;
        public string Value;
        public string EscapedSqlColumnName;
        //public SqlOperation Operator;//Now we implement only enqual operation
    }

    public interface IDatabaseExpressionService
    {
        List<DatabaseExpression> Parse(string filter);
    }
}