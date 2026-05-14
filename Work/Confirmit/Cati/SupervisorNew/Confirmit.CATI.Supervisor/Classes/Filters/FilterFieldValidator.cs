using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Classes.Filters
{
    internal class FilterFieldValidator : IFilterFieldValidator
    {
        public void Validate(BvFilterFieldsEntity field)
        {
            switch (field.Type)
            {
                case (int)VariableTypes.Integer:
                case (int)VariableTypes.Subfilter:
                    ValidateInteger(field);
                    break;
                case (int)VariableTypes.PredefinedValue:
                    ValidatePredefined(field);
                    break;
            }
        }

        private static void ValidatePredefined(BvFilterFieldsEntity field)
        {
            if ((field.Sign == (int) FilterOperator.Equal || field.Sign == (int) FilterOperator.NotEqual) == false)
            {
                throw new UserMessageException(string.Format(Strings.InvalidFilterOperator, field.Column));
            }
        }

        private static void ValidateInteger(BvFilterFieldsEntity field)
        {
            int temp;
            if (!Int32.TryParse(field.Value, out temp))
            {
                throw new UserMessageException(String.Format(Strings.InvalidValueOfVariable, field.Column));
            }
        }
    }
}