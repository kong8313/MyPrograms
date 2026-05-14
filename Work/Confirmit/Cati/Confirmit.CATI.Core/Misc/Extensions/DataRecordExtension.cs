using System;
using System.Data;

namespace Confirmit.CATI.Core.Misc.Extensions
{
    public static class DataRecordExtension
    {
        public static T GetValueOrDefault<T>(this IDataRecord row, string fieldName, T defaultValue = default(T))
        {
            for (var i = 0; i < row.FieldCount; i++)
                if (row.GetName(i) == fieldName)
                    return (T)Convert.ChangeType(row[i], typeof(T));
            return defaultValue;
        }
    }
}