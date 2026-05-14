using System;
using System.Collections.Generic;
using System.Xml;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.Supervisor.Core.Timezone;

namespace Confirmit.CATI.Supervisor.Classes.Filters
{
    internal class FilterFieldsFactory : IFilterFieldsFactory
    {
        private readonly IFilterFieldValidator _validator;
        private readonly ICachedLocalTimezoneManager _timezoneProvider;

        public FilterFieldsFactory(IFilterFieldValidator validator, ICachedLocalTimezoneManager timezoneProvider)
        {
            if (validator == null)
            {
                throw new ArgumentNullException("validator");
            }

            if (timezoneProvider == null)
            {
                throw new ArgumentNullException("timezoneProvider");
            }

            _validator = validator;
            _timezoneProvider = timezoneProvider;
        }

        public IEnumerable<BvFilterFieldsEntity> Create(string fieldsXml)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(fieldsXml);
            XmlNode root = xmlDoc.DocumentElement;
            var fields = new List<BvFilterFieldsEntity>();

            foreach (XmlNode node in root.SelectNodes("var"))
            {
                var type = Convert.ToInt32(node.SelectSingleNode("VarType").InnerText);
                var value = node.SelectSingleNode("Value").InnerText;

                //we should convert datetime from timezone to utc 
                if ((VariableTypes)type == VariableTypes.Date)
                {
                    value = GetDateValueConvertedFromLocalToUtc(value);
                }

                var field = new BvFilterFieldsEntity
                                {
                                    Table = Convert.ToInt32(node.SelectSingleNode("TableType").InnerText),
                                    Column = node.SelectSingleNode("Column").InnerText,
                                    Type = type,
                                    Sign = Convert.ToInt32(node.SelectSingleNode("Sign").InnerText),
                                    Value = value,
                                    IsNeedCast = Convert.ToBoolean(node.SelectSingleNode("IsBackground").InnerText)
                                };

                _validator.Validate(field);
                fields.Add(field);
            }

            return fields;
        }

        private string GetDateValueConvertedFromLocalToUtc(string value)
        {
            var date = DateTime.Parse(value);
            return _timezoneProvider.ConvertToUtc(date).ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}