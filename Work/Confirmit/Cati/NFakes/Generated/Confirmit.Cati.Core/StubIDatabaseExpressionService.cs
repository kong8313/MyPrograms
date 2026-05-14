using System;
using Confirmit.CATI.Core.Services.Database.Interfaces;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.Database.Interfaces.Fakes
{
    public class StubIDatabaseExpressionService : IDatabaseExpressionService 
    {
        private IDatabaseExpressionService _inner;

        public StubIDatabaseExpressionService()
        {
            _inner = null;
        }

        public IDatabaseExpressionService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<DatabaseExpression> ParseStringDelegate(string filter);
        public ParseStringDelegate ParseString;

        List<DatabaseExpression> IDatabaseExpressionService.Parse(string filter)
        {


            if (ParseString != null)
            {
                return ParseString(filter);
            } else if (_inner != null)
            {
                return ((IDatabaseExpressionService)_inner).Parse(filter);
            }

            return default(List<DatabaseExpression>);
        }

    }
}