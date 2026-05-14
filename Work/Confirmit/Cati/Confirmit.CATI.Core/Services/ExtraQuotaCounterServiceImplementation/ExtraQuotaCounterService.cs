using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Handmade.Entity;

namespace Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation
{
    public class ExtraQuotaCounterService : IExtraQuotaCounterService
    {
        public static IEnumerable<QuotaCellCounter> ExecuteExtraCellCounterQuery(string query)
        {
            using (var reader = new DatabaseEngine().ExecuteReaderInNewConnection(query, CommandType.Text))
            {
                while (reader.Read())
                {
                    yield return new QuotaCellCounter
                        {
                            Descriptor = (string)reader["CellDescriptor"],
                            Value = (int)reader["Counter"]
                        };
                }
            }
        }

        public static string CreateCellDescriptor(string[] fieldsOrder, string[] precodeFields, string[] precodeValues)
        {
            var orderedPrecodes = fieldsOrder.Select(x =>  precodeValues[Array.IndexOf(precodeFields, x)]);
            return String.Join(",", orderedPrecodes);
        }

        IExtraQuotaCounterCalculator IExtraQuotaCounterService.Create(IExtraQuotaCounterParameters parameters)
        {
            return Create(parameters);
        }

        public static IExtraQuotaCounterCalculator Create(IExtraQuotaCounterParameters parameters)
        {
            var type = parameters.GetType();

            if (type == typeof(CallsCounterParameter))
            {
                return new CallsCounterCalculator(parameters as CallsCounterParameter);
            }
            else if (type == typeof(InterviewsCounterParameter))
            {
                return new InterviewsCounterCalculator(parameters as InterviewsCounterParameter);
            }
            else if (type == typeof (DailyCounterParameter))
            {
                return new DailyCallCounterCalculator(parameters as DailyCounterParameter);
            }

            throw new Exception("Unknown counter parameter");
        }


        public static IEnumerable<KeyValuePair<int, int>> ExecuteExtraItsCellCounterQuery(string query)
        {
            using (var reader = new DatabaseEngine().ExecuteReaderInNewConnection(query, CommandType.Text))
            {
                while (reader.Read())
                {
                    yield return new KeyValuePair<int, int>((int)reader["ITS"], (int)reader["Counter"]);
                }
            }
        }
    }
}
