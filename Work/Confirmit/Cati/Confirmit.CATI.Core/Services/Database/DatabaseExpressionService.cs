using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Confirmit.CATI.Core.Services.Database.Interfaces;

namespace Confirmit.CATI.Core.Services.Database
{
    public class DatabaseExpressionService : IDatabaseExpressionService
    {
        private readonly IDatabaseIdentifierService _databaseIdentifierService;

        public DatabaseExpressionService(IDatabaseIdentifierService databaseIdentifierService)
        {
            _databaseIdentifierService = databaseIdentifierService;
        }

        public List<DatabaseExpression> Parse(string filter)
        {
            var result = new List<DatabaseExpression>();

            if (String.IsNullOrEmpty(filter))
                return result;

            var expressions = Regex.Split(filter, "AND", RegexOptions.IgnoreCase);
            
            foreach (var expression in expressions)
            {
                var expressionParts = expression.Split(new[] {"="}, StringSplitOptions.None);
                if(expressionParts.Length != 2)
                    throw new Exception(String.Format("filter syntax error in:{0}", expression));
                
                var column = expressionParts[0].Trim();
                var value = expressionParts[1].Trim();

                result.Add(new DatabaseExpression()
                {
                    ColumnName = column,
                    Value = value,
                    EscapedSqlColumnName = _databaseIdentifierService.GetEscapedIdentifier(column)
                });
            }

            return result;
        }
    }
}