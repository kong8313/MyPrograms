using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confirmit.CATI.Supervisor.Core.Filters;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace IntegrationTests.Tests.FilterAndPaging.Tools
{
    internal class FilterField
    {
        TableTypes tableType;
        string column;
        VariableTypes coulmnType;
        FilterOperator sign;
        object value;
        bool disable;

        public FilterField(TableTypes tableType,
            string column,
            VariableTypes coulmnType,
            FilterOperator sign,
            object value,
            bool disable)
        {
            this.tableType = tableType;
            this.column = column;
            this.coulmnType = coulmnType;
            this.sign = sign;
            this.value = value;
            this.disable = disable;
        }

        public BvFilterFieldsEntity GetBvFilterFieldsEntity(int filterSid)
        {
            BvFilterFieldsEntity bvFilterFieldsEntity = new BvFilterFieldsEntity();

            bvFilterFieldsEntity.Column = column;
            bvFilterFieldsEntity.Table = (int)tableType;
            bvFilterFieldsEntity.FilterSID = filterSid;
            bvFilterFieldsEntity.Sign = (int)sign;
            bvFilterFieldsEntity.Type = (int)coulmnType;
            bvFilterFieldsEntity.Value = value.ToString();

            return bvFilterFieldsEntity;
        }

        public static FilterField CreateSomeFilterField()
        {
            return new FilterField(TableTypes.Call,
                "priority",
                VariableTypes.Integer,
                FilterOperator.Equal,
                34,
                false);
        }

        public static FilterField CreateFilterFieldForSubFilter(int filterId)
        {
            return new FilterField(TableTypes.Subfilter,
                "",
                VariableTypes.Subfilter,
                FilterOperator.Subfilter,
                filterId,
                false);
        }

        public static FilterField CreateSurveySpecificFilterField()
        {
            return new FilterField(TableTypes.CFVariables,
                "q1",
                VariableTypes.Integer,
                FilterOperator.Equal,
                123,
                false);
        }
    }
}
