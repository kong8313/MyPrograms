using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.FilterServiceImplementation;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

namespace Confirmit.CATI.IntegrationTests.Framework.Data.Builders
{
    class FilterDataBuilder : BaseObjectBuilder<FilterData>
    {
        private Regex ConditionPattern = new Regex(@"(?<Table>[\w\d]+)\.(?<Column>[\w\d]+)(?<Sign>[=]+)(?<Value>.+)");
        public FilterDataBuilder(TestDataContext context, FilterData data, DataGenerator dataGenerator) : base(context, data, dataGenerator)
        {
        }

        public override void Create()
        {
            var filter = new BvFiltersEntity()
            {
                Name = "Filter:" + Data.Tag,
                AndOrOperator = Data.Join == FilterJoinType.And ? (byte) AndOrOperator.And : (byte) AndOrOperator.Or,
                Hidden = 0
            };

            var id = FilterRepository.Insert(filter);

            var fields = new List<BvFilterFieldsEntity>();

            foreach (var condition in Data.Conditions)
            {
                var match = ConditionPattern.Match(condition);
                
                if (!match.Success)
                    throw new Exception(String.Format("Wrong condition format for:{0}", condition));

                var table = Enum.Parse(typeof(TableTypes), match.Groups["Table"].Value);
                var column = match.Groups["Column"].Value;
                var sign = GetFilterSign(match.Groups["Sign"].Value);
                var value = match.Groups["Value"].Value;

                fields.Add(new BvFilterFieldsEntity()
                {
                    Table = (int)table,
                    Column = column,
                    Sign = (int)sign,
                    Value = value
                });
            }

            FilterService.SetFields(id, fields);

            Context.Filters.Add(new FilterController(Data.Tag, id, Context));
        }

        private FilterOperator GetFilterSign(string signText)
        {
            if(signText == "=")
                return FilterOperator.Equal;

            throw new Exception(String.Format("Unknown sign in filter condition: {0}", signText));
        }
    }
}