using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.Services.PersonImport;

namespace Confirmit.CATI.Supervisor.Core.Persons.Import
{
    public class GetRolesFromStringProvider
    {
        /// <summary>
        /// Gets a hashtable, where keys are column names and values are column roles
        /// </summary>
        /// <param name="str">Input string like 'column1=role1;column2=role2;...'</param>
        /// <returns>dictionary that contains Column name - Column role mapping</returns>
        public static Dictionary<string, ColumnRole> GetColumnNameToRoleMap(string str)
        {
            var columnRoleMap = new Dictionary<string, ColumnRole>();

            if (str == null)
            {
                return columnRoleMap;
            }

            string[] roles = str.Split(';');
            foreach (string role in roles)
            {
                string[] roleFields = role.Split('=');
                if (roleFields.Length < 2)
                {
                    continue;
                }

                string columnName = roleFields[0];
                ColumnRole columnRole;
                try
                {
                    columnRole = (ColumnRole)Convert.ToInt32(roleFields[1]);
                }
                catch
                {
                    continue;
                }

                columnRoleMap.Add(columnName, columnRole);
            }

            return columnRoleMap;
        }
    }
}
