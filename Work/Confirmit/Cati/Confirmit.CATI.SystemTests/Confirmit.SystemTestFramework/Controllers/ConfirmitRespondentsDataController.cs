using System;
using System.Collections.Generic;
using System.Data;
using Confirmit.SystemTestFramework.Settings;

namespace Confirmit.SystemTestFramework.Controllers
{
    public class ConfirmitRespondentsDataController : TestController
    {
        private readonly string _pid;

        public ConfirmitRespondentsDataController(UserInfo userInfo, string pid)
        {
            UserInfo = userInfo;
            _pid = pid;
        }

        public List<Respondent> Get()
        {
            var dataProvider = new DataProvider();
            var table = dataProvider.GetTableFromDb(_pid, "SELECT * FROM respondent");

            var respondents = new List<Respondent>();
            foreach (DataRow tableRow in table.Rows)
            {
                respondents.Add(new Respondent(tableRow, table.Columns));
            }

            return respondents;
        }
    }

    public class Respondent
    {
        public int Id { get; set; }
        public Dictionary<string, object> Values { get; set; }

        public Respondent(DataRow tableRow, DataColumnCollection columns)
        {
            Values = new Dictionary<string, object>();

            for (int i = 0; i < columns.Count; i++)
            {
                if (columns[i].ColumnName.ToLowerInvariant() == "respid")
                {
                    Id = Convert.ToInt32(tableRow[i]);
                    continue;
                }

                Values.Add(columns[i].ColumnName, tableRow[i]);
            }
        }
    }
}