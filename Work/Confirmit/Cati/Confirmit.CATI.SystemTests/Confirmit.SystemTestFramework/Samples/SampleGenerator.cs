using System;
using System.Text;

namespace Confirmit.SystemTestFramework.Samples
{
    public class SampleGenerator
    {
        public string Generate(int count, params ColumnType[] columnType)
        {
            var result = new StringBuilder();

            for (int i = 0; i < columnType.Length; i++)
            {
                result.Append(columnType[i]);

                if (i < columnType.Length - 1)
                {
                    result.Append("\t");
                }
            }

            result.AppendLine();

            for (var i = 0; i < count; i++)
            {
                for (var j = 0;j < columnType.Length; j++)
                {
                    result.Append(GetValueFor(i + 1, columnType[j]));

                    if (j < columnType.Length - 1)
                    {
                        result.Append("\t");
                    }
                }

                if (i < count - 1)
                {
                    result.AppendLine();
                }
            }

            return result.ToString();
        }

        private string GetValueFor(int index, ColumnType type)
        {
            switch (type)
            {
                case ColumnType.TelephoneNumber:
                    return index.ToString();
                case ColumnType.Email:
                    return String.Format("person{0}@firmsw.no", index);
                default:
                    return string.Empty;
            }
        }
    }
}
